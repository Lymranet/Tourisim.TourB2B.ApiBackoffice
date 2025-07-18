// Services/Rezdy/ProductService.cs
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Rezdy.Api.Models;

namespace TourManagementApi.Services.Rezdy
{
    public class ProductService
    {
        private readonly IRezdyApiClient _client;

        public ProductService(IRezdyApiClient client)
        {
            _client = client;
        }

        public async Task<string> PublishProductAsync(DomainProduct domain)
        {
            // 1️⃣ Domain → Rezdy CreateRequest’e çevirin
            var req = new ProductCreateRequest
            {
                Name = domain.Name,
                Description = domain.Description,
                PriceOptions = new[] {
                    new PriceOption { Price = domain.Price, PriceType = "PER_PERSON" }
                },
                Locations = new[] { new Location { Code = domain.LocationCode } }
                // ... diğer gerekli alanlar
            };

            var resp = await _client.CreateProductAsync(req);
            return resp.ResponseData.ProductCode;
        }

        public async Task UploadImageAsync(string productCode, Stream imageStream, string filename)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(imageStream), "file", filename);
            await _client.AddProductImageAsync(productCode, content);
        }

        public async Task SyncAvailabilityAsync(string productCode, DateTime date, decimal price, int capacity)
        {
            var session = new Session
            {
                Date = date,
                Price = price,
                Quantity = capacity
            };
            var req = new AvailabilityRequest
            {
                ProductCode = productCode,
                Sessions = new[] { session }
            };
            await _client.CreateAvailabilityAsync(req);
        }
    }

    // Domain tarafı için basit bir model
    public class DomainProduct
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string LocationCode { get; set; }
    }
}
