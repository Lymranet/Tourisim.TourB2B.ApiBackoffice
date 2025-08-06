namespace TourManagementApi.Models.Api.ExperinceBank
{
    public class CancelBookingRequest
    {
        public CancelBookingData Data { get; set; }
    }

    public class CancelBookingData
    {
        public string Reason { get; set; }
        public string Note { get; set; }
    }

}
