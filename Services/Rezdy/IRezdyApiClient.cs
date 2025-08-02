// Services/Rezdy/IRezdyApiClient.cs
using System;
using System.Threading.Tasks;
using TourManagementApi.Models.Api.Rezdy;

namespace TourManagementApi.Services.Rezdy
{
    public interface IRezdyApiClient
    {
        // 1. Ürünleri listele (paginated)
        Task<PaginatedResponse<ProductDetail>> GetProductsAsync(int offset = 0, int limit = 100);

        // 2. Belirli ürünün availability’si
        /// <summary>
        /// Cheks availability for one or more products over a local date range.
        /// </summary>
        /// <param name="productCodes">One or more Rezdy productCode values.</param>
        /// <param name="startLocal">Local start boundary (yyyy-MM-dd HH:mm:ss).</param>
        /// <param name="endLocal">Local end boundary (yyyy-MM-dd HH:mm:ss).</param>
        Task<AvailabilityResponse> GetAvailabilityAsync(
    IEnumerable<string> productCodes,
    DateTime startLocal,
    DateTime endLocal);

        // 3. Yeni session/availability oluştur
        Task<AvailabilityResponse> CreateAvailabilityAsync(AvailabilityRequest request);

        // 4. Mevcut session’ı güncelle
        Task<AvailabilityResponse> UpdateAvailabilityAsync(int sessionId, AvailabilityRequest request);

        // 5. Yeni ürün oluştur
        Task<ProductResponse> CreateProductAsync(ProductCreateRequest request);

        // 6. Ürün resmi ekle
        Task<ImageUploadResponse> AddProductImageAsync(string productCode, MultipartFormDataContent content);

        // 7. Rezervasyon oluştur
        /// <summary>
        /// Creates a new booking with manual/payment-recording.
        /// </summary>
        Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request);

        // 8. Rezervasyonu güncelle
        Task<BookingResponse> UpdateBookingAsync(string orderNumber, BookingUpdateRequest request);

        // 9. Rezervasyonu iptal et
        Task<CancelBookingResponse> CancelBookingAsync(string orderNumber);
    }
}
