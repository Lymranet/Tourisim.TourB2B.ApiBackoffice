using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingPayment
{
    public long PaymentId { get; set; }

    public long? BookingId { get; set; }

    public double? Amount { get; set; }

    public string? Type { get; set; }

    public string? Currency { get; set; }

    public string? Date { get; set; }

    public string? Label { get; set; }

    public string? Recipient { get; set; }

    public virtual Booking? Booking { get; set; }
}
