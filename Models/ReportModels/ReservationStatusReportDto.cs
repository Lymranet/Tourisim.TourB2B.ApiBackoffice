namespace TourManagementApi.Models.ReportModels
{
    public class ReservationStatusReportDto
    {
        public string Status { get; set; }
        public int ReservationCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
