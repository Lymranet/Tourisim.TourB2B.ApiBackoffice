namespace TourManagementApi.Models.Api
{
    public class Availability
    {
        public int Id { get; set; }
        public string PartnerSupplierId { get; set; }
        public string ActivityId { get; set; }
        public string OptionId { get; set; }
        public DateTime Date { get; set; } // Tarih
        public DateTimeOffset? StartTime { get; set; } // Varsa saatli

        public int AvailableCapacity { get; set; }
        public int MaximumCapacity { get; set; }

        public List<OpeningHour> OpeningHours { get; set; }
        public List<TicketCategoryCapacity> TicketCategoryCapacities { get; set; }
    }
    public class OpeningHour
    {
        public string FromTime { get; set; } // "09:00:00"
        public string ToTime { get; set; }   // "20:30:00"
    }
    public class TicketCategoryCapacity
    {
        public string TicketCategoryId { get; set; }
        public int? Capacity { get; set; } // null ise sınırsız
    }

}
