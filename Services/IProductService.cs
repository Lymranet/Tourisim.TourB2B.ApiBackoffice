using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public interface IProductService
    {
        bool Exists(string productCode, string externalProductCode);
        List<RezdyProductDto> GetAll();
        ProductResponseModel GetByCode(string productCode);
    }
}
