namespace TourManagementApi.Models.Api
{
    public class ExperienceBankBookingRequest
    {
        public BookingData Data { get; set; }
    }

    public class BookingData
    {
        public string BookingId { get; set; }
        public string ReservationReference { get; set; }
        public List<BookingItem> BookingItems { get; set; }
        public MarketplaceInfo Marketplace { get; set; }
        public ContactInfo Contact { get; set; }
        public PaymentInfo Payment { get; set; }
        public string Notes { get; set; }
        public string Language { get; set; }
    }

    public class BookingItem
    {
        public string Date { get; set; }
        public string OptionId { get; set; }
        public List<GuestInfo> Guests { get; set; }
        public List<Addon> Addons { get; set; }
    }

    public class GuestInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Occupancy { get; set; }
        public string TicketId { get; set; }
        public string TicketCategory { get; set; }
        public List<AdditionalField> AdditionalFields { get; set; }
        public List<Addon> Addons { get; set; }
    }

    public class AdditionalField
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Addon
    {
        public string Id { get; set; }
        public int Quantity { get; set; }
    }

    public class MarketplaceInfo
    {
        public string Id { get; set; }
        public string BookingId { get; set; }
    }

    public class ContactInfo
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class PaymentInfo
    {
        public string Amount { get; set; }
        public string Currency { get; set; }
    }

}
