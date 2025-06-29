using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int Duration { get; set; }

    public string? ContactInfoName { get; set; }

    public string? ContactInfoEmail { get; set; }

    public string? ContactInfoPhone { get; set; }

    public string? ContactInfoRole { get; set; }

    public string Status { get; set; } = null!;

    public double Rating { get; set; }

    public string? MediaVideos { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool? ContactInfoIsNull { get; set; }

    public string CountryCode { get; set; } = null!;

    public string DestinationCode { get; set; } = null!;

    public string DestinationName { get; set; } = null!;

    public string Exclusions { get; set; } = null!;

    public string? Highlights { get; set; }

    public string ImportantInfo { get; set; } = null!;

    public string Inclusions { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Itinerary { get; set; }

    public int? TotalRatingCount { get; set; }

    public string? CoverImage { get; set; }

    public string? GalleryImages { get; set; }

    public string? PreviewImage { get; set; }

    public string ExclusionsJson { get; set; } = null!;

    public string GuestFieldsJson { get; set; } = null!;

    public string ImportantInfoJson { get; set; } = null!;

    public string InclusionsJson { get; set; } = null!;

    public string GuestFields { get; set; } = null!;

    public string? DetailsUrl { get; set; }

    public string? PartnerSupplierId { get; set; }

    public string Label { get; set; } = null!;

    public bool? IsFreeCancellation { get; set; }

    public int? TourCompanyId { get; set; }

    public virtual ICollection<ActivityLanguage> ActivityLanguages { get; set; } = new List<ActivityLanguage>();

    public virtual ICollection<Addon> Addons { get; set; } = new List<Addon>();

    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    public virtual ICollection<CancellationPolicyCondition> CancellationPolicyConditions { get; set; } = new List<CancellationPolicyCondition>();

    public virtual ICollection<MeetingPoint> MeetingPoints { get; set; } = new List<MeetingPoint>();

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual ICollection<PriceCategory> PriceCategories { get; set; } = new List<PriceCategory>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    public virtual TourCompany? TourCompany { get; set; }

    public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
}
