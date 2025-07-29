using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Services
{
    public class AvailabilityService
    {
        public List<AvailabilityResponse> Get(AvailabilityRequest request)
        {
            return new List<AvailabilityResponse>
        {
            new AvailabilityResponse
            {
                ProductCode = request.ProductCode,
                StartTimeLocal = request.StartTimeLocal,
                RemainingPlaces = 10
            }
        };
        }
    }

}
