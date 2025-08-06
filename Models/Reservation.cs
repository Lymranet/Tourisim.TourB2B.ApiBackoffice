using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Reservation
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public int OptionId { get; set; }

    public DateTime ReservationDate { get; set; }

    public DateTime ScheduledDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = null!;

    public int GuestCount { get; set; }

    public string ContactName { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string MarketplaceBookingId { get; set; } = null!;

    public string MarketplaceId { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public string PartnerBookingId { get; set; } = null!;

    public string PartnerSupplierId { get; set; } = null!;

    public bool IsCancelled { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancelReason { get; set; }

    public string? CancelNote { get; set; }

    public string? BookingId { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Option Option { get; set; } = null!;

    public virtual ICollection<ReservationGuest> ReservationGuests { get; set; } = new List<ReservationGuest>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
