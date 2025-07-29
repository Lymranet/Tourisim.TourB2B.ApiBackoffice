using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public interface IProductService
    {
        IEnumerable<ProductModel> GetAll();
        ProductModel GetByCode(string productCode);
    }

}
