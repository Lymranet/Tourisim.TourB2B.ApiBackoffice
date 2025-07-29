using Microsoft.EntityFrameworkCore;
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
                    productCode = activity.Id.ToString(), // veya özel code alanı
                    name = activity.Title,
                    description = activity.Description,
                    durationMinutes = activity.Duration,
                    status = activity.Status,
                    label = activity.Label,
                    supplierId = activity.PartnerSupplierId,
                    // diğer alanlar da burada hazırlanabilir
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.rezdy.com/v1/products")
                {
                    Content = JsonContent.Create(rezdyPayload)
                };

                request.Headers.Add("apikey", _configuration["Rezdy:ApiKey"]);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }

}
