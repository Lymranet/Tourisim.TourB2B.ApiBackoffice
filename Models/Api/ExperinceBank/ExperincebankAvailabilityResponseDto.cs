namespace TourManagementApi.Models.Api.ExperinceBank
{
    public class ExperincebankAvailabilityResponseDto
    {
        public string dateTime { get; set; }
        public List<OpeningHourDto> openingHours { get; set; } = new();
        public int availableCapacity { get; set; }
        public int maximumCapacity { get; set; }
        public string activityId { get; set; }
        public string optionId { get; set; }
        public List<TicketCategoryDto> ticketCategories { get; set; } = new();
    }

    public class OpeningHourDto
    {
        public string fromTime { get; set; }
        public string toTime { get; set; }
    }

    public class TicketCategoryDto
    {
        public string id { get; set; }
        public int? availableCapacity { get; set; }
    }

    public class AvailabilityResultDto
    {
        public LinksDto links { get; set; }
        public List<ExperincebankAvailabilityResponseDto> data { get; set; }
    }

    public class LinksDto
    {
        public string? next { get; set; }
    }

}
