// Services/Rezdy/RezdyService.cs
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TourManagementApi.Models;
using Microsoft.Extensions.Logging;

namespace TourManagementApi.Services.Rezdy
{
    public class RezdyService
    {
        private readonly IHttpClientFactory _factory;
        private readonly RezdySettings _settings;
        private readonly ILogger<RezdyService> _logger;

        public RezdyService(
            IHttpClientFactory factory,
            IOptions<RezdySettings> opts,
            ILogger<RezdyService> logger)
        {
            _factory = factory;
            _settings = opts.Value;
            _logger = logger;
        }

        // Ortak HTTP gönderim metodu
        private async Task<JObject> SendRequestAsync(HttpRequestMessage req)
        {
            var client = _factory.CreateClient();
            _logger.LogInformation("Rezdy Request: {Method} {Url} – Body: {Body}",
                req.Method, req.RequestUri, await req.Content.ReadAsStringAsync());

            var res = await client.SendAsync(req);
            var content = await res.Content.ReadAsStringAsync();
            _logger.LogInformation("Rezdy Response ({Status}): {Content}",
                res.StatusCode, content);

            if (!res.IsSuccessStatusCode)
                throw new ApplicationException($"Rezdy hatası: {content}");

            return JObject.Parse(content);
        }

        // 1️⃣ Ürün oluşturma
        public async Task<string> CreateProductAsync(object createRequest)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/products")
            {
                Content = new StringContent(JsonConvert.SerializeObject(createRequest), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", _settings.ApiKey);

            var json = await SendRequestAsync(req);
            return json["responseData"]?["productCode"]?.ToString();
        }

        // 2️⃣ Ürün görsel yükleme
        public async Task UploadProductImageAsync(string productCode, Stream imageStream, string filename)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(imageStream), "file", filename);

            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/products/{productCode}/images")
            {
                Content = content
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", _settings.ApiKey);

            await SendRequestAsync(req);
        }

        // 3️⃣ Rezervasyon oluşturma
        public async Task<string> CreateBookingAsync(object bookingRequest)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/bookings")
            {
                Content = new StringContent(JsonConvert.SerializeObject(bookingRequest), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", _settings.ApiKey);

            var json = await SendRequestAsync(req);
            return json["responseData"]?["orderNumber"]?.ToString();
        }

        // 4️⃣ Webhook abone/çıkar
        public async Task SubscribeWebhookAsync(string @event, string url)
        {
            var body = new
            {
                eventName = @event,
                url
            };
            var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/webhooks")
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", _settings.ApiKey);
            await SendRequestAsync(req);
        }

        public async Task UnsubscribeWebhookAsync(string webhookId)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"{_settings.BaseUrl}/v1/webhooks/{webhookId}");
            req.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", _settings.ApiKey);
            await SendRequestAsync(req);
        }
    }
}
