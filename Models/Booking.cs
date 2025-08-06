using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Booking
{
    public long BookingId { get; set; }

    public string? ApiKey { get; set; }

    public string? Comments { get; set; }

    public decimal? Commission { get; set; }

    public string? Coupon { get; set; }

    public string? Status { get; set; }

    public string? Code { get; set; }

    public DateTime? DateConfirmed { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DatePaid { get; set; }

    public DateTime? DateReconciled { get; set; }

    public DateTime? DateUpdated { get; set; }

    public string? InternalNotes { get; set; }

    public string? OrderNumber { get; set; }

    public string? PaymentOption { get; set; }

    public string? ResellerAlias { get; set; }

    public string? ResellerComments { get; set; }

    public long? ResellerId { get; set; }

    public string? ResellerName { get; set; }

    public string? ResellerReference { get; set; }

    public string? ResellerSource { get; set; }

    public bool? SendNotifications { get; set; }

    public string? Source { get; set; }

    public string? SourceChannel { get; set; }

    public string? SourceReferrer { get; set; }

    public string? SupplierAlias { get; set; }

    public long? SupplierId { get; set; }

    public string? SupplierName { get; set; }

    public decimal? Surcharge { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? TotalCurrency { get; set; }

    public decimal? TotalDue { get; set; }

    public decimal? TotalPaid { get; set; }

    public string? BarcodeType { get; set; }

    public virtual ICollection<BookingCreatedBy> BookingCreatedBies { get; set; } = new List<BookingCreatedBy>();

    public virtual ICollection<BookingCreditCard> BookingCreditCards { get; set; } = new List<BookingCreditCard>();

    public virtual ICollection<BookingCustomer> BookingCustomers { get; set; } = new List<BookingCustomer>();

    public virtual ICollection<BookingField> BookingFields { get; set; } = new List<BookingField>();

    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    public virtual ICollection<BookingPayment> BookingPayments { get; set; } = new List<BookingPayment>();

    public virtual ICollection<BookingResellerUser> BookingResellerUsers { get; set; } = new List<BookingResellerUser>();
}
