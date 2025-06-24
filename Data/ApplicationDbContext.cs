using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using TourManagementApi.Models;
using TourManagementApi.Models.Common;

namespace TourManagementApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Activity> Activities { get; set; } = null!;
    public DbSet<Option> Options { get; set; } = null!;
    public DbSet<OpeningHour> OpeningHours { get; set; } = null!;
    public DbSet<TicketCategory> TicketCategories { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<ReservationGuest> ReservationGuests { get; set; }
    public DbSet<ActivityLanguage> ActivityLanguages { get; set; }
    public DbSet<Translation> Translations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Convert collections to JSON with value comparers
            entity.Property(e => e.Languages)
                .HasJsonConversion(jsonOptions)
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.Requirements)
                .HasJsonConversion(jsonOptions)
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.Included)
                .HasJsonConversion(jsonOptions)
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.Exclusions)
                .HasJsonConversion(jsonOptions)
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.Categories)
                .HasJsonConversion(jsonOptions)
                .HasDefaultValueSql("'[]'")
                .IsRequired()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.Inclusions)
                .HasJsonConversion(jsonOptions)
                .HasDefaultValueSql("'[]'")
                .IsRequired()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.ImportantInfo)
                .HasJsonConversion(jsonOptions)
                .HasDefaultValueSql("'[]'")
                .IsRequired()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.Property(e => e.GuestFields)
                .HasJsonConversion(jsonOptions)
                .HasDefaultValueSql("'[]'")
                .IsRequired()
                .Metadata.SetValueComparer(new CollectionValueComparer<GuestField>());

            entity.Property(e => e.AverageRating)
                .HasPrecision(4, 2);

            // Configure owned types that have collections
            entity.OwnsMany(e => e.MeetingPoints);
            entity.OwnsMany(e => e.RoutePoints);
            entity.OwnsMany(e => e.TimeSlots, ts =>
            {
                ts.Property(t => t.DaysOfWeek)
                    .HasJsonConversion(jsonOptions)
                    .Metadata.SetValueComparer(new CollectionValueComparer<string>());
            });

            // Configure optional owned types
            entity.OwnsOne(e => e.Location, l =>
            {
                l.Property(p => p.Address).IsRequired(false);
                l.Property(p => p.City).IsRequired(false);
                l.Property(p => p.Country).IsRequired(false);
                l.Property<bool>("_isNull").HasColumnName("LocationIsNull");
            });

            entity.OwnsOne(e => e.SeasonalAvailability, s =>
            {
                s.Property<bool>("_isNull").HasColumnName("SeasonalAvailabilityIsNull");
            });

            entity.OwnsOne(e => e.PriceInfo, p =>
            {
                p.Property(pi => pi.BasePrice).HasPrecision(18, 2);
                p.Property<bool>("_isNull").HasColumnName("PriceInfoIsNull");
            });

            entity.OwnsOne(e => e.Pricing, p =>
            {
                p.Property(ap => ap.DefaultCurrency).IsRequired(false);
                p.Property(ap => ap.TaxRate).HasPrecision(18, 2);
                p.OwnsMany(ap => ap.Categories, c =>
                {
                    c.Property(pc => pc.Amount).HasPrecision(18, 2);
                    c.Property(pc => pc.DiscountValue).HasPrecision(18, 2);
                });
                p.Property<bool>("_isNull").HasColumnName("PricingIsNull");
            });

            entity.OwnsOne(e => e.ContactInfo, c =>
            {
                c.Property(p => p.Name).IsRequired(false);
                c.Property(p => p.Email).IsRequired(false);
                c.Property(p => p.Phone).IsRequired(false);
                c.Property(p => p.Role).IsRequired(false);
                c.Property<bool>("_isNull").HasColumnName("ContactInfoIsNull");
            });

            entity.OwnsOne(e => e.Media, m =>
            {
                m.Property(p => p.Videos).IsRequired(false);
                m.OwnsOne(p => p.Images, i =>
                {
                    i.Property(p => p.Gallery).IsRequired(false);
                    i.Property(p => p.Header).IsRequired(false);
                    i.Property(p => p.Teaser).IsRequired(false);
                });
                m.Property<bool>("_isNull").HasColumnName("MediaIsNull");
            });

            entity.OwnsOne(e => e.SalesAvailability, s =>
            {
                s.Property<bool>("_isNull").HasColumnName("SalesAvailabilityIsNull");
            });
        });

        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Activity)
                .WithMany(a => a.Options)
                .HasForeignKey(e => e.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.Weekdays)
                .HasJsonConversion(jsonOptions)
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());

            entity.HasMany(e => e.OpeningHours)
                .WithOne(o => o.Option)
                .HasForeignKey(o => o.OptionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.TicketCategories)
                .WithOne(t => t.Option)
                .HasForeignKey(t => t.OptionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OpeningHour>(entity =>
        {
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TicketCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2);
        });

        modelBuilder.Entity<Reservation>()
           .HasOne(r => r.Activity)
           .WithMany()
           .HasForeignKey(r => r.ActivityId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Option)
            .WithMany()
            .HasForeignKey(r => r.OptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReservationGuest>()
            .HasOne(g => g.Reservation)
            .WithMany(r => r.Guests)
            .HasForeignKey(g => g.ReservationId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Reservation>()
      .HasIndex(r => r.ExperienceBankBookingId)
      .IsUnique();

        modelBuilder.Entity<ReservationGuest>()
            .Property(g => g.AdditionalFieldsJson)
            .HasColumnType("nvarchar(max)");

        modelBuilder.Entity<ReservationGuest>()
            .Property(g => g.AddonsJson)
            .HasColumnType("nvarchar(max)");
    }
}

public static class ModelBuilderExtensions
{
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder, JsonSerializerOptions options) where T : class
    {
        return propertyBuilder.HasConversion(
            v => v == null ? "[]" : JsonSerializer.Serialize(v, options),
            v => string.IsNullOrEmpty(v) ? null : JsonSerializer.Deserialize<T>(v, options)
        );
    }
}

public class CollectionValueComparer<T> : ValueComparer<List<T>>
{
    public CollectionValueComparer() : base(
        (c1, c2) => (c1 == null && c2 == null) || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
        c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)) : 0,
        c => c != null ? new List<T>(c) : new List<T>())
    {
    }
} 