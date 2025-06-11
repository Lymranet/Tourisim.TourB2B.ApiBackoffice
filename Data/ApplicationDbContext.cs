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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Convert collections to JSON with value comparers
            entity.Property(e => e.Languages)
                .HasJsonConversion()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());
            
            entity.Property(e => e.Requirements)
                .HasJsonConversion()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());
            
            entity.Property(e => e.Included)
                .HasJsonConversion()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());
            
            entity.Property(e => e.Excluded)
                .HasJsonConversion()
                .Metadata.SetValueComparer(new CollectionValueComparer<string>());
            
            // Configure owned types that have collections
            entity.OwnsMany(e => e.MeetingPoints);
            entity.OwnsMany(e => e.TimeSlots, ts =>
            {
                ts.Property(t => t.DaysOfWeek)
                    .HasJsonConversion()
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
            
            entity.OwnsMany(e => e.GuestFields, gf =>
            {
                gf.OwnsMany(f => f.Options, o =>
                {
                    o.OwnsMany(opt => opt.Translations);
                });
                gf.OwnsMany(f => f.Translations);
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
    }
}

public static class ModelBuilderExtensions
{
    public static PropertyBuilder<List<string>> HasJsonConversion(this PropertyBuilder<List<string>> propertyBuilder)
    {
        var options = new JsonSerializerOptions();
        return propertyBuilder.HasConversion(
            v => JsonSerializer.Serialize(v, options),
            v => JsonSerializer.Deserialize<List<string>>(v, options) ?? new List<string>()
        );
    }
}

public class CollectionValueComparer<T> : ValueComparer<List<T>>
{
    public CollectionValueComparer() : base(
        (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
        c => c != null ? c.Aggregate(0, (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)) : 0,
        c => c != null ? new List<T>(c) : new List<T>())
    {
    }
} 