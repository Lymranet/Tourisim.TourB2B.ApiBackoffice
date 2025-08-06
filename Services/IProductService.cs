using TourManagementApi.Models.Api;
using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public interface IProductService
    {
        List<RezdyProductDto> GetAll();
        ProductResponseModel GetByCode(string productCode);
    }
}
