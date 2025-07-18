// Services/Rezdy/RezdyApiClient.cs
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Rezdy.Api.Models;

namespace TourManagementApi.Services.Rezdy
{
    public class RezdyApiClient : IRezdyApiClient
    {
        private readonly HttpClient _http;

        public RezdyApiClient(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<ProductResponse> CreateProductAsync(ProductCreateRequest request)
        {
            var resp = await _http.PostAsJsonAsync("/v1/products", request);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ProductResponse>();
        }

        public async Task<ImageUploadResponse> AddProductImageAsync(string productCode, MultipartFormDataContent content)
        {
            var resp = await _http.PostAsync($"/v1/products/{productCode}/images", content);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<ImageUploadResponse>();
        }

        public async Task<AvailabilityResponse> GetAvailabilityAsync(string productCode, DateTime fromDate, DateTime toDate)
        {
            var url = $"/v1/availability?productCode={productCode}"
                    + $"&fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}";
            var resp = await _http.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AvailabilityResponse>();
        }

        public async Task<AvailabilityResponse> CreateAvailabilityAsync(AvailabilityRequest request)
        {
            var resp = await _http.PostAsJsonAsync("/v1/availability", request);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AvailabilityResponse>();
        }

        public async Task<AvailabilityResponse> UpdateAvailabilityAsync(int sessionId, AvailabilityRequest request)
        {
            var resp = await _http.PutAsJsonAsync($"/v1/availability/{sessionId}", request);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<AvailabilityResponse>();
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request)
        {
            var resp = await _http.PostAsJsonAsync("/v1/bookings", request);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<BookingResponse>();
        }
    }
}
