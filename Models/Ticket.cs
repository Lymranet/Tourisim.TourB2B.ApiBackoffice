using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public string ExperienceBankTicketId { get; set; } = null!;

    public string InternalTicketId { get; set; } = null!;

    public string TicketCode { get; set; } = null!;

    public string TicketCodeType { get; set; } = null!;

    public int? ReservationId { get; set; }

    public virtual Reservation? Reservation { get; set; }
    public string Status { get; internal set; }
}
