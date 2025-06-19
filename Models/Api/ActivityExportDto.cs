using TourManagementApi.Models.Common;

namespace TourManagementApi.Models.Api
{
    public class ActivityExportDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string CoverImage { get; set; }
        public string CountryCode { get; set; }
        public string DestinationCode { get; set; }
        public string DestinationName { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public List<TimeSlot>? TimeSlots { get; set; }
    }

}
