using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Addon
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public string Title { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PriceAmount { get; set; }

    public string Currency { get; set; } = null!;

    public virtual Activity Activity { get; set; } = null!;

    public virtual ICollection<AddonTranslation> AddonTranslations { get; set; } = new List<AddonTranslation>();
}
