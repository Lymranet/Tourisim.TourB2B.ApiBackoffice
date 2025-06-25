using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Activity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string Subcategory { get; set; } = null!;

    public string Languages { get; set; } = null!;

    public string? LocationAddress { get; set; }

    public string? LocationCity { get; set; }

    public string? LocationCountry { get; set; }

    public double? LocationCoordinatesLatitude { get; set; }

    public double? LocationCoordinatesLongitude { get; set; }

    public int Duration { get; set; }

    public string? SeasonalAvailabilityStartDate { get; set; }

    public string? SeasonalAvailabilityEndDate { get; set; }

    public string? PriceInfoCurrency { get; set; }

    public decimal? PriceInfoBasePrice { get; set; }

    public int? PriceInfoMinimumParticipants { get; set; }

    public int? PriceInfoMaximumParticipants { get; set; }

    public string? PricingDefaultCurrency { get; set; }

    public bool? PricingTaxIncluded { get; set; }

    public decimal? PricingTaxRate { get; set; }

    public string Requirements { get; set; } = null!;

    public string Included { get; set; } = null!;

    public string? ContactInfoName { get; set; }

    public string? ContactInfoEmail { get; set; }

    public string? ContactInfoPhone { get; set; }

    public string? ContactInfoRole { get; set; }

    public string? AdditionalNotes { get; set; }

    public string Status { get; set; } = null!;

    public double Rating { get; set; }

    public string? MediaImagesHeader { get; set; }

    public string? MediaImagesTeaser { get; set; }

    public string? MediaImagesGallery { get; set; }

    public string? MediaVideos { get; set; }

    public DateTime? SalesAvailabilityStartDate { get; set; }

    public DateTime? SalesAvailabilityEndDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool? ContactInfoIsNull { get; set; }

    public bool? LocationIsNull { get; set; }

    public bool? MediaIsNull { get; set; }

    public bool? PriceInfoIsNull { get; set; }

    public bool? PricingIsNull { get; set; }

    public bool? SalesAvailabilityIsNull { get; set; }

    public bool? SeasonalAvailabilityIsNull { get; set; }

    public int? PriceInfoMaxAge { get; set; }

    public int? PriceInfoMinAge { get; set; }

    public decimal? AverageRating { get; set; }

    public string Categories { get; set; } = null!;

    public string CountryCode { get; set; } = null!;

    public string DestinationCode { get; set; } = null!;

    public string DestinationName { get; set; } = null!;

    public string Exclusions { get; set; } = null!;

    public string? Highlights { get; set; }

    public string ImportantInfo { get; set; } = null!;

    public string Inclusions { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Itinerary { get; set; }

    public string Language { get; set; } = null!;

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

    public virtual ICollection<ActivityLanguage> ActivityLanguages { get; set; } = new List<ActivityLanguage>();

    public virtual ICollection<Addon> Addons { get; set; } = new List<Addon>();

    public virtual ICollection<CancellationPolicyCondition> CancellationPolicyConditions { get; set; } = new List<CancellationPolicyCondition>();

    public virtual ICollection<MeetingPoint> MeetingPoints { get; set; } = new List<MeetingPoint>();

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual ICollection<PriceCategory> PriceCategories { get; set; } = new List<PriceCategory>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<RoutePoint> RoutePoints { get; set; } = new List<RoutePoint>();

    public virtual ICollection<TimeSlot> TimeSlots { get; set; } = new List<TimeSlot>();

    public virtual ICollection<Translation> Translations { get; set; } = new List<Translation>();
}
