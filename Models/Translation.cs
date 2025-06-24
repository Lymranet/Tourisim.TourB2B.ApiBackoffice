using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace TourManagementApi.Models
{
    public class Translation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityId { get; set; }

        [ForeignKey(nameof(ActivityId))]
        public Activity Activity { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Language { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string? Highlights { get; set; }

        public string? Itinerary { get; set; }

        public string InclusionsJson { get; set; } = "[]";

        [NotMapped]
        public List<string> Inclusions
        {
            get => JsonSerializer.Deserialize<List<string>>(InclusionsJson) ?? new();
            set => InclusionsJson = JsonSerializer.Serialize(value ?? new());
        }

        public string ExclusionsJson { get; set; } = "[]";

        [NotMapped]
        public List<string> Exclusions
        {
            get => JsonSerializer.Deserialize<List<string>>(ExclusionsJson) ?? new();
            set => ExclusionsJson = JsonSerializer.Serialize(value ?? new());
        }

        public string ImportantInfoJson { get; set; } = "[]";

        [NotMapped]
        public List<string> ImportantInfo
        {
            get => JsonSerializer.Deserialize<List<string>>(ImportantInfoJson) ?? new();
            set => ImportantInfoJson = JsonSerializer.Serialize(value ?? new());
        }
    }

}
