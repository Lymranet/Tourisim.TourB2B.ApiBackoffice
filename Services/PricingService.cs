using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public class PricingService
    {
        public List<ProductPricing> Get(string productCode)
        {
            return new List<ProductPricing>
        {
            new ProductPricing { Currency = "EUR", Amount = 150 }
        };
        }
    }

}
