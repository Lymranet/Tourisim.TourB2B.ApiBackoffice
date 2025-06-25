using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Models;

namespace TourManagementApi.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityLanguage> ActivityLanguages { get; set; }

    public virtual DbSet<Addon> Addons { get; set; }

    public virtual DbSet<AddonTranslation> AddonTranslations { get; set; }

    public virtual DbSet<Availability> Availabilities { get; set; }

    public virtual DbSet<CancellationPolicyCondition> CancellationPolicyConditions { get; set; }

    public virtual DbSet<MeetingPoint> MeetingPoints { get; set; }

    public virtual DbSet<OpeningHour> OpeningHours { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<PriceCategory> PriceCategories { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<ReservationGuest> ReservationGuests { get; set; }

    public virtual DbSet<RoutePoint> RoutePoints { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<TicketCategory> TicketCategories { get; set; }

    public virtual DbSet<TicketCategoryCapacity> TicketCategoryCapacities { get; set; }

    public virtual DbSet<TimeSlot> TimeSlots { get; set; }

    public virtual DbSet<Translation> Translations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=213.159.29.100;Database=TourManagementDb;User Id=tourlogin;Password=d93cHL:?bHb[.a%/4c3:yGALR{KGBAdsa;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(e => e.AverageRating).HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Categories).HasDefaultValue("[]");
            entity.Property(e => e.ContactInfoEmail).HasColumnName("ContactInfo_Email");
            entity.Property(e => e.ContactInfoName).HasColumnName("ContactInfo_Name");
            entity.Property(e => e.ContactInfoPhone).HasColumnName("ContactInfo_Phone");
            entity.Property(e => e.ContactInfoRole).HasColumnName("ContactInfo_Role");
            entity.Property(e => e.CountryCode).HasDefaultValue("");
            entity.Property(e => e.DestinationCode).HasDefaultValue("");
            entity.Property(e => e.DestinationName).HasDefaultValue("");
            entity.Property(e => e.DetailsUrl).HasDefaultValue("");
            entity.Property(e => e.ExclusionsJson).HasDefaultValue("");
            entity.Property(e => e.GuestFields).HasDefaultValue("[]");
            entity.Property(e => e.GuestFieldsJson).HasDefaultValue("");
            entity.Property(e => e.ImportantInfo).HasDefaultValue("[]");
            entity.Property(e => e.ImportantInfoJson).HasDefaultValue("");
            entity.Property(e => e.Inclusions).HasDefaultValue("[]");
            entity.Property(e => e.InclusionsJson).HasDefaultValue("");
            entity.Property(e => e.Label).HasDefaultValue("");
            entity.Property(e => e.Language).HasDefaultValue("");
            entity.Property(e => e.LocationAddress).HasColumnName("Location_Address");
            entity.Property(e => e.LocationCity).HasColumnName("Location_City");
            entity.Property(e => e.LocationCoordinatesLatitude).HasColumnName("Location_Coordinates_Latitude");
            entity.Property(e => e.LocationCoordinatesLongitude).HasColumnName("Location_Coordinates_Longitude");
            entity.Property(e => e.LocationCountry).HasColumnName("Location_Country");
            entity.Property(e => e.MediaImagesGallery).HasColumnName("Media_Images_Gallery");
            entity.Property(e => e.MediaImagesHeader).HasColumnName("Media_Images_Header");
            entity.Property(e => e.MediaImagesTeaser).HasColumnName("Media_Images_Teaser");
            entity.Property(e => e.MediaVideos).HasColumnName("Media_Videos");
            entity.Property(e => e.PartnerSupplierId).HasDefaultValue("");
            entity.Property(e => e.PriceInfoBasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PriceInfo_BasePrice");
            entity.Property(e => e.PriceInfoCurrency).HasColumnName("PriceInfo_Currency");
            entity.Property(e => e.PriceInfoMaxAge).HasColumnName("PriceInfo_MaxAge");
            entity.Property(e => e.PriceInfoMaximumParticipants).HasColumnName("PriceInfo_MaximumParticipants");
            entity.Property(e => e.PriceInfoMinAge).HasColumnName("PriceInfo_MinAge");
            entity.Property(e => e.PriceInfoMinimumParticipants).HasColumnName("PriceInfo_MinimumParticipants");
            entity.Property(e => e.PricingDefaultCurrency).HasColumnName("Pricing_DefaultCurrency");
            entity.Property(e => e.PricingTaxIncluded).HasColumnName("Pricing_TaxIncluded");
            entity.Property(e => e.PricingTaxRate)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Pricing_TaxRate");
            entity.Property(e => e.SalesAvailabilityEndDate).HasColumnName("SalesAvailability_EndDate");
            entity.Property(e => e.SalesAvailabilityStartDate).HasColumnName("SalesAvailability_StartDate");
            entity.Property(e => e.SeasonalAvailabilityEndDate).HasColumnName("SeasonalAvailability_EndDate");
            entity.Property(e => e.SeasonalAvailabilityStartDate).HasColumnName("SeasonalAvailability_StartDate");
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<ActivityLanguage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ActivityLanguage");

            entity.HasIndex(e => e.ActivityId, "IX_ActivityLanguage_ActivityId");

            entity.Property(e => e.LanguageCode).HasMaxLength(10);

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityLanguages)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_ActivityLanguage_Activities_ActivityId");
        });

        modelBuilder.Entity<Addon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Addons__3214EC07FF36CB8D");

            entity.Property(e => e.Currency)
                .HasMaxLength(10)
                .HasDefaultValue("TRY");
            entity.Property(e => e.PriceAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Activity).WithMany(p => p.Addons)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_Addons_Activities");
        });

        modelBuilder.Entity<AddonTranslation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AddonTra__3214EC07325ED02A");

            entity.Property(e => e.Language).HasMaxLength(10);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Addon).WithMany(p => p.AddonTranslations)
                .HasForeignKey(d => d.AddonId)
                .HasConstraintName("FK_AddonTranslations_Addons");
        });

        modelBuilder.Entity<Availability>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Availabi__3214EC0722F78A97");

            entity.Property(e => e.ActivityId).HasMaxLength(100);
            entity.Property(e => e.OptionId).HasMaxLength(100);
            entity.Property(e => e.PartnerSupplierId).HasMaxLength(100);
        });

        modelBuilder.Entity<CancellationPolicyCondition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cancella__3214EC070325041F");

            entity.HasOne(d => d.Activity).WithMany(p => p.CancellationPolicyConditions)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_CancellationPolicyConditions_Activity");
        });

        modelBuilder.Entity<MeetingPoint>(entity =>
        {
            entity.HasKey(e => new { e.ActivityId, e.Id });

            entity.ToTable("MeetingPoint");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Activity).WithMany(p => p.MeetingPoints).HasForeignKey(d => d.ActivityId);
        });

        modelBuilder.Entity<OpeningHour>(entity =>
        {
            entity.HasIndex(e => e.OptionId, "IX_OpeningHours_OptionId");

            entity.HasOne(d => d.Option).WithMany(p => p.OpeningHours).HasForeignKey(d => d.OptionId);
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasIndex(e => e.ActivityId, "IX_Options_ActivityId");

            entity.Property(e => e.Duration).HasDefaultValue("");
            entity.Property(e => e.EndTime).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.StartTime).HasMaxLength(50);

            entity.HasOne(d => d.Activity).WithMany(p => p.Options).HasForeignKey(d => d.ActivityId);
        });

        modelBuilder.Entity<PriceCategory>(entity =>
        {
            entity.HasKey(e => new { e.ActivityPricingActivityId, e.Id });

            entity.ToTable("PriceCategory");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.ActivityPricingActivity).WithMany(p => p.PriceCategories).HasForeignKey(d => d.ActivityPricingActivityId);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasIndex(e => e.ActivityId, "IX_Reservations_ActivityId");

            entity.HasIndex(e => e.ExperienceBankBookingId, "IX_Reservations_ExperienceBankBookingId").IsUnique();

            entity.HasIndex(e => e.OptionId, "IX_Reservations_OptionId");

            entity.Property(e => e.ExperienceBankBookingId).HasDefaultValue("");
            entity.Property(e => e.MarketplaceBookingId).HasDefaultValue("");
            entity.Property(e => e.MarketplaceId).HasDefaultValue("");
            entity.Property(e => e.Notes).HasDefaultValue("");
            entity.Property(e => e.PartnerBookingId).HasDefaultValue("");
            entity.Property(e => e.PartnerSupplierId).HasDefaultValue("");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Activity).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Option).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.OptionId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ReservationGuest>(entity =>
        {
            entity.HasIndex(e => e.ReservationId, "IX_ReservationGuests_ReservationId");

            entity.Property(e => e.AdditionalFieldsJson).HasDefaultValue("");
            entity.Property(e => e.AddonsJson).HasDefaultValue("");
            entity.Property(e => e.Email).HasDefaultValue("");
            entity.Property(e => e.FirstName).HasDefaultValue("");
            entity.Property(e => e.LastName).HasDefaultValue("");
            entity.Property(e => e.PhoneNumber).HasDefaultValue("");
            entity.Property(e => e.TicketCategory).HasDefaultValue("");
            entity.Property(e => e.TicketId).HasDefaultValue("");

            entity.HasOne(d => d.Reservation).WithMany(p => p.ReservationGuests).HasForeignKey(d => d.ReservationId);
        });

        modelBuilder.Entity<RoutePoint>(entity =>
        {
            entity.HasKey(e => new { e.ActivityId, e.Id });

            entity.ToTable("RoutePoint");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Activity).WithMany(p => p.RoutePoints).HasForeignKey(d => d.ActivityId);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("Ticket");

            entity.HasIndex(e => e.ReservationId, "IX_Ticket_ReservationId");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Tickets).HasForeignKey(d => d.ReservationId);
        });

        modelBuilder.Entity<TicketCategory>(entity =>
        {
            entity.HasIndex(e => e.OptionId, "IX_TicketCategories_OptionId");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Option).WithMany(p => p.TicketCategories).HasForeignKey(d => d.OptionId);
        });

        modelBuilder.Entity<TicketCategoryCapacity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TicketCa__3214EC07C98BD1B7");

            entity.Property(e => e.TicketCategoryId).HasMaxLength(100);

            entity.HasOne(d => d.Availability).WithMany(p => p.TicketCategoryCapacities)
                .HasForeignKey(d => d.AvailabilityId)
                .HasConstraintName("FK__TicketCat__Avail__69FBBC1F");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => new { e.ActivityId, e.Id });

            entity.ToTable("TimeSlot");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Activity).WithMany(p => p.TimeSlots).HasForeignKey(d => d.ActivityId);
        });

        modelBuilder.Entity<Translation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Translat__3214EC07A96AFBF4");

            entity.HasIndex(e => new { e.ActivityId, e.Language }, "UX_Translation_Activity_Language").IsUnique();

            entity.Property(e => e.Language).HasMaxLength(10);

            entity.HasOne(d => d.Activity).WithMany(p => p.Translations)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Translations_Activities");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
