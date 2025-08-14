using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Models;

namespace TourManagementApi.Data;

public partial class TourManagementDbContext : DbContext
{
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityLanguage> ActivityLanguages { get; set; }

    public virtual DbSet<Addon> Addons { get; set; }

    public virtual DbSet<AddonTranslation> AddonTranslations { get; set; }

    public virtual DbSet<Availability> Availabilities { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingCreatedBy> BookingCreatedBies { get; set; }

    public virtual DbSet<BookingCreditCard> BookingCreditCards { get; set; }

    public virtual DbSet<BookingCustomer> BookingCustomers { get; set; }

    public virtual DbSet<BookingExtra> BookingExtras { get; set; }

    public virtual DbSet<BookingField> BookingFields { get; set; }

    public virtual DbSet<BookingItem> BookingItems { get; set; }

    public virtual DbSet<BookingParticipant> BookingParticipants { get; set; }

    public virtual DbSet<BookingParticipantField> BookingParticipantFields { get; set; }

    public virtual DbSet<BookingPayment> BookingPayments { get; set; }

    public virtual DbSet<BookingPickupLocation> BookingPickupLocations { get; set; }

    public virtual DbSet<BookingQuantity> BookingQuantities { get; set; }

    public virtual DbSet<BookingResellerUser> BookingResellerUsers { get; set; }

    public virtual DbSet<BookingVoucher> BookingVouchers { get; set; }

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

    public virtual DbSet<TourCompany> TourCompanies { get; set; }

    public virtual DbSet<Translation> Translations { get; set; }

    public DbSet<ActivitySalesArea> ActivitySalesAreas { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.Property(e => e.B2BAgencyId).HasMaxLength(100);
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
            entity.Property(e => e.PartnerSupplierId).HasDefaultValue("");
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.TourCompany).WithMany(p => p.Activities)
                .HasForeignKey(d => d.TourCompanyId)
                .HasConstraintName("FK_Activities_TourCompanies");
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

            entity.Property(e => e.PartnerSupplierId).HasMaxLength(100);

            entity.HasOne(d => d.Activity).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Availabilities_Availabilities");

            entity.HasOne(d => d.Option).WithMany(p => p.Availabilities)
                .HasForeignKey(d => d.OptionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Availabilities_Options");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AED67C0017F");

            entity.Property(e => e.ApiKey).HasMaxLength(255);
            entity.Property(e => e.BarcodeType).HasMaxLength(50);
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.Commission).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Coupon).HasMaxLength(255);
            entity.Property(e => e.DateConfirmed).HasColumnType("datetime");
            entity.Property(e => e.DateCreated).HasColumnType("datetime");
            entity.Property(e => e.DatePaid).HasColumnType("datetime");
            entity.Property(e => e.DateReconciled).HasColumnType("datetime");
            entity.Property(e => e.DateUpdated).HasColumnType("datetime");
            entity.Property(e => e.OrderNumber).HasMaxLength(100);
            entity.Property(e => e.PaymentOption).HasMaxLength(100);
            entity.Property(e => e.ResellerAlias).HasMaxLength(255);
            entity.Property(e => e.ResellerName).HasMaxLength(255);
            entity.Property(e => e.ResellerReference).HasMaxLength(255);
            entity.Property(e => e.ResellerSource).HasMaxLength(255);
            entity.Property(e => e.Source).HasMaxLength(255);
            entity.Property(e => e.SourceChannel).HasMaxLength(255);
            entity.Property(e => e.SourceReferrer).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.SupplierAlias).HasMaxLength(255);
            entity.Property(e => e.SupplierName).HasMaxLength(255);
            entity.Property(e => e.Surcharge).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalCurrency).HasMaxLength(10);
            entity.Property(e => e.TotalDue).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalPaid).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<BookingCreatedBy>(entity =>
        {
            entity.HasKey(e => e.CreatedById).HasName("PK__BookingC__F5415E62DAC17E7E");

            entity.ToTable("BookingCreatedBy");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCreatedBies)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingsCr__Booki__58671BC9");
        });

        modelBuilder.Entity<BookingCreditCard>(entity =>
        {
            entity.HasKey(e => e.CreditCardId).HasName("PK__BookingC__6EB1F4F0EAFD9430");

            entity.ToTable("BookingCreditCard");

            entity.Property(e => e.CardName).HasMaxLength(255);
            entity.Property(e => e.CardNumber).HasMaxLength(50);
            entity.Property(e => e.CardSecurityNumber).HasMaxLength(10);
            entity.Property(e => e.CardToken).HasMaxLength(255);
            entity.Property(e => e.CardType).HasMaxLength(50);
            entity.Property(e => e.ExpiryMonth).HasMaxLength(10);
            entity.Property(e => e.ExpiryYear).HasMaxLength(10);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCreditCards)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingsCr__Booki__5B438874");
        });

        modelBuilder.Entity<BookingCustomer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__BookingC__A4AE64D815BAE01F");

            entity.ToTable("BookingCustomer");

            entity.Property(e => e.AboutUs).HasMaxLength(255);
            entity.Property(e => e.AddressLine).HasMaxLength(255);
            entity.Property(e => e.AddressLine2).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.CountryCode).HasMaxLength(10);
            entity.Property(e => e.Dob).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.MiddleName).HasMaxLength(100);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.PostCode).HasMaxLength(20);
            entity.Property(e => e.PreferredLanguage).HasMaxLength(50);
            entity.Property(e => e.Skype).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(50);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingCustomers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingsCu__Booki__558AAF1E");
        });

        modelBuilder.Entity<BookingExtra>(entity =>
        {
            entity.HasKey(e => e.ExtraId).HasName("PK__BookingE__D1F3A8275C6969AA");

            entity.Property(e => e.ExtraPriceType).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Item).WithMany(p => p.BookingExtras)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__BookingEx__ItemI__6F4A8121");
        });

        modelBuilder.Entity<BookingField>(entity =>
        {
            entity.HasKey(e => e.FieldId).HasName("PK__BookingF__C8B6FF07B824CEBD");

            entity.Property(e => e.BarcodeType).HasMaxLength(50);
            entity.Property(e => e.Label).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingFields)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingFi__Booki__60FC61CA");
        });

        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__BookingI__727E838B19488869");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EndTime).HasMaxLength(50);
            entity.Property(e => e.EndTimeLocal).HasMaxLength(50);
            entity.Property(e => e.ExternalProductCode).HasMaxLength(100);
            entity.Property(e => e.ProductCode).HasMaxLength(100);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.StartTime).HasMaxLength(50);
            entity.Property(e => e.StartTimeLocal).HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TotalItemTax).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TransferFrom).HasMaxLength(255);
            entity.Property(e => e.TransferTo).HasMaxLength(255);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingItems)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingsIt__Booki__63D8CE75");
        });

        modelBuilder.Entity<BookingParticipant>(entity =>
        {
            entity.HasKey(e => e.ParticipantId).HasName("PK__BookingP__7227995ED5D1B1A6");

            entity.HasOne(d => d.Item).WithMany(p => p.BookingParticipants)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__BookingPa__ItemI__6991A7CB");
        });

        modelBuilder.Entity<BookingParticipantField>(entity =>
        {
            entity.HasKey(e => e.FieldId).HasName("PK__BookingP__C8B6FF079BD8464C");

            entity.Property(e => e.BarcodeType).HasMaxLength(50);
            entity.Property(e => e.Label).HasMaxLength(255);

            entity.HasOne(d => d.Participant).WithMany(p => p.BookingParticipantFields)
                .HasForeignKey(d => d.ParticipantId)
                .HasConstraintName("FK__BookingPa__Parti__6C6E1476");
        });

        modelBuilder.Entity<BookingPayment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__BookingP__9B556A386817D66C");

            entity.Property(e => e.Currency).HasMaxLength(10);
            entity.Property(e => e.Date).HasMaxLength(30);
            entity.Property(e => e.Label).HasMaxLength(255);
            entity.Property(e => e.Recipient).HasMaxLength(255);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingPayments)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingsPa__Booki__5E1FF51F");
        });

        modelBuilder.Entity<BookingPickupLocation>(entity =>
        {
            entity.HasKey(e => e.PickupLocationId).HasName("PK__BookingP__F6FC9D68A088167A");

            entity.ToTable("BookingPickupLocation");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.LocationName).HasMaxLength(255);
            entity.Property(e => e.PickupTime).HasMaxLength(50);

            entity.HasOne(d => d.Item).WithMany(p => p.BookingPickupLocations)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__BookingPi__ItemI__75035A77");
        });

        modelBuilder.Entity<BookingQuantity>(entity =>
        {
            entity.HasKey(e => e.QuantityId).HasName("PK__BookingQ__51531453D8164E58");

            entity.Property(e => e.OptionLabel).HasMaxLength(255);

            entity.HasOne(d => d.Item).WithMany(p => p.BookingQuantities)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__BookingQu__ItemI__66B53B20");
        });

        modelBuilder.Entity<BookingResellerUser>(entity =>
        {
            entity.HasKey(e => e.ResellerUserId).HasName("PK__BookingR__2DD2F9C10ED7AA5D");

            entity.ToTable("BookingResellerUser");

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);

            entity.HasOne(d => d.Booking).WithMany(p => p.BookingResellerUsers)
                .HasForeignKey(d => d.BookingId)
                .HasConstraintName("FK__BookingsRe__Booki__77DFC722");
        });

        modelBuilder.Entity<BookingVoucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__BookingV__3AEE7921D91C6E03");

            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.ExpiryDate).HasMaxLength(50);
            entity.Property(e => e.InternalReference).HasMaxLength(255);
            entity.Property(e => e.IssueDate).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.ValueType).HasMaxLength(50);

            entity.HasOne(d => d.Item).WithMany(p => p.BookingVouchers)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__BookingVo__ItemI__7226EDCC");
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

            entity.HasIndex(e => e.OptionId, "IX_Reservations_OptionId");

            entity.Property(e => e.BookingId).HasDefaultValue("");
            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
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

            entity.Property(e => e.Status).HasMaxLength(50);

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

            entity.HasOne(d => d.Availability).WithMany(p => p.TicketCategoryCapacities)
                .HasForeignKey(d => d.AvailabilityId)
                .HasConstraintName("FK__TicketCat__Avail__69FBBC1F");

            entity.HasOne(d => d.TicketCategory).WithMany(p => p.TicketCategoryCapacities)
                .HasForeignKey(d => d.TicketCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TicketCategoryCapacities_TicketCategories");
        });

        modelBuilder.Entity<TimeSlot>(entity =>
        {
            entity.HasKey(e => new { e.ActivityId, e.Id });

            entity.ToTable("TimeSlot");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Activity).WithMany(p => p.TimeSlots).HasForeignKey(d => d.ActivityId);
        });

        modelBuilder.Entity<TourCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TourComp__3214EC07A431D01F");

            entity.Property(e => e.AracD2BelgesiPath).HasMaxLength(500);
            entity.Property(e => e.AuthorizedPerson).HasMaxLength(255);
            entity.Property(e => e.CompanyName).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FaaliyetBelgesiPath).HasMaxLength(500);
            entity.Property(e => e.HizmetDetayiPath).HasMaxLength(500);
            entity.Property(e => e.ImzaDocumentPath).HasMaxLength(500);
            entity.Property(e => e.LogoPath).HasMaxLength(500);
            entity.Property(e => e.OdaSicilKaydiPath).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.SigortaBelgesiPath).HasMaxLength(500);
            entity.Property(e => e.SportifFaaliyetBelgesiPath).HasMaxLength(500);
            entity.Property(e => e.TicaretSicilGazetesiPath).HasMaxLength(500);
            entity.Property(e => e.VergiLevhasıPath).HasMaxLength(500);
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
