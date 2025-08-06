namespace TourManagementApi.Models.Api.Rezdy
{
    public class RezdyBookingRequest
    {
        public string? ApiKey { get; set; }
        public string? Comments { get; set; }
        public decimal? Commission { get; set; }
        public string? Coupon { get; set; }
        public BookingRequestCreatedBy? CreatedBy { get; set; }
        public BookingRequestCreditCard? CreditCard { get; set; }
        public BookingRequestCustomer? Customer { get; set; }
        public List<BookingRequestFields>? Fields { get; set; }
        public List<BookingRequestItems> Items { get; set; } = new();
        public List<BookingRequestPayments>? Payments { get; set; }
        public BookingRequestResellerUser? ResellerUser { get; set; }
        public string? Status { get; set; }
        public string? Code { get; set; }
        public string? DateConfirmed { get; set; }
        public string? DateCreated { get; set; }
        public string? DatePaid { get; set; }
        public string? DateReconciled { get; set; }
        public string? DateUpdated { get; set; }
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
        public List<string>? Vouchers { get; set; }
        public string? BarcodeType { get; set; }
    }

    public class ConfirmationRequest
    {
        public string ReservationId { get; set; } = null!;
    }
    public class BookingRequestCustomer
    {
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
        public long? Id { get; set; }
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
    }


    public class BookingRequestCreatedBy
    {
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }

    public class BookingRequestCreditCard
    {
        public string? CardToken { get; set; }
        public string? CardType { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? CardName { get; set; }
        public string? CardNumber { get; set; }
        public string? CardSecurityNumber { get; set; }
    }

    public class BookingRequestFields
    {
        public string? Label { get; set; }
        public string? Value { get; set; }
        public string? BarcodeType { get; set; }
    }

    public class BookingRequestItems
    {
        public string? ProductCode { get; set; }
        public int TotalQuantity { get; set; }
        public List<BookingRequestQuantities>? Quantities { get; set; }
        public List<BookingRequestParticipants>? Participants { get; set; }
        public List<BookingRequestExtras>? Extras { get; set; }
        public List<BookingRequestVouchers>? Vouchers { get; set; }
        public BookingRequestPickupLocation? PickupLocation { get; set; }

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
    }

    public class BookingRequestQuantities
    {
        public string OptionLabel { get; set; }
        public int Value { get; set; }
        public float? OptionPrice { get; set; }
        public int? OptionSeatsUsed { get; set; }
    }
    public class BookingRequestParticipants
    {
        public List<BookingRequestFields> Fields { get; set; } = new();
    }

    public class BookingRequestExtras
    {
        public string Name { get; set; }
        public string ExtraPriceType { get; set; } // "ANY", "FIXED", "QUANTITY"
        public int Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }
    public class BookingRequestVouchers
    {
        public string Code { get; set; }
        public string ExpiryDate { get; set; }
        public string InternalNotes { get; set; }
        public string InternalReference { get; set; }
        public string IssueDate { get; set; }
        public string Status { get; set; } // "ISSUED", "REDEEMED", etc.
        public long? Value { get; set; }
        public string ValueType { get; set; } // "PERCENT", "VALUE", etc.
    }
    public class BookingRequestPickupLocation
    {
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public long? Latitude { get; set; }
        public long? Longitude { get; set; }
        public int? MinutesPrior { get; set; }
        public string? PickupInstructions { get; set; }
        public string? PickupTime { get; set; }
    }

    public class BookingRequestPayments
    {
        public float Amount { get; set; }
        public string Type { get; set; } = null!;
        public string? Currency { get; set; }
        public string? Date { get; set; }
        public string? Label { get; set; }
        public string? Recipient { get; set; }
    }

    public class BookingRequestResellerUser
    {
        public string? Code { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
