namespace TourManagementApi.Models.ReportModels
{
    public class TourReservationReportDto
    {
        public string TourTitle { get; set; }
        public int ReservationCount { get; set; }
        public int TotalGuests { get; set; }
    }
}
