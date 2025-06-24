using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Models.Common;

public class PriceInfo
{
    public string Currency { get; set; } = null!;
    public decimal BasePrice { get; set; }
    public int? MinimumParticipants { get; set; }
    public int? MaximumParticipants { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
}

[Owned]
public class ActivityPricing
{
    public List<PriceCategory> Categories { get; set; } = new();
    public string DefaultCurrency { get; set; } = null!;
    public bool TaxIncluded { get; set; } = true;
    public decimal? TaxRate { get; set; }
}

//[Owned]
//public class PriceCategory
//{
//    public string Type { get; set; } = null!;
//    public string PriceType { get; set; } = null!; // fixed or variable
//    public decimal Amount { get; set; }
//    public string Currency { get; set; } = null!;
//    public string? Description { get; set; }
//    public int? MinParticipants { get; set; }
//    public int? MaxParticipants { get; set; }
//    public string? DiscountType { get; set; } // percentage or fixed
//    public decimal? DiscountValue { get; set; }
//    public int? MinAge { get; set; }
//    public int? MaxAge { get; set; }
//} 