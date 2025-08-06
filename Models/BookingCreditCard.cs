using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingCreditCard
{
    public long CreditCardId { get; set; }

    public long? BookingId { get; set; }

    public string? CardToken { get; set; }

    public string? CardType { get; set; }

    public string? ExpiryMonth { get; set; }

    public string? ExpiryYear { get; set; }

    public string? CardName { get; set; }

    public string? CardNumber { get; set; }

    public string? CardSecurityNumber { get; set; }

    public virtual Booking? Booking { get; set; }
}
