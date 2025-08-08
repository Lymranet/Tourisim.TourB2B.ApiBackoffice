namespace TourManagementApi.Models.ViewModels
{
    public partial class ActivityPreviewViewModel
    {
        public int ActivityId { get; set; }
        public List<string> MissingFields { get; set; } = new();
    }

    public partial class ActivityPreviewViewModel
    {
        // Ana bilgiler
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public int Duration { get; set; }
        public string Status { get; set; } = "";
        public double Rating { get; set; }
        public bool? IsFreeCancellation { get; set; }
        public string Label { get; set; } = "";

        // Lokasyon
        public string CountryCode { get; set; } = "";
        public string DestinationCode { get; set; } = "";
        public string DestinationName { get; set; } = "";

        // Medya
        public string? CoverImage { get; set; }
        public List<string> GalleryImages { get; set; } = new();
        public string? PreviewImage { get; set; }
        public string? MediaVideos { get; set; }

        // İletişim
        public string? ContactInfoName { get; set; }
        public string? ContactInfoEmail { get; set; }
        public string? ContactInfoPhone { get; set; }
        public string? ContactInfoRole { get; set; }

        // Ek bilgiler
        public string Exclusions { get; set; } = "";
        public string Inclusions { get; set; } = "";
        public string ImportantInfo { get; set; } = "";
        public string? Highlights { get; set; }
        public string? Itinerary { get; set; }
        public string? DetailsUrl { get; set; }

        // JSON alanlar
        public string ExclusionsJson { get; set; } = "";
        public string InclusionsJson { get; set; } = "";
        public string ImportantInfoJson { get; set; } = "";
        public string GuestFieldsJson { get; set; } = "";
        public string GuestFields { get; set; } = "";

        // Tarihler
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // İlişkili Veriler
        public List<Option> Options { get; set; } = new();
        public List<Addon> Addons { get; set; } = new();
        public List<MeetingPoint> MeetingPoints { get; set; } = new();
        public List<RoutePoint> RoutePoints { get; set; } = new();
        public List<Translation> Translations { get; set; } = new();
        public List<ActivityLanguage> ActivityLanguages { get; set; } = new();
        public List<CancellationPolicyCondition> CancellationPolicies { get; set; } = new();
        public List<Availability> Availabilities { get; set; } = new();

        // Tur şirketi
        public string? TourCompanyName { get; set; }

        // Diğer
        public string? PartnerSupplierId { get; set; }
        public string? B2BAgencyId { get; set; }
    }


}
