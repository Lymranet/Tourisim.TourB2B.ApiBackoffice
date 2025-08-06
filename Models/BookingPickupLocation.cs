using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingPickupLocation
{
    public long PickupLocationId { get; set; }

    public long? ItemId { get; set; }

    public string? LocationName { get; set; }

    public string? Address { get; set; }

    public long? Latitude { get; set; }

    public long? Longitude { get; set; }

    public int? MinutesPrior { get; set; }

    public string? PickupInstructions { get; set; }

    public string? PickupTime { get; set; }

    public virtual BookingItem? Item { get; set; }
}
