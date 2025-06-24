using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class PriceCategory
{
    public int ActivityPricingActivityId { get; set; }

    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string PriceType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Currency { get; set; } = null!;

    public string? Description { get; set; }

    public int? MinParticipants { get; set; }

    public int? MaxParticipants { get; set; }

    public string? DiscountType { get; set; }

    public decimal? DiscountValue { get; set; }

    public int? MaxAge { get; set; }

    public int? MinAge { get; set; }

    public virtual Activity ActivityPricingActivity { get; set; } = null!;
}
