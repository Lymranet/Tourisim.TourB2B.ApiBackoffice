using System.Net.Sockets;

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

        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // CM entegrasyonu için
        public string ExperienceBankBookingId { get; set; }
        public string MarketplaceId { get; set; }
        public string MarketplaceBookingId { get; set; }
        public string Notes { get; set; }

        public ICollection<ReservationGuest> Guests { get; set; }
        public string PartnerSupplierId { get; internal set; }
        public string PartnerBookingId { get; internal set; }
        public List<Ticket> Tickets { get; set; }
    }
}
