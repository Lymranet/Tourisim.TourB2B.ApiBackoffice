using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Models.Common;
using System.Text.Json;
using TourManagementApi.Models.ViewModels;

namespace TourManagementApi.Models;

public class Activity
{
    [Key]
    public int Id { get; set; }
    
    // Basic Information
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    public string Subcategory { get; set; } = string.Empty;
    
    private string _languagesJson = "[]";
    [NotMapped]
    public List<string> Languages
    {
        get => JsonSerializer.Deserialize<List<string>>(_languagesJson) ?? new List<string>();
        set => _languagesJson = JsonSerializer.Serialize(value ?? new List<string>());
    }
    
    // Location
    private Location? _location;
    public Location? Location 
    { 
        get => _location;
        set => _location = value?.IsEmpty == true ? null : value;
    }
    
    public List<MeetingPoint> MeetingPoints { get; set; } = new List<MeetingPoint>();
    public List<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();
    
    // Timing
    public int Duration { get; set; }
    
    private string _timeSlotsJson = "[]";
    [NotMapped]
    public List<TimeSlot> TimeSlots
    {
        get => JsonSerializer.Deserialize<List<TimeSlot>>(_timeSlotsJson) ?? new List<TimeSlot>();
        set => _timeSlotsJson = JsonSerializer.Serialize(value ?? new List<TimeSlot>());
    }
    
    public SeasonalAvailability? SeasonalAvailability { get; set; }
    
    // Pricing & Capacity
    public PriceInfo? PriceInfo { get; set; }
    public ActivityPricing? Pricing { get; set; }
    
    // Requirements & Includes
    private string _requirementsJson = "[]";
    [NotMapped]
    public List<string> Requirements
    {
        get => JsonSerializer.Deserialize<List<string>>(_requirementsJson) ?? new List<string>();
        set => _requirementsJson = JsonSerializer.Serialize(value ?? new List<string>());
    }
    
    private string _includedJson = "[]";
    [NotMapped]
    public List<string> Included
    {
        get => JsonSerializer.Deserialize<List<string>>(_includedJson) ?? new List<string>();
        set => _includedJson = JsonSerializer.Serialize(value ?? new List<string>());
    }
    
    private string _exclusionsJson = "[]";
    [Column("ExclusionsJson")]
    public string ExclusionsJson 
    { 
        get => _exclusionsJson;
        set => _exclusionsJson = string.IsNullOrEmpty(value) ? "[]" : value;
    }

    [NotMapped]
    public List<string> Exclusions
    {
        get => JsonSerializer.Deserialize<List<string>>(ExclusionsJson) ?? new List<string>();
        set => ExclusionsJson = JsonSerializer.Serialize(value ?? new List<string>());
    }

    private string _inclusionsJson = "[]";
    [Column("InclusionsJson")]
    public string InclusionsJson 
    { 
        get => _inclusionsJson;
        set => _inclusionsJson = string.IsNullOrEmpty(value) ? "[]" : value;
    }

    [NotMapped]
    public List<string> Inclusions
    {
        get => JsonSerializer.Deserialize<List<string>>(InclusionsJson) ?? new List<string>();
        set => InclusionsJson = JsonSerializer.Serialize(value ?? new List<string>());
    }

    private string _importantInfoJson = "[]";
    [Column("ImportantInfoJson")]
    public string ImportantInfoJson 
    { 
        get => _importantInfoJson;
        set => _importantInfoJson = string.IsNullOrEmpty(value) ? "[]" : value;
    }

    [NotMapped]
    public List<string> ImportantInfo
    {
        get => JsonSerializer.Deserialize<List<string>>(ImportantInfoJson) ?? new List<string>();
        set => ImportantInfoJson = JsonSerializer.Serialize(value ?? new List<string>());
    }

    private string _guestFieldsJson = "[]";
    [Column("GuestFieldsJson")]
    public string GuestFieldsJson 
    { 
        get => _guestFieldsJson;
        set => _guestFieldsJson = string.IsNullOrEmpty(value) ? "[]" : value;
    }

    [NotMapped]
    public List<GuestField> GuestFields
    {
        get => JsonSerializer.Deserialize<List<GuestField>>(GuestFieldsJson) ?? new List<GuestField>();
        set => GuestFieldsJson = JsonSerializer.Serialize(value ?? new List<GuestField>());
    }
    
    // Contact & Additional Info
    public ContactInfo? ContactInfo { get; set; }
    
    public string? CancellationPolicy { get; set; }
    public string? AdditionalNotes { get; set; }
    
    // Metadata
    [Required(ErrorMessage = "Durum seçilmelidir.")]
    [MaxLength(50)]
    public string Status { get; set; } = "draft";
    public double Rating { get; set; }
    public Media? Media { get; set; }
    public SalesAvailability? SalesAvailability { get; set; }
    
    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    private string _addonsJson = "[]";
    [NotMapped]
    public List<Addon> Addons
    {
        get => JsonSerializer.Deserialize<List<Addon>>(_addonsJson) ?? new List<Addon>();
        set => _addonsJson = JsonSerializer.Serialize(value ?? new List<Addon>());
    }

    public ICollection<Option> Options { get; set; } = new List<Option>();

    // Trekksoft uyumlu yeni alanlar
    [Required(ErrorMessage = "Dil alanı zorunludur.")]
    public string Language { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Etiket alanı zorunludur.")]
    public string Label { get; set; } = string.Empty;
    public string? Highlights { get; set; }
    public string? Itinerary { get; set; }
    
    // Destination bilgileri
    public string CountryCode { get; set; } = string.Empty;
    public string DestinationCode { get; set; } = string.Empty;
    public string DestinationName { get; set; } = string.Empty;
    
    // Rating bilgileri
    public decimal? AverageRating { get; set; }
    public int? TotalRatingCount { get; set; }
    
    private string _categoriesJson = "[]";
    [NotMapped]
    public List<string> Categories
    {
        get => JsonSerializer.Deserialize<List<string>>(_categoriesJson) ?? new List<string>();
        set => _categoriesJson = JsonSerializer.Serialize(value ?? new List<string>());
    }
    
    public bool IsActive { get; set; }
    
    // İptal politikası
    public bool IsFreeCancellation { get; set; }
    
    private string _refundConditionsJson = "[]";
    [NotMapped]
    public List<RefundCondition> RefundConditions
    {
        get => JsonSerializer.Deserialize<List<RefundCondition>>(_refundConditionsJson) ?? new List<RefundCondition>();
        set => _refundConditionsJson = JsonSerializer.Serialize(value ?? new List<RefundCondition>());
    }

    // Medya alanları
    public string? CoverImage { get; set; }
    public string? PreviewImage { get; set; }

    [Column("GalleryImages")]
    public string? GalleryImagesJson
    {
        get => JsonSerializer.Serialize(GalleryImages ?? []);
        set => GalleryImages = JsonSerializer.Deserialize<List<string>>(value ?? "[]") ?? [];
    }



    [NotMapped]
    public List<string> GalleryImages { get; set; } = new();

    private string _videoUrlsJson = "[]";
    [NotMapped]
    public List<string> VideoUrls
    {
        get => JsonSerializer.Deserialize<List<string>>(_videoUrlsJson) ?? new List<string>();
        set => _videoUrlsJson = JsonSerializer.Serialize(value ?? new List<string>());
    }
    public string PartnerSupplierId { get; internal set; }
    public string DetailsUrl { get; internal set; }
    public ICollection<ActivityLanguage> ActivityLanguages { get; set; } = new List<ActivityLanguage>();

}

[Owned]
public class MeetingPoint
{
    [Required(ErrorMessage = "Buluşma noktası adı zorunludur")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enlem bilgisi zorunludur")]
    public string Latitude { get; set; } = string.Empty;

    [Required(ErrorMessage = "Boylam bilgisi zorunludur")]
    public string Longitude { get; set; } = string.Empty;

    [Required(ErrorMessage = "Adres bilgisi zorunludur")]
    public string Address { get; set; } = string.Empty;
}

[Owned]
public class RoutePoint
{
    [Required(ErrorMessage = "Rota noktası adı zorunludur")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enlem bilgisi zorunludur")]
    public string Latitude { get; set; } = string.Empty;

    [Required(ErrorMessage = "Boylam bilgisi zorunludur")]
    public string Longitude { get; set; } = string.Empty;
}

[Owned]
public class SeasonalAvailability
{
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

[Owned]
public class GuestField
{
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public bool Required { get; set; }
    public List<GuestFieldOption> Options { get; set; } = new();
}

[Owned]
public class GuestFieldOption
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

[Owned]
public class GuestFieldTranslation
{
    public string Language { get; set; } = null!;
    public string Label { get; set; } = null!;
}

[Owned]
public class GuestFieldOptionTranslation
{
    public string Language { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Label { get; set; } = null!;
}

[Owned]
public class SalesAvailability
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ContactInfo
{
    [Required(ErrorMessage = "İletişim ismi zorunludur.")]
    public string? Name { get; set; }

    [Required(ErrorMessage = "İletişim rolü zorunludur.")]
    public string? Role { get; set; }

    [Required(ErrorMessage = "İletişim e-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "İletişim telefon zorunludur.")]
    [MinLength(10, ErrorMessage = "Telefon en az 10 karakter olmalıdır.")]
    public string? Phone { get; set; }
}

[Owned]
public class Addon
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AddonPrice? Price { get; set; }
    public List<AddonTranslation> Translations { get; set; } = new();
}

[Owned]
public class AddonPrice
{
    public string Amount { get; set; } = "0";
    public string Currency { get; set; } = "TRY";
}

[Owned]
public class AddonTranslation
{
    public string Language { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

[Owned]
public class RefundCondition
{
    public int Id { get; set; }
    public int MinDurationBeforeStartTimeSec { get; set; }
    public int RefundPercentage { get; set; }
}

