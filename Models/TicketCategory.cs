using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class TicketCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int MinSeats { get; set; }

    public int MaxSeats { get; set; }

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string PriceType { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int? MinAge { get; set; }

    public int? MaxAge { get; set; }

    public int OptionId { get; set; }

    public virtual Option Option { get; set; } = null!;
}
