using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Availability
{
    public int Id { get; set; }

    public string PartnerSupplierId { get; set; } = null!;

    public string ActivityId { get; set; } = null!;

    public string OptionId { get; set; } = null!;

    public DateOnly Date { get; set; }

    public DateTimeOffset? StartTime { get; set; }

    public int AvailableCapacity { get; set; }

    public int MaximumCapacity { get; set; }

    public virtual ICollection<TicketCategoryCapacity> TicketCategoryCapacities { get; set; } = new List<TicketCategoryCapacity>();
}
