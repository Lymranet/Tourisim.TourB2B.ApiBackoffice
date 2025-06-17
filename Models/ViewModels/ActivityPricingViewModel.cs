using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TourManagementApi.Models.Common;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityPricingViewModel
    {
        public int ActivityId { get; set; }

        // Genel fiyatlandırma ayarları
        [Required]
        public string DefaultCurrency { get; set; } = "TRY";
        public decimal? TaxRate { get; set; } = 18;

        // Fiyat kategorileri
        public List<PriceCategoryViewModel> Categories { get; set; } = new();

        // Servisler ve gereksinimler
        public List<string> Included { get; set; } = new();
        public List<string> Excluded { get; set; } = new();
        public List<string> Requirements { get; set; } = new();

        // Ek bilgi
        public string? CancellationPolicy { get; set; }
        public string? AdditionalNotes { get; set; }

        // Pricing özelliği
        public ActivityPricing? Pricing { get; set; }
    }

    public class PriceCategoryViewModel
    {
        [Required]
        public string Type { get; set; } = string.Empty; // Adult, Child, vb.
        [Required]
        public string PriceType { get; set; } = string.Empty; // Sabit, Değişken
        [Required]
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public int? MinAge { get; set; }
        public int? MaxAge { get; set; }
        public string? Description { get; set; }
        public int? MinParticipants { get; set; }
        public int? MaxParticipants { get; set; }
        public string? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }
} 