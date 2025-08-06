using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingVoucher
{
    public long VoucherId { get; set; }

    public long? ItemId { get; set; }

    public string? Code { get; set; }

    public string? ExpiryDate { get; set; }

    public string? InternalNotes { get; set; }

    public string? InternalReference { get; set; }

    public string? IssueDate { get; set; }

    public string? Status { get; set; }

    public long? Value { get; set; }

    public string? ValueType { get; set; }

    public virtual BookingItem? Item { get; set; }
}
