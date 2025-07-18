// Services/Rezdy/BookingService.cs
using Rezdy.Api.Models;
using System.Threading.Tasks;
using TourManagementApi.Models.Api;

namespace TourManagementApi.Services.Rezdy
{
    public class BookingService
    {
        private readonly IRezdyApiClient _client;

        public BookingService(IRezdyApiClient client)
        {
            _client = client;
        }

        public async Task<string> CreateBookingAsync(BookingCreateRequest req)
        {
            var resp = await _client.CreateBookingAsync(req);
            return resp.ResponseData.OrderNumber;
        }

        // Webhook’lardan gelen event’leri burada işleyin
        public Task OnBookingCreatedAsync(BookingDto booking)
        {
            // → DB’ye kaydet, e-posta gönder…
            return Task.CompletedTask;
        }

        public Task OnBookingUpdatedAsync(BookingDto booking)
        {
            // → Güncelle
            return Task.CompletedTask;
        }

        public Task OnBookingCancelledAsync(string orderNumber)
        {
            // → İptal işlemi
            return Task.CompletedTask;
        }
    }
}
