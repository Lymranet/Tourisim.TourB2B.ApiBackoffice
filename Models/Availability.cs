using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Availability
{
    public int Id { get; set; }

    public string PartnerSupplierId { get; set; } = null!;

    public int ActivityId { get; set; }

    public int OptionId { get; set; }

    public DateOnly Date { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public int AvailableCapacity { get; set; }

    public int MaximumCapacity { get; set; }

    public virtual Activity Activity { get; set; } = null!;

    public virtual Option Option { get; set; } = null!;

    public virtual ICollection<TicketCategoryCapacity> TicketCategoryCapacities { get; set; } = new List<TicketCategoryCapacity>();
}
