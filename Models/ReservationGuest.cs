using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class ReservationGuest
{
    public int Id { get; set; }

    public int ReservationId { get; set; }

    public string GuestName { get; set; } = null!;

    public string GuestType { get; set; } = null!;

    public int Age { get; set; }

    public string AdditionalFieldsJson { get; set; } = null!;

    public string AddonsJson { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int Occupancy { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string TicketCategory { get; set; } = null!;

    public string TicketId { get; set; } = null!;

    public virtual Reservation Reservation { get; set; } = null!;
}
