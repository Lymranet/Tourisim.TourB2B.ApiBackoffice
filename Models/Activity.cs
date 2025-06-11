using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Models.Common;

namespace TourManagementApi.Models;

public class Activity
{
    [Key]
    public int Id { get; set; }
    
    // Basic Information
    [Required(ErrorMessage = "Başlık zorunludur.")]
    [MaxLength(200)]
    public string Title { get; set; } = null!;
    
    [Required(ErrorMessage = "Açıklama zorunludur.")]
    public string Description { get; set; } = null!;
    
    [Required(ErrorMessage = "Kategori zorunludur.")]
    [MaxLength(100)]
    public string Category { get; set; } = null!;
    
    [Required(ErrorMessage = "Alt kategori zorunludur.")]
    [MaxLength(100)]
    public string? Subcategory { get; set; }
    
    [Required(ErrorMessage = "En az bir dil seçilmelidir.")]
    public List<string>? Languages { get; set; }
    
    // Location
    private Location? _location;
    public Location? Location 
    { 
        get => _location;
        set => _location = value?.IsEmpty == true ? null : value;
    }
    
    public List<MeetingPoint> MeetingPoints { get; set; } = new();
    
    // Timing
    public int Duration { get; set; }
    public List<TimeSlot> TimeSlots { get; set; } = new();
    public SeasonalAvailability? SeasonalAvailability { get; set; }
    
    // Pricing & Capacity
    public PriceInfo? PriceInfo { get; set; }
    public ActivityPricing? Pricing { get; set; }
    
    // Requirements & Includes
    public List<string> Requirements { get; set; } = new();
    public List<string> Included { get; set; } = new();
    public List<string> Excluded { get; set; } = new();
    public List<GuestField> GuestFields { get; set; } = new();
    
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

    public List<Addon> Addons { get; set; } = new();
}

[Owned]
public class MeetingPoint
{
    public string Name { get; set; } = null!;
    public string Latitude { get; set; } = null!;
    public string Longitude { get; set; } = null!;
    public string Address { get; set; } = null!;
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
    public string Code { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string Type { get; set; } = "text";
    public bool Required { get; set; }
    public List<GuestFieldOption> Options { get; set; } = new();
    public List<GuestFieldTranslation> Translations { get; set; } = new();
}

[Owned]
public class GuestFieldOption
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public List<GuestFieldOptionTranslation> Translations { get; set; } = new();
}

[Owned]
public class GuestFieldOptionTranslation
{
    public string Language { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string Label { get; set; } = null!;
}

[Owned]
public class GuestFieldTranslation
{
    public string Language { get; set; } = null!;
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