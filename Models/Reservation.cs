namespace TourManagementApi.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public int OptionId { get; set; }
        public Option Option { get; set; }

        public DateTime ReservationDate { get; set; }
        public DateTime ScheduledDate { get; set; }

        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }

        public int GuestCount { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        public string Status { get; set; } // Confirmed, Cancelled vs.

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<ReservationGuest> Guests { get; set; }
    }

}
