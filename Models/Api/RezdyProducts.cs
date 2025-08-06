using TourManagementApi.Models.Api.RezdyConnectModels;

namespace TourManagementApi.Models.Api
{
    public class RezdyProducts
    {
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
        public string BarcodeType { get; set; } = "QR_CODE";
        public string BarcodeOutputType { get; set; } = "PARTICIPANT";
        public string ProductCode { get; internal set; }
        public string ExternalCode { get; internal set; }
        public decimal AdvertisedPrice { get; set; }
        public string Terms { get; set; }
        public string AdditionalInformation { get; set; }
        public string Label { get; set; }
        public string SupplierId { get; set; }
        public List<string> Languages { get; set; }
        public RezdyConnectSettings RezdyConnectSettings { get; set; }
        public List<TaxDto> Taxes { get; set; }
        public List<BookingFieldDto> BookingFields { get; set; }
        public List<PriceOption> PriceOptions { get; set; }
        public List<ExtraDto> Extras { get; set; }
        public List<ImageItem> Images { get; set; }
        public List<PickupLocationDto> PickupLocations { get; set; }
        public LocationAddressDto LocationAddress { get; set; }
    }
}
