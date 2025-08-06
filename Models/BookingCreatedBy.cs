using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingCreatedBy
{
    public long CreatedById { get; set; }

    public long? BookingId { get; set; }

    public string? Code { get; set; }

    public string? Email { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual Booking? Booking { get; set; }
}
