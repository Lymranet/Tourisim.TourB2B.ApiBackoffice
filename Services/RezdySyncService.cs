using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using TourManagementApi.Data;

namespace TourManagementApi.Services
{
    public class RezdySyncService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RezdySyncService(ApplicationDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task SyncActivitiesToRezdy()
        {
            var activities = _context.Activities
                .Include(a => a.Options)
                .Include(a => a.Availabilities)
                .Where(a => a.IsActive)
                .ToList();

            foreach (var activity in activities)
            {
                var rezdyPayload = new
                {
                    productCode = activity.Id.ToString(),
                    name = activity.Title,
                    description = activity.Description,
                    durationMinutes = activity.Duration,
                    status = activity.Status,
                    label = activity.Label,
                    supplierId = activity.PartnerSupplierId,
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://app.rezdy-staging.com/v1/products")
                {
                    Content = JsonContent.Create(rezdyPayload)
                };

                request.Headers.Add("apikey", _configuration["Rezdy:ApiKey"]);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task TriggerProductUpdateNotificationAsync(string rezdyProductCode, string externalProductCode)
        {
            var payload = new
            {
                productCode = rezdyProductCode,
                externalProductCode = externalProductCode,
                importFeatures = new
                {
                    basicDetails = true,
                    pricing = true,
                    availability = true,
                    images = true,
                    locations = true
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://app.rezdy-staging.com/rc/product/update?apiKey={_configuration["Rezdy:ApiKey"]}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                // TODO: logla
                throw new Exception($"Rezdy product update notification failed: {response.StatusCode} - {error}");
            }
        }

        public async Task TriggerAvailabilityUpdateNotificationAsync(string rezdyProductCode, string externalProductCode, DateTime fromUtc, DateTime toUtc)
        {
            var payload = new
            {
                productCode = rezdyProductCode,
                externalProductCode = externalProductCode,
                from = fromUtc.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                to = toUtc.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var request = new HttpRequestMessage(HttpMethod.Post, $"https://app.rezdy-staging.com/rc/availability/update?apiKey={_configuration["Rezdy:ApiKey"]}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                // Logla veya hata yönetimini tetikle
                throw new Exception($"Rezdy availability update notification failed: {response.StatusCode} - {error}");
            }
        }

    }


}
