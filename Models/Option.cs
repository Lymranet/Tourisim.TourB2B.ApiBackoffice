using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Option
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public string Name { get; set; } = null!;

    public string StartTime { get; set; } = null!;

    public int CutOff { get; set; }

    public string Weekdays { get; set; } = null!;

    public bool CanBeBookedAfterStartTime { get; set; }

    public string Duration { get; set; } = null!;

    public DateTime FromDate { get; set; }

    public DateTime UntilDate { get; set; }

    public string? EndTime { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual ICollection<OpeningHour> OpeningHours { get; set; } = new List<OpeningHour>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<TicketCategory> TicketCategories { get; set; } = new List<TicketCategory>();
}
