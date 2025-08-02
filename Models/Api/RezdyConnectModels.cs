using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.Api.RezdyConnectModels
{

    public class BookingDto
    {
        [Required]
        public string ProductCode { get; set; }

        [Required]
        public CustomerDto Customer { get; set; }

        public List<ParticipantDto> Participants { get; set; }
        public DateTime StartTime { get; set; }
        public string Currency { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderNumber { get; set; }
        public int SupplierId { get; set; }
        public string Status { get; internal set; }

        public string ExternalProductCode { get; set; } = null!; // For Rezdy, this is the ExternalCode
    }



    public class ProductLocation
    {
        public string Address { get; set; }
        public GeoCoordinates Geo { get; set; }
    }
    public class ConfirmationRequest
    {
        public string ReservationId { get; set; } = null!;
    }
    public class GeoCoordinates
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public class ProductImage
    {
        public string Url { get; set; }
        public bool IsThumbnail { get; set; }
    }

    public class ProductOption
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<ProductPricing> Pricing { get; set; }
    }

    public class ProductPricing
    {
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }
    public class AvailabilityRequest
    {
        public string ProductCode { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public DateTime EndTimeLocal { get; set; }
    }
    public class AvailabilitySessionResponse
    {
        public string ProductCode { get; set; }
        public string SessionCode { get; set; }

        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public string StartTimeLocal { get; set; }
        public string EndTimeLocal { get; set; }

        public int Seats { get; set; }
        public int SeatsAvailable { get; set; }

        public bool AllDay { get; set; }
        public string CutoffUnit { get; set; }
        public int CutoffWindowDuration { get; set; }

        public string LastUpdated { get; set; }

        public List<PriceOption> PriceOptions { get; set; }
    }
    //public class AvailabilitySessionResponse
    //{
    //    public string ProductCode { get; set; }
    //    public string SessionCode { get; set; }

    //    public string StartTime { get; set; }        // ISO8601 UTC format
    //    public string EndTime { get; set; }

    //    public string StartTimeLocal { get; set; }   // Local format
    //    public string EndTimeLocal { get; set; }

    //    public int Seats { get; set; }
    //    public int SeatsAvailable { get; set; }

    //    public List<PriceOption> PriceOptions { get; set; }
    //}

    public class PriceOption
    {
        public string Label { get; set; } = null!;
        public decimal Price { get; set; }
    }
    public class ProductResponseModel
    {
        public string ProductCode { get; set; } = null!;
        public string ExternalCode { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string BookingMode { get; set; } = "INVENTORY"; // DATE_ENQUIRY, NO_DATE
        public string BarcodeType { get; set; } = "QR_CODE";   // CODE_128, TEXT vs.
        public string BarcodeOutputType { get; set; } = "ORDER"; // or PARTICIPANT
        public List<BookingField>? BookingFields { get; set; }
    }

    public class ProductModel
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public ProductLocation Location { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<ProductOption> Options { get; set; }
        public string ExternalCode { get; internal set; }
    }

    //public class BookingField
    //{
    //    public string Label { get; set; } = null!;
    //    public string FieldType { get; set; } = "String"; // Boolean, List, etc.
    //    public bool RequiredPerParticipant { get; set; }
    //    public bool RequiredPerBooking { get; set; }
    //    public bool VisiblePerParticipant { get; set; }
    //    public bool VisiblePerBooking { get; set; }
    //    public string? ListOptions { get; set; } // For List fields
    //}
    public class AvailabilityResponse
    {
        public string ProductCode { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public int RemainingPlaces { get; set; }
    }

    public class CancellationRequest
    {
        public string BookingReference { get; set; } = null!;
        public string Reason { get; set; } = null!;
    }
    public class BookingCreateResponse
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; } = null!;
        public string Status { get; set; } = "Confirmed";
    }








    public class RezdyBookingDto
    {
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAlias { get; set; }
        public CreatedByDto CreatedBy { get; set; }
        public int ResellerId { get; set; }
        public string ResellerName { get; set; }
        public string ResellerAlias { get; set; }
        public ResellerUserDto ResellerUser { get; set; }
        public RezdyCustomerDto Customer { get; set; }
        public List<RezdyItemDto> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string TotalCurrency { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal TotalDue { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public DateTime? DatePaid { get; set; }
        public List<PaymentDto> Payments { get; set; }
        public string Source { get; set; }
        public string SourceChannel { get; set; }
        public decimal Commission { get; set; }
        public string ResellerReference { get; set; }
        public string BarcodeType { get; set; }
    }

    public class CreatedByDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class ResellerUserDto
    {
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class PaymentDto
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Date { get; set; }
        public string Label { get; set; }
        public string Recipient { get; set; }
    }

    public class RezdyCustomerDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class RezdyItemDto
    {
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string StartTimeLocal { get; set; }
        public string EndTimeLocal { get; set; }
        public List<QuantityDto> Quantities { get; set; }
        public int TotalQuantity { get; set; }
        public decimal Amount { get; set; }
        public List<RezdyParticipantDto> Participants { get; set; }
        public bool TransferReturn { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TotalItemTax { get; set; }
        public string ExternalProductCode { get; set; }
    }

    public class QuantityDto
    {
        public string OptionLabel { get; set; }
        public decimal OptionPrice { get; set; }
        public int Value { get; set; }
        public string CommissionType { get; set; }
        public decimal CommissionValue { get; set; }
    }

    public class RezdyParticipantDto
    {
        public List<RezdyFieldDto> Fields { get; set; }
    }

    public class RezdyFieldDto
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class CustomerDto
    {
        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }
    }
    public partial class BookingRequest
    {
        public string BookingReference { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public int OptionId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EUR";
        public string ContactName { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
        public string ContactPhone { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string? Notes { get; set; }
        public string OptionCode { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public CustomerData Customer { get; set; }
        public List<Participant> Participants { get; set; }
        public int Quantity { get; set; }
        public string OrderNumber { get; internal set; }
    }

    public class ParticipantDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string TicketCategory { get; set; } = null!;
    }
    public class CustomerData
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class Participant
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class BookingField
    {
        /// <summary>
        /// Alanın etiketi – örn: "First Name", "Gender", "Custom Field 1"
        /// </summary>
        public string Label { get; set; } = null!;

        /// <summary>
        /// Alan tipi – String, Boolean, Hidden, List
        /// </summary>
        public string FieldType { get; set; } = "String";  // Varsayılan: String

        /// <summary>
        /// Her katılımcı için bu alan zorunlu mu?
        /// </summary>
        public bool RequiredPerParticipant { get; set; }

        /// <summary>
        /// Tüm rezervasyon için bu alan zorunlu mu?
        /// </summary>
        public bool RequiredPerBooking { get; set; }

        /// <summary>
        /// Her katılımcı için bu alan görünür mü?
        /// </summary>
        public bool VisiblePerParticipant { get; set; }

        /// <summary>
        /// Rezervasyon seviyesinde bu alan görünür mü?
        /// </summary>
        public bool VisiblePerBooking { get; set; }

        /// <summary>
        /// Sadece List tipindeki alanlar için gerekli. Örn: "A\nB\nC"
        /// </summary>
        public string? ListOptions { get; set; }
    }
    public class RezdyProductDto
    {
        public string InternalCode { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public string BookingMode { get; set; } = "INVENTORY";
        public string ConfirmMode { get; set; } = "AUTOCONFIRM";
        public string ProductType { get; set; } = "ACTIVITY";
        public bool QuantityRequired { get; set; } = true;
        public int QuantityRequiredMin { get; set; } = 1;
        public int QuantityRequiredMax { get; set; } = 10;
        public string UnitLabel { get; set; } = "Passenger";
        public string UnitLabelPlural { get; set; } = "Passengers";
        public List<BookingField> BookingFields { get; set; }
        public List<PriceOptionDto> PriceOptions { get; set; }
        public List<ExtraDto> Extras { get; set; }
        public List<ImageItem> Images { get; set; }
        public List<PickupLocationDto> PickupLocations { get; set; }
        public List<string> Languages { get; set; }
        public LocationAddressDto LocationAddress { get; set; }
        public string BarcodeType { get; set; } = "QR_CODE";
        public string BarcodeOutputType { get; set; } = "PARTICIPANT";
        public RezdyConnectSettings RezdyConnectSettings { get; set; }

        public string ProductCode { get; internal set; }
        public string ExternalCode { get; internal set; }

        public decimal AdvertisedPrice { get; set; }
        public string Terms { get; set; }
        public string AdditionalInformation { get; set; }
        public List<TaxDto> Taxes { get; set; }

        public string Label { get; set; }
        public string SupplierId { get; set; }
    }

    public class ImageItem
    {
        public string ItemUrl { get; set; }
    }

    public class PriceOptionDto
    {
        public string Label { get; set; }
        public decimal Price { get; set; }
        public int SeatsUsed { get; set; } = 1;
    }

    public class ExtraDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExtraPriceType { get; set; } = "QUANTITY";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public ImageItem Image { get; set; }
    }

    public class PickupLocationDto
    {
        public string Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; }
        public int MinutesPrior { get; set; } = 60;
        public string PickupInstructions { get; set; }
    }

    public class RezdyConnectSettings
    {
        public string ExternalAvailabilityApi { get; set; }
        public string ExternalReservationApi { get; set; }
        public string ExternalBookingApi { get; set; }
        public string ExternalCancellationApi { get; set; }
    }
    public class BookingConfirmRequest
    {
        [Required]
        public string OrderNumber { get; set; }
    }

    public class BookingCancelRequest
    {
        [Required]
        public string OrderNumber { get; set; }
    }
    // Yardımcı adres DTO'su:
    public class LocationAddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
    }

    // Yardımcı Tax DTO'su:
    public class TaxDto
    {
        public string TaxFeeType { get; set; }
        public string Label { get; set; }
        public string TaxType { get; set; }
        public decimal TaxPercent { get; set; }
        public decimal TaxAmount { get; set; } = 0;
        public bool PriceInclusive { get; set; }
        public bool Compound { get; set; }
    }
    public class RezdyBookingConfirmRequest
    {
        [Required]
        public string OrderNumber { get; set; }

        [Required]
        public string Status { get; set; }  // CONFIRMED

        [Required]
        public CustomerDto Customer { get; set; }

        [Required]
        public List<ParticipantDto> Participants { get; set; }

        [Required]
        public string ProductCode { get; set; }

        [Required]
        public string Currency { get; set; }

        public decimal TotalAmount { get; set; }

        public string Comments { get; set; }
        public List<BookingField> Fields { get; set; }
        public string BarcodeType { get; set; } = "QR_CODE";

        // Diğer isteğe bağlı alanları da eklersiniz
    }


}
