using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityAddonsViewModel
    {
        public int ActivityId { get; set; }
        public List<AddonViewModel> Addons { get; set; } = new();
    }

    public class AddonViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty; // guest, group, vb.
        public string? Description { get; set; }
        public List<AddonTranslationViewModel> Translations { get; set; } = new();
        public decimal? PriceAmount { get; set; }
        public string Currency { get; set; } = "TRY";
    }


    public class AddonTranslationViewModel
    {
        [Required]
        public string Language { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
} 