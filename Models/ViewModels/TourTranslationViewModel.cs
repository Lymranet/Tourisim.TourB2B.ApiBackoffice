namespace TourManagementApi.Models.ViewModels
{
    public class TourTranslationViewModel
    {
        public int ActivityId { get; set; }
        public string Title { get; set; } = "";
        public List<string> ExistingLanguages { get; set; } = new();
    }

    public class AddLanguageViewModel
    {
        public bool IsEditMode { get; set; }

        public int ActivityId { get; set; }

        public string LanguageCode { get; set; } = string.Empty;

        public ActivityTranslationDTO Original { get; set; } = new ActivityTranslationDTO();

        public ActivityTranslationDTO Translated { get; set; } = new ActivityTranslationDTO();
    }


    public class ActivityTranslationDTO
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;

        public string? Highlights { get; set; }

        public string? Itinerary { get; set; }

        public List<string> Inclusions { get; set; } = new List<string>();

        public List<string> Exclusions { get; set; } = new List<string>();

        public List<string> ImportantInfo { get; set; } = new List<string>();
    }
}
