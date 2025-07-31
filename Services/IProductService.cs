using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public interface IProductService
    {
        List<ProductResponseModel> GetAll();
        ProductResponseModel GetByCode(string productCode);
    }

}
