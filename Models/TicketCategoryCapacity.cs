using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class TicketCategoryCapacity
{
    public int Id { get; set; }

    public int AvailabilityId { get; set; }

    public string TicketCategoryId { get; set; } = null!;

    public int? Capacity { get; set; }

    public virtual Availability Availability { get; set; } = null!;
}
