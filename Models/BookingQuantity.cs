using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingQuantity
{
    public long QuantityId { get; set; }

    public long? ItemId { get; set; }

    public string? OptionLabel { get; set; }

    public int? Value { get; set; }

    public double? OptionPrice { get; set; }

    public int? OptionSeatsUsed { get; set; }

    public virtual BookingItem? Item { get; set; }
}
