using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingExtra
{
    public long ExtraId { get; set; }

    public long? ItemId { get; set; }

    public string? Name { get; set; }

    public string? ExtraPriceType { get; set; }

    public int? Price { get; set; }

    public int? Quantity { get; set; }

    public string? Description { get; set; }

    public virtual BookingItem? Item { get; set; }
}
