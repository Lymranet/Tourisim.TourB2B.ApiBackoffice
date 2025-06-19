namespace TourManagementApi.Models.ReportModels
{
    public class DailyOperationalItemDto
    {
        public string TourTitle { get; set; }
        public string OptionName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int GuestCount { get; set; }
        public List<string> GuestNames { get; set; }
    }

}
