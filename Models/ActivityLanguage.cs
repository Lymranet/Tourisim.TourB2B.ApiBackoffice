using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementApi.Models
{
    public class ActivityLanguage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public Activity Activity { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string LanguageCode { get; set; } = string.Empty; // Örn: "tr", "en", vs.
    }

}
