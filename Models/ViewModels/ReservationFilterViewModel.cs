namespace TourManagementApi.Models.ViewModels
{
    public class ReservationFilterViewModel
    {

        public ReservationFilterViewModel()
        {
            Activities = new List<Activity>();
            Reservations = new List<Reservation>();
        }
        public int? ActivityId { get; set; }
        public string Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public List<Activity> Activities { get; set; } = new();
        public List<Reservation> Reservations { get; set; } = new();
    }


    public class BookingListFilterVM
    {
        public int? ActivityId { get; set; }          // Mapping ile
        public string? Status { get; set; }           // "CONFIRMED", "CANCELLED", ...
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public int TotalCount { get; set; }

        public List<ActivityLookupVM> Activities { get; set; } = new();
        public List<BookingListItemVM> Items { get; set; } = new();
    }

    public class ActivityLookupVM { public int Id { get; set; } public string Title { get; set; } = ""; }

    public class BookingListItemVM
    {
        public int BookingId { get; set; }
        public string OrderNumber { get; set; } = "";
        public string Code { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string? ExternalProductCode { get; set; }
        public DateTime? ReservationDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string CustomerName { get; set; } = "";
        public int GuestCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalCurrency { get; set; } = "";
        public string Status { get; set; } = "";
        public int? ActivityId { get; set; }
        public string? ActivityTitle { get; set; }
    }

    // --- Detay VM’leri ---
    public class BookingDetailVM
    {
        public BookingHeaderVM Header { get; set; } = new();
        public List<BookingCustomerVM> Customers { get; set; } = new();
        public List<BookingItemDetailVM> Items { get; set; } = new();
        public List<BookingPaymentVM> Payments { get; set; } = new();
        public List<BookingFieldVM> BookingFields { get; set; } = new();
        public string? InternalNotes { get; set; }
        public string? ResellerComments { get; set; }
        public string? Source { get; set; }
        public string? SourceChannel { get; set; }
        public string? SupplierName { get; set; }
        public string? ResellerName { get; set; }
    }

    public class BookingHeaderVM
    {
        public long BookingId { get; set; }
        public string OrderNumber { get; set; } = "";
        public string Code { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime? DateCreated { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalCurrency { get; set; } = "";
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public string? BarcodeValue { get; set; }     // Booking-level "Barcode" field varsa
        public string? BarcodeType { get; set; }      // QR_CODE, CODE_128...
    }

    public class BookingCustomerVM
    {
        public string Name { get; set; } = "";
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CountryCode { get; set; }
    }

    public class BookingItemDetailVM
    {
        public int ItemId { get; set; }
        public string ProductName { get; set; } = "";
        public string? ExternalProductCode { get; set; }
        public int? ActivityId { get; set; }
        public string? ActivityTitle { get; set; }
        public DateTime? StartTimeLocal { get; set; }
        public DateTime? EndTimeLocal { get; set; }
        public decimal Amount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalItemTax { get; set; }
        public int QuantitySum { get; set; }

        public List<BookingQuantityVM> Quantities { get; set; } = new();
        public List<BookingParticipantVM> Participants { get; set; } = new();
        public BookingPickupLocationVM? Pickup { get; set; }
        public List<BookingExtraVM> Extras { get; set; } = new();
        public List<BookingVoucherVM> Vouchers { get; set; } = new();
    }

    public class BookingQuantityVM { public string OptionLabel { get; set; } = ""; public int Value { get; set; } public decimal? OptionPrice { get; set; } }
    public class BookingParticipantVM
    {
        public int ParticipantId { get; set; }
        public List<BookingFieldVM> Fields { get; set; } = new(); // içinde "Barcode" alanı olabilir
    }
    public class BookingFieldVM { public string Label { get; set; } = ""; public string? Value { get; set; } public string? BarcodeType { get; set; } }
    public class BookingPaymentVM { public DateTime? Date { get; set; } public string? Type { get; set; } public decimal Amount { get; set; } public string Currency { get; set; } = ""; }
    public class BookingPickupLocationVM { public string? LocationName { get; set; } public string? Address { get; set; } public string? PickupInstructions { get; set; } public string? PickupTime { get; set; } }
    public class BookingExtraVM { public string Name { get; set; } = ""; public string? Description { get; set; } public string? ExtraPriceType { get; set; } public decimal Price { get; set; } public int Quantity { get; set; } }
    public class BookingVoucherVM { public string Code { get; set; } = ""; public string? Status { get; set; } public decimal Value { get; set; } public string? ValueType { get; set; } }


}
