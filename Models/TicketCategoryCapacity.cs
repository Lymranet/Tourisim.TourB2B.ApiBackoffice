using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class TicketCategoryCapacity
{
    public int Id { get; set; }

    public int AvailabilityId { get; set; }

    public int TicketCategoryId { get; set; }

    public int? Capacity { get; set; }

    public virtual Availability Availability { get; set; } = null!;

    public virtual TicketCategory TicketCategory { get; set; } = null!;
}
