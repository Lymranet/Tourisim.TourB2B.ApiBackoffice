using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingField
{
    public long FieldId { get; set; }

    public long? BookingId { get; set; }

    public string? Label { get; set; }

    public string? Value { get; set; }

    public string? BarcodeType { get; set; }

    public virtual Booking? Booking { get; set; }
}
