using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingItem
{
    public long ItemId { get; set; }

    public long? BookingId { get; set; }

    public string? ProductCode { get; set; }

    public int? TotalQuantity { get; set; }

    public string? ProductName { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? TotalItemTax { get; set; }

    public string? StartTime { get; set; }

    public string? StartTimeLocal { get; set; }

    public string? EndTime { get; set; }

    public string? EndTimeLocal { get; set; }

    public string? ExternalProductCode { get; set; }

    public string? TransferFrom { get; set; }

    public string? TransferTo { get; set; }

    public bool? TransferReturn { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<BookingExtra> BookingExtras { get; set; } = new List<BookingExtra>();

    public virtual ICollection<BookingParticipant> BookingParticipants { get; set; } = new List<BookingParticipant>();

    public virtual ICollection<BookingPickupLocation> BookingPickupLocations { get; set; } = new List<BookingPickupLocation>();

    public virtual ICollection<BookingQuantity> BookingQuantities { get; set; } = new List<BookingQuantity>();

    public virtual ICollection<BookingVoucher> BookingVouchers { get; set; } = new List<BookingVoucher>();
}
