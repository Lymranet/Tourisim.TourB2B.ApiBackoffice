using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingCustomer
{
    public long CustomerId { get; set; }

    public long? BookingId { get; set; }

    public string? AboutUs { get; set; }

    public string? AddressLine { get; set; }

    public string? AddressLine2 { get; set; }

    public string? City { get; set; }

    public string? CompanyName { get; set; }

    public string? CountryCode { get; set; }

    public string? Dob { get; set; }

    public string? Email { get; set; }

    public string? Fax { get; set; }

    public string? FirstName { get; set; }

    public string? Gender { get; set; }

    public long? CustomerExternalId { get; set; }

    public string? LastName { get; set; }

    public bool? Marketing { get; set; }

    public string? MiddleName { get; set; }

    public string? Mobile { get; set; }

    public string? Name { get; set; }

    public bool? Newsletter { get; set; }

    public string? Phone { get; set; }

    public string? PostCode { get; set; }

    public string? PreferredLanguage { get; set; }

    public string? Skype { get; set; }

    public string? State { get; set; }

    public string? Title { get; set; }

    public virtual Booking? Booking { get; set; }
}
