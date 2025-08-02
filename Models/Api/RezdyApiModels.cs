using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TourManagementApi.Models.Api.Rezdy
{
    /// <summary>
    /// All possible error codes returned in Rezdy’s requestStatus.error.errorCode
    /// </summary>
    public enum RezdyErrorCode
    {
        UNKNOWN = 1,
        NO_IMPLEMENTATION = 2,
        BIZ_RULE = 3,
        AUTHENTICATION = 4,
        AUTHENTICATION_TIMEOUT = 5,
        AUTHORIZATION = 6,
        PROTOCOL_VIOLATION = 7,
        TRANSACTION_MODEL = 8,
        AUTHENTICAL_MODEL = 9,
        MISSING_FIELD = 10,
        AUTOMATED_PAYMENTS_PRICE_TOO_LOW_TO_COVER_FEES = 11,
        AUTOMATED_PAYMENTS_PRICE_TOO_LOW_FOR_AGENT_COMMISSION = 12,
        PAYMENT_CARD_TYPE_DECLINED = 13,
        PAYMENT_CARD_TYPE_REJECTED = 14,
        PAYMENT_ERROR = 15,
        SINGLE_ORDER_LINE_ITEM_ALLOWED = 16,
        PAYMENT_REQUIRED_SCA_ACTION = 17,
        MINIMUM_QUANTITY_REQUIRED = 18,
        MAXIMUM_QUANTITY_REACHED = 19,
        DOWNSTREAM_ERROR = 20,
        NOT_ENOUGH_SEATS_AVAILABLE = 21,
        SESSION_NOT_FOUND = 22,
        PRODUCT_NOT_FOUND = 23,
        ORDER_NOT_FOUND = 24,
        ORDER_LINE_ITEM_NOT_FOUND = 25,
        MIN_BOOK_AHEAD_START_TIME = 26,
        MIN_BOOK_AHEAD_END_TIME = 27,
        INVALID_CUSTOMER_EMAIL = 28,
        BLOCKED_EMAIL = 29,
        DOWNSTREAM_TIMEOUT = 30,
        INTERNAL_ERROR = 31,
        INVALID_PRICE_OPTION = 32,
        REZDYCONNECT_EXTERNAL_SYSTEM = 33,
        OTHER_OPERATION_IN_PROGRESS = 34
    }
    public class CustomerDto
    {
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        // Eğer email zorunluysa ekleyin:
        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }
    }
    public class BookingItemDto
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("startTimeLocal")]
        public string StartTimeLocal { get; set; }

        [JsonProperty("endTimeLocal", NullValueHandling = NullValueHandling.Ignore)]
        public string EndTimeLocal { get; set; }

        [JsonProperty("quantities")]
        public QuantityDto[] Quantities { get; set; }
    }
    public class QuantityDto
    {
        [JsonProperty("optionLabel")]
        public string OptionLabel { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
    public class PaymentDto
    {
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }       // e.g. "CASH" | "CARD" | "MANUAL"

        [JsonProperty("recipient")]
        public string Recipient { get; set; }  // e.g. "SUPPLIER"

        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
    }
    public class AvailabilityResponse
    {
        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("sessions")]
        public AvailabilitySession[] Sessions { get; set; }
    }
    public class AvailabilitySession
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("startTimeLocal")]
        public string StartTimeLocal { get; set; }

        [JsonProperty("endTimeLocal")]
        public string EndTimeLocal { get; set; }

        [JsonProperty("allDay")]
        public bool AllDay { get; set; }

        [JsonProperty("seats")]
        public int Seats { get; set; }

        [JsonProperty("seatsAvailable")]
        public int SeatsAvailable { get; set; }

        [JsonProperty("priceOptions")]
        public AvailabilityPriceOption[] PriceOptions { get; set; }
    }
    public class AvailabilityPriceOption
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("seatsUsed")]
        public int SeatsUsed { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }
    }
    public class RequestStatus
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("error")]
        public RezdyErrorDetail Error { get; set; }

        [JsonIgnore]
        public RezdyErrorCode? ErrorCodeEnum
        {
            get
            {
                if (Error?.ErrorCode == null) return null;
                return Enum.TryParse<RezdyErrorCode>(Error.ErrorCode, out var code)
                    ? code
                    : (RezdyErrorCode?)null;
            }
        }
    }
    public class RezdyErrorDetail
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
    public class PaginatedResponse<T>
    {
        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public PaginatedData<T> ResponseData { get; set; }
    }
    public class PaginatedData<T>
    {
        [JsonProperty("items")]
        public T[] Items { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
    public class ProductCreateRequest
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("priceOptions")]
        public PriceOption[] PriceOptions { get; set; }

        [JsonProperty("locations")]
        public Location[] Locations { get; set; }
    }
    public class BookingUpdateRequest
    {
        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("supplierId")]
        public int SupplierId { get; set; }

        [JsonProperty("supplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("supplierAlias")]
        public string SupplierAlias { get; set; }

        [JsonProperty("customer")]
        public CustomerUpdateDto Customer { get; set; }

        [JsonProperty("items")]
        public BookingUpdateItemDto[] Items { get; set; }

        [JsonProperty("totalAmount")]
        public decimal TotalAmount { get; set; }

        [JsonProperty("totalCurrency")]
        public string TotalCurrency { get; set; }

        [JsonProperty("totalPaid")]
        public decimal TotalPaid { get; set; }

        [JsonProperty("totalDue")]
        public decimal TotalDue { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("dateConfirmed")]
        public DateTime? DateConfirmed { get; set; }

        [JsonProperty("datePaid")]
        public DateTime? DatePaid { get; set; }

        [JsonProperty("payments")]
        public PaymentDetailDto[] Payments { get; set; }

        [JsonProperty("fields")]
        public BookingFieldUpdate[] Fields { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("vouchers")]
        public VoucherDto[] Vouchers { get; set; }

        [JsonProperty("barcodeType")]
        public string BarcodeType { get; set; }
    }
    public class CustomerUpdateDto
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        // Eğer gerekirse e-posta vb. eklenebilir
    }
    public class PickupLocationDto
    {
        [JsonProperty("locationName")]
        public string LocationName { get; set; }
    }
    public class QuantityDetailDto
    {
        [JsonProperty("optionLabel")]
        public string OptionLabel { get; set; }

        [JsonProperty("optionPrice")]
        public decimal OptionPrice { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }
    public class ExtraDto
    {
        // Example: { name, description, price, extraPriceType }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("extraPriceType")]
        public string ExtraPriceType { get; set; }
    }
    public class ParticipantDetailDto
    {
        [JsonProperty("fields")]
        public BookingFieldUpdate[] Fields { get; set; }
    }
    public class PaymentDetailDto
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }
    }
    public class VoucherDto
    {
        // Rezdy dokümanında voucher içeriği boş bir dizi olarak gelmiş
        // Varsa alanlar eklenebilir, yoksa basitçe:
        [JsonProperty("code")]
        public string Code { get; set; }
    }
    public class BookingUpdateItemDto
    {
        [JsonProperty("pickupLocation")]
        public PickupLocationDto PickupLocation { get; set; }

        [JsonProperty("productName")]
        public string ProductName { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("startTimeLocal")]
        public string StartTimeLocal { get; set; }

        [JsonProperty("endTimeLocal")]
        public string EndTimeLocal { get; set; }

        [JsonProperty("quantities")]
        public QuantityDetailDto[] Quantities { get; set; }

        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("extras")]
        public ExtraDto[] Extras { get; set; }

        [JsonProperty("participants")]
        public ParticipantDetailDto[] Participants { get; set; }

        [JsonProperty("subtotal")]
        public decimal Subtotal { get; set; }
    }
    public class BookingFieldUpdate
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
    public class ImageUploadResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public ImageUploadResponseData ResponseData { get; set; }
    }
    public class ImageUploadResponseData
    {
        [JsonProperty("imageId")]
        public int ImageId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
    public class Location
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
    public class ProductResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public ProductResponseData ResponseData { get; set; }
    }
    public class ProductResponseData
    {
        [JsonProperty("product")]
        public ProductDetail Product { get; set; }
    }
    public class ProductDetail
    {
        [JsonProperty("productType")]
        public string ProductType { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("internalCode")]
        public string InternalCode { get; set; }

        [JsonProperty("supplierId")]
        public int SupplierId { get; set; }

        [JsonProperty("supplierAlias")]
        public string SupplierAlias { get; set; }

        [JsonProperty("supplierName")]
        public string SupplierName { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("advertisedPrice")]
        public decimal AdvertisedPrice { get; set; }

        [JsonProperty("priceOptions")]
        public PriceOption[] PriceOptions { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("unitLabel")]
        public string UnitLabel { get; set; }

        [JsonProperty("unitLabelPlural")]
        public string UnitLabelPlural { get; set; }

        [JsonProperty("quantityRequired")]
        public bool QuantityRequired { get; set; }

        [JsonProperty("quantityRequiredMin")]
        public int QuantityRequiredMin { get; set; }

        [JsonProperty("quantityRequiredMax")]
        public int QuantityRequiredMax { get; set; }

        [JsonProperty("images")]
        public ImageDetail[] Images { get; set; }

        [JsonProperty("bookingMode")]
        public string BookingMode { get; set; }

        [JsonProperty("charter")]
        public bool Charter { get; set; }

        [JsonProperty("terms")]
        public string Terms { get; set; }

        [JsonProperty("extras")]
        public Extra[] Extras { get; set; }

        [JsonProperty("bookingFields")]
        public BookingFieldDefinition[] BookingFields { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("confirmMode")]
        public string ConfirmMode { get; set; }

        [JsonProperty("confirmModeMinParticipants")]
        public int ConfirmModeMinParticipants { get; set; }

        [JsonProperty("commissionIncludesExtras")]
        public bool CommissionIncludesExtras { get; set; }

        [JsonProperty("cancellationPolicyDays")]
        public int CancellationPolicyDays { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("minimumNoticeMinutes")]
        public int MinimumNoticeMinutes { get; set; }

        [JsonProperty("durationMinutes")]
        public int DurationMinutes { get; set; }

        [JsonProperty("dateUpdated")]
        public DateTime DateUpdated { get; set; }

        [JsonProperty("pickupId")]
        public int PickupId { get; set; }

        [JsonProperty("locationAddress")]
        public LocationAddress LocationAddress { get; set; }

        [JsonProperty("languages")]
        public string[] Languages { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("waitListingEnabled")]
        public bool WaitListingEnabled { get; set; }

        [JsonProperty("isApiBookingSupported")]
        public bool IsApiBookingSupported { get; set; }
    }
    public class PriceOption
    {
        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("seatsUsed")]
        public int SeatsUsed { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("priceType")]
        public string PriceType { get; set; }
    }
    public class ImageDetail
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("itemUrl")]
        public string ItemUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("mediumSizeUrl")]
        public string MediumSizeUrl { get; set; }

        [JsonProperty("largeSizeUrl")]
        public string LargeSizeUrl { get; set; }
    }
    public class Extra
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("extraPriceType")]
        public string ExtraPriceType { get; set; }
    }
    public class BookingFieldDefinition
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("requiredPerParticipant")]
        public bool RequiredPerParticipant { get; set; }

        [JsonProperty("requiredPerBooking")]
        public bool RequiredPerBooking { get; set; }

        [JsonProperty("visiblePerParticipant")]
        public bool VisiblePerParticipant { get; set; }

        [JsonProperty("visiblePerBooking")]
        public bool VisiblePerBooking { get; set; }

        [JsonProperty("fieldType")]
        public string FieldType { get; set; }

        [JsonProperty("listOptions")]
        public string ListOptions { get; set; }
    }
    public class LocationAddress
    {
        [JsonProperty("addressLine")]
        public string AddressLine { get; set; }

        [JsonProperty("postCode")]
        public string PostCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }
    public class BookingCreateRequest
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("sessionId")]
        public int SessionId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("fields")]
        public BookingFieldUpdate[] Fields { get; set; }

        [JsonProperty("participants")]
        public ParticipantUpdate[] Participants { get; set; }
    }
    public class BookingResponse
    {
        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public BookingResponseData ResponseData { get; set; }
    }
    public class BookingResponseData
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("booking")]
        public BookingDto Booking { get; set; }
    }
    public class BookingDto
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("sessionId")]
        public int SessionId { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("fields")]
        public BookingFieldUpdate[] Fields { get; set; }

        [JsonProperty("participants")]
        public ParticipantUpdate[] Participants { get; set; }
    }
    public class ParticipantUpdate
    {
        [JsonProperty("participantIndex")]
        public int ParticipantIndex { get; set; }

        [JsonProperty("fields")]
        public BookingFieldUpdate[] Fields { get; set; }
    }
    public class CancelBookingResponse
    {
        [JsonProperty("requestStatus")]
        public RequestStatus RequestStatus { get; set; }

        [JsonProperty("responseData")]
        public object ResponseData { get; set; }
    }
    public class RateLimitExceededException : Exception
    {
        public RateLimitExceededException(string message) : base(message) { }
    }
    public class RezdyApiException : Exception
    {
        public RezdyErrorCode ErrorCode { get; }

        public RezdyApiException(string message, RezdyErrorCode errorCode)
            : base($"Rezdy API Error ({errorCode}): {message}")
        {
            ErrorCode = errorCode;
        }
    }
    public class AvailabilityRequest
    {
        [JsonProperty("productCode")]
        public string ProductCode { get; set; }

        [JsonProperty("sessions")]
        public Session[] Sessions { get; set; }
    }
    public class Session
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}
