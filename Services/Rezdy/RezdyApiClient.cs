// Services/Rezdy/RezdyApiClient.cs
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Services.Rezdy
{
    public class RezdyApiClient : IRezdyApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<RezdyApiClient> _logger;

        public RezdyApiClient(
            HttpClient httpClient,
            ILogger<RezdyApiClient> logger)
        {
            _http = httpClient;
            _logger = logger;
        }

        private async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Rezdy raw response: {Json}", json);

            var jObj = JObject.Parse(json);

            // 1️⃣ requestStatus kontrolü
            var status = jObj["requestStatus"]?.ToObject<RequestStatus>()
                         ?? throw new ApplicationException("Missing requestStatus in Rezdy response");

            if (!status.Success)
            {
                var codeEnum = status.ErrorCodeEnum ?? RezdyErrorCode.UNKNOWN;
                var msg = status.Error?.ErrorMessage ?? "Unknown error";
                throw new RezdyApiException(msg, codeEnum);
            }

            // 2️⃣ responseData’yı T tipine çevir
            var dataToken = jObj["responseData"];
            if (dataToken == null)
                throw new ApplicationException("Missing responseData in Rezdy response");

            return dataToken.ToObject<T>()!;
        }



        private async Task<HttpResponseMessage> SendWithRetryAsync(HttpRequestMessage req, int maxRetries = 3)
        {
            for (var attempt = 0; attempt < maxRetries; attempt++)
            {
                var res = await _http.SendAsync(req);
                if (res.StatusCode != (HttpStatusCode)406)
                    return res;

                // Rate‐limit aşıldı
                if (attempt == maxRetries - 1)
                    throw new RateLimitExceededException("Rezdy rate limit exceeded");

                if (res.Headers.TryGetValues("Retry-After", out var values) &&
                    int.TryParse(values.FirstOrDefault(), out var secs))
                {
                    await Task.Delay(TimeSpan.FromSeconds(secs));
                }
                else
                {
                    // Default backoff
                    await Task.Delay(TimeSpan.FromSeconds(5 * (attempt + 1)));
                }
            }
            // Reachable değil ama compiler için:
            throw new RateLimitExceededException("Rezdy rate limit retry failed");
        }


        public async Task<AvailabilityResponse> GetAvailabilityAsync(
    IEnumerable<string> productCodes,
    DateTime startLocal,
    DateTime endLocal)
        {
            // query string’i inşa et
            var parts = new List<string>();
            foreach (var code in productCodes)
            {
                parts.Add($"productCode={Uri.EscapeDataString(code)}");
            }
            // local format: yyyy-MM-dd HH:mm:ss
            var fmt = "yyyy-MM-dd HH:mm:ss";
            parts.Add($"startTimeLocal={Uri.EscapeDataString(startLocal.ToString(fmt))}");
            parts.Add($"endTimeLocal={Uri.EscapeDataString(endLocal.ToString(fmt))}");

            var requestUri = "/v1/availability?" + string.Join("&", parts);

            var req = new HttpRequestMessage(HttpMethod.Get, requestUri);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var resp = await _http.SendAsync(req);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            // JSON’u parse et ve modelle eşleştir
            var raw = await resp.Content.ReadAsStringAsync();
            var jObj = JObject.Parse(raw);

            var status = jObj["requestStatus"]?.ToObject<RequestStatus>()
                         ?? throw new ApplicationException("Missing requestStatus");
            if (!status.Success)
            {
                var error = status.Error;
                var code = status.ErrorCodeEnum ?? RezdyErrorCode.UNKNOWN;
                var msg = error?.ErrorMessage ?? "Unknown error";
                throw new RezdyApiException(msg, code);
            }

            var sessionsToken = jObj["sessions"]
                ?? throw new ApplicationException("Missing sessions in availability response");

            var sessions = sessionsToken.ToObject<AvailabilitySession[]>()!;
            return new AvailabilityResponse
            {
                RequestStatus = status,
                Sessions = sessions
            };
        }

        public async Task<AvailabilityResponse> UpdateAvailabilityAsync(int sessionId, AvailabilityRequest request)
        {
            var httpReq = new HttpRequestMessage(HttpMethod.Put, $"/v1/availability/{sessionId}")
            {
                Content = JsonContent.Create(request)
            };
            httpReq.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };

            var resp = await _http.SendAsync(httpReq);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            return await ReadResponseAsync<AvailabilityResponse>(resp);
        }

        public async Task<AvailabilityResponse> CreateAvailabilityAsync(AvailabilityRequest request)
        {
            var httpReq = new HttpRequestMessage(HttpMethod.Post, "/v1/availability")
            {
                Content = JsonContent.Create(request)
            };
            httpReq.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };

            var resp = await _http.SendAsync(httpReq);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            return await ReadResponseAsync<AvailabilityResponse>(resp);
        }

        // Diğer metotlar için de aynı deseni kullanın:
        public async Task<ProductResponse> CreateProductAsync(ProductCreateRequest request)
        {
            var httpReq = new HttpRequestMessage(HttpMethod.Post, "/v1/products")
            {
                Content = JsonContent.Create(request)
            };
            httpReq.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };

            var resp = await _http.SendAsync(httpReq);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            return await ReadResponseAsync<ProductResponse>(resp);
        }


        public async Task<ImageUploadResponse> AddProductImageAsync(string productCode, MultipartFormDataContent content)
        {
            var resp = await _http.PostAsync($"/v1/products/{productCode}/images", content);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            return await ReadResponseAsync<ImageUploadResponse>(resp);
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request)
        {
            // 1️⃣ Payload’ı hazırla
            var httpReq = new HttpRequestMessage(HttpMethod.Post, "/v1/bookings")
            {
                Content = JsonContent.Create(request)
            };
            httpReq.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };

            // 2️⃣ Gönder ve 406 rate‐limit kontrolü
            var resp = await _http.SendAsync(httpReq);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            // 3️⃣ requestStatus kontrolü ve responseData parse
            return await ReadResponseAsync<BookingResponse>(resp);
        }

        // 7️⃣ Rezervasyonu güncelle
        public async Task<BookingResponse> UpdateBookingAsync(
       string orderNumber,
       BookingUpdateRequest request)
        {
            var httpReq = new HttpRequestMessage(HttpMethod.Put, $"/v1/bookings/{orderNumber}")
            {
                Content = JsonContent.Create(request)
            };
            httpReq.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/json") { CharSet = "UTF-8" };

            var resp = await _http.SendAsync(httpReq);
            if (resp.StatusCode == (HttpStatusCode)406)
                throw new RateLimitExceededException("Too Many Requests to Rezdy API");
            resp.EnsureSuccessStatusCode();

            return await ReadResponseAsync<BookingResponse>(resp);
        }

        // 8️⃣ Rezervasyonu iptal et
        public async Task<CancelBookingResponse> CancelBookingAsync(string orderNumber)
        {
            // Rezdy doc’a göre iptal endpoint’i DELETE /v1/bookings/{orderNumber}
            var httpReq = new HttpRequestMessage(HttpMethod.Delete, $"/v1/bookings/{orderNumber}");
            var resp = await _http.SendAsync(httpReq);
            if (resp.StatusCode == (HttpStatusCode)406) throw new RateLimitExceededException("Too Many Requests");
            resp.EnsureSuccessStatusCode();

            return await ReadResponseAsync<CancelBookingResponse>(resp);
        }

        // 1️⃣ Ürünleri getir (paginated)
        public async Task<PaginatedResponse<ProductDetail>> GetProductsAsync(int offset = 0, int limit = 100)
        {
            limit = Math.Min(limit, 100);
            var url = $"/v1/products?offset={offset}&limit={limit}";
            var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var resp = await _http.SendAsync(req);
            if (resp.StatusCode == (HttpStatusCode)406) throw new RateLimitExceededException("Too Many Requests");
            resp.EnsureSuccessStatusCode();

            // doğrudan ReadResponseAsync kullanmıyoruz, çünkü root’ta responseData değil top-level’de products[] var:
            var json = await resp.Content.ReadAsStringAsync();
            var jObj = JObject.Parse(json);

            var status = jObj["requestStatus"]!.ToObject<RequestStatus>()!;
            if (!status.Success) throw new RezdyApiException(status.Error.ErrorMessage, status.ErrorCodeEnum!.Value);

            var items = jObj["products"]!.ToObject<ProductDetail[]>()!;
            var total = jObj["total"]?.Value<int>() ?? items.Length;

            return new PaginatedResponse<ProductDetail>
            {
                RequestStatus = status,
                ResponseData = new PaginatedData<ProductDetail>
                {
                    Items = items,
                    Offset = offset,
                    Limit = limit,
                    Total = total
                }
            };
        }


        //public async Task<ProductResponseData> CreateProductAsync(ProductCreateRequest request)
        //{
        //    var response = await _http.PostAsJsonAsync("/v1/products", request);
        //    response.EnsureSuccessStatusCode();
        //    return await ReadResponseAsync<ProductResponseData>(response);
        //}



        //public async Task<ImageUploadResponseData> AddProductImageAsync(string productCode, MultipartFormDataContent content)
        //{
        //    var response = await _http.PostAsync($"/v1/products/{productCode}/images", content);
        //    response.EnsureSuccessStatusCode();
        //    return await ReadResponseAsync<ImageUploadResponseData>(response);
        //}

        //public async Task<AvailabilityResponse> GetAvailabilityAsync(string productCode, DateTime fromDate, DateTime toDate)
        //{
        //    var url = $"/v1/availability?productCode={productCode}"
        //            + $"&fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
        //    var resp = await _http.GetAsync(url);
        //    resp.EnsureSuccessStatusCode();
        //    return await resp.Content.ReadFromJsonAsync<AvailabilityResponse>();
        //}

        //public async Task<AvailabilityData> CreateAvailabilityAsync(AvailabilityRequest request)
        //{
        //    var response = await _http.PostAsJsonAsync("/v1/availability", request);
        //    response.EnsureSuccessStatusCode();
        //    return await ReadResponseAsync<AvailabilityData>(response);
        //}

        //public async Task<AvailabilityResponse> UpdateAvailabilityAsync(int sessionId, AvailabilityRequest request)
        //{
        //    var resp = await _http.PutAsJsonAsync($"/v1/availability/{sessionId}", request);
        //    resp.EnsureSuccessStatusCode();
        //    return await resp.Content.ReadFromJsonAsync<AvailabilityResponse>();
        //}

        //public async Task<BookingResponseData> CreateBookingAsync(BookingCreateRequest request)
        //{
        //    var response = await _http.PostAsJsonAsync("/v1/bookings", request);
        //    response.EnsureSuccessStatusCode();
        //    return await ReadResponseAsync<BookingResponseData>(response);
        //}
    }
    // Assuming the 'Error' property of 'RequestStatus' is of a type that contains 'ErrorMessage' property

}
