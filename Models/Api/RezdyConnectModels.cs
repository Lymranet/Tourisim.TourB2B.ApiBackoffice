namespace TourManagementApi.Models.Api.RezdyConnectModels
{
    public class ProductModel
    {
        public string ProductCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DurationMinutes { get; set; }
        public ProductLocation Location { get; set; }
        public List<ProductImage> Images { get; set; }
        public List<ProductOption> Options { get; set; }
    }

    public class ProductLocation
    {
        public string Address { get; set; }
        public GeoCoordinates Geo { get; set; }
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

    public class AvailabilityResponse
    {
        public string ProductCode { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public int RemainingPlaces { get; set; }
    }
    public partial class BookingRequest
    {
        public string OptionCode { get; set; }
        public DateTime StartTimeLocal { get; set; }
        public int Quantity { get; set; }
        public CustomerData Customer { get; set; }
        public List<Participant> Participants { get; set; }
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
    public class BookingDto
    {
        public string OrderNumber { get; set; } = null!;
        public string SupplierId { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EUR";

        public CustomerDto Customer { get; set; } = new();
        public List<ParticipantDto> Participants { get; set; } = new();
    }

    public class CustomerDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
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

}
