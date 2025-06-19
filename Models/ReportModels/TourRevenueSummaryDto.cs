namespace TourManagementApi.Models.ReportModels
{
    public class TourRevenueSummaryDto
    {
        public string TourTitle { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalGuests { get; set; }
    }

}
