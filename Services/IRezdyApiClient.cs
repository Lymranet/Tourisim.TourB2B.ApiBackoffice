// Services/Rezdy/IRezdyApiClient.cs
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Rezdy.Api.Models;

namespace TourManagementApi.Services.Rezdy
{
    public interface IRezdyApiClient
    {
        Task<ProductResponse> CreateProductAsync(ProductCreateRequest request);
        Task<ImageUploadResponse> AddProductImageAsync(string productCode, MultipartFormDataContent content);
        Task<AvailabilityResponse> GetAvailabilityAsync(string productCode, DateTime fromDate, DateTime toDate);
        Task<AvailabilityResponse> CreateAvailabilityAsync(AvailabilityRequest request);
        Task<AvailabilityResponse> UpdateAvailabilityAsync(int sessionId, AvailabilityRequest request);
        Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request);
    }
}
