using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public class ProductService : IProductService
    {
        private static readonly List<ProductModel> _products = new()
    {
        new ProductModel
        {
            ProductCode = "KAPADOKYA-BALON",
            Name = "Kapadokya Balon Turu",
            Description = "Muhteşem bir balon deneyimi!",
            DurationMinutes = 120,
            Location = new ProductLocation
            {
                Address = "Göreme, Nevşehir",
                Geo = new GeoCoordinates { Lat = 38.643, Lon = 34.830 }
            },
            Images = new List<ProductImage>
            {
                new ProductImage { Url = "https://cdn.orneksite.com/balon.jpg", IsThumbnail = true }
            },
            Options = new List<ProductOption>
            {
                new ProductOption
                {
                    Code = "SABAH",
                    Name = "Sabah Turu",
                    Pricing = new List<ProductPricing>
                    {
                        new ProductPricing { Currency = "EUR", Amount = 150 }
                    }
                }
            }
        }
    };

        public IEnumerable<ProductModel> GetAll() => _products;

        public ProductModel GetByCode(string productCode) =>
            _products.FirstOrDefault(p => p.ProductCode == productCode);
    }

}
