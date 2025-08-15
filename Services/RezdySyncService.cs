using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using TourManagementApi.Data;

namespace TourManagementApi.Services
{
    //public class RezdySyncService
    //{
    //    private readonly TourManagementDbContext _context;
    //    private readonly HttpClient _httpClient;
    //    private readonly IConfiguration _configuration;

    //    public RezdySyncService(TourManagementDbContext context, HttpClient httpClient, IConfiguration configuration)
    //    {
    //        _context = context;
    //        _httpClient = httpClient;
    //        _configuration = configuration;
    //    }

    //    public async Task SyncActivitiesToRezdy()
    //    {
    //        var activities = _context.Activities
    //            .Include(a => a.Options)
    //            .Include(a => a.Availabilities)
    //            .Where(a => a.IsActive)
    //            .ToList();

    //        foreach (var activity in activities)
    //        {
    //            var rezdyPayload = new
    //            {
    //                productCode = activity.Id.ToString(),
    //                name = activity.Title,
    //                description = activity.Description,
    //                durationMinutes = activity.Duration,
    //                status = activity.Status,
    //                label = activity.Label,
    //                supplierId = activity.PartnerSupplierId,
    //            };

    //            var request = new HttpRequestMessage(HttpMethod.Post, "https://app.rezdy-staging.com/v1/products")
    //            {
    //                Content = JsonContent.Create(rezdyPayload)
    //            };

    //            request.Headers.Add("apikey", _configuration["Rezdy:ApiKey"]);

    //            var response = await _httpClient.SendAsync(request);
    //            response.EnsureSuccessStatusCode();
    //        }
    //    }

    //    public async Task TriggerProductUpdateNotificationAsync(string rezdyProductCode, string externalProductCode)
    //    {
    //        var payload = new
    //        {
    //            productCode = rezdyProductCode,
    //            externalProductCode = externalProductCode,
    //            importFeatures = new
    //            {
    //                basicDetails = true,
    //                pricing = true,
    //                availability = true,
    //                images = true,
    //                locations = true
    //            }
    //        };

    //        var request = new HttpRequestMessage(HttpMethod.Post, $" https://connect.rezdy.com/notifications/product?apiKey={_configuration["Rezdy:ApiKey"]}")
    //        {
    //            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
    //        };

    //        var response = await _httpClient.SendAsync(request);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            var error = await response.Content.ReadAsStringAsync();
    //            // TODO: logla
    //            throw new Exception($"Rezdy product update notification failed: {response.StatusCode} - {error}");
    //        }
    //    }

    //    public async Task TriggerAvailabilityUpdateNotificationAsync(string rezdyProductCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
    //    {
    //        var payload = new
    //        {
    //            productCode = rezdyProductCode,
    //            externalProductCode = externalProductCode,
    //            from = fromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
    //            to = toUtc.ToString("yyyy-MM-ddTHH:mm:ssZ")
    //        };

    //        var request = new HttpRequestMessage(HttpMethod.Post, $" https://connect.rezdy.com/notifications/product?apiKey={_configuration["Rezdy:ApiKey"]}")
    //        {
    //            Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
    //        };

    //        var response = await _httpClient.SendAsync(request);

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            var error = await response.Content.ReadAsStringAsync();
    //            // Logla veya hata yönetimini tetikle
    //            throw new Exception($"Rezdy availability update notification failed: {response.StatusCode} - {error}");
    //        }
    //    }

    //}

    public class RezdySyncService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;

        public RezdySyncService(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;
        }

        // REZDY'YE "PRODUCT UPDATE" TETİKLE
        public async Task TriggerProductUpdateAsync(string rezdyProductCode, string externalProductCode,
                                                    bool desc = true, bool price = true, bool images = true,
                                                    bool extras = true, bool bookingInfo = true, bool pickups = true)
        {
            var payload = new
            {
                productCode = rezdyProductCode,              // optional ama önerilir
                externalProductCode = externalProductCode,   // optional ama önerilir
                importFeatures = new
                {
                    description = desc,
                    price = price,
                    images = images,
                    extras = extras,
                    bookingInfo = bookingInfo,
                    pickups = pickups
                }
            };

            var url = $"https://connect.rezdy.com/notifications/product?apiKey={_cfg["Rezdy:ApiKey"]}";
            using var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync();
                throw new Exception($"Rezdy product update notification failed: {(int)res.StatusCode} - {err}");
            }
            // 204 döner (body yok)
        }

        // REZDY'YE "AVAILABILITY UPDATE" TETİKLE
        public async Task TriggerAvailabilityUpdateAsync(string rezdyProductCode, string externalProductCode,
                                                         DateTime fromUtc, DateTime? toUtc = null)
        {
            var payload = new
            {
                productCode = rezdyProductCode,            // Rezdy ProductCode varsa gönder
                externalProductCode = externalProductCode, // dış sistem kodun
                from = fromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                to = (toUtc ?? fromUtc).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var url = $"https://connect.rezdy.com/notifications/availability?apiKey={_cfg["Rezdy:ApiKey"]}";
            using var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            var res = await _http.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                var err = await res.Content.ReadAsStringAsync();
                throw new Exception($"Rezdy availability update notification failed: {(int)res.StatusCode} - {err}");
            }
            // 204 döner
        }
    }
}
