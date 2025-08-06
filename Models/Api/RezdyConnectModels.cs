using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.Api.RezdyConnectModels
{
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

        public List<PriceOptionLite> PriceOptions { get; set; }
    }
    public class PriceOptionLite
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
        public List<BookingFieldDto>? BookingFields { get; set; }
    }
    public class RezdyConnectSettings
    {
        public string ExternalAvailabilityApi { get; set; }
        public string ExternalReservationApi { get; set; }
        public string ExternalBookingApi { get; set; }
        public string ExternalCancellationApi { get; set; }
    }
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
    public class BookingFieldDto
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
        public string Value { get; set; }  // örn: kullanıcının verdiği cevap
        /// <summary>
        /// Rezervasyon seviyesinde bu alan görünür mü?
        /// </summary>
        public bool VisiblePerBooking { get; set; }

        /// <summary>
        /// Sadece List tipindeki alanlar için gerekli. Örn: "A\nB\nC"
        /// </summary>
        public string? ListOptions { get; set; }

    }
    public class PriceOption
    {
        public string Label { get; set; }
        public decimal Price { get; set; }
        public int SeatsUsed { get; set; } = 1;

        // Yeni alanlar
        public int? MinQuantity { get; set; }
        public int? MaxQuantity { get; set; }
        public string PriceGroupType { get; set; } // "EACH" veya "TOTAL"
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
    public class ImageItem
    {
        public string ItemUrl { get; set; }
    }
    public class PickupLocationDto
    {
        public string Address { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string LocationName { get; set; } = string.Empty;
        public int MinutesPrior { get; set; } = 60;
        public string PickupInstructions { get; set; } = string.Empty;
        public string PickupTime { get; set; } = string.Empty;
    }
    public class LocationAddressDto
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string PostCode { get; set; }
        public string State { get; set; }
    }
}
