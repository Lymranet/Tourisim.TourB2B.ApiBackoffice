namespace TourManagementApi.Models.Api
{
    namespace TourManagementApi.Models.Api.Rezdy
    {
        public class RezdyBookingResponse
        {
            public List<BookingResponseBooking> Bookings { get; set; }
        }

        public class BookingResponseBooking
        {
            public string BarcodeType { get; set; } // QR_CODE, etc.
            public string Comments { get; set; }
            public List<BookingRequestField> Fields { get; set; }
            public List<BookingResponseItem> Items { get; set; }
            public string OrderNumber { get; set; }
            public string ResellerComments { get; set; }
            public RequestStatus RequestStatus { get; set; }
        }

        public class BookingResponseItem
        {
            public List<BookingResponseParticipant> Participants { get; set; }
        }

        public class BookingResponseParticipant
        {
            public List<BookingRequestField> Fields { get; set; }
        }

        public class BookingRequestField
        {
            public string BarcodeType { get; set; } // QR_CODE, TEXT, etc.
            public string Label { get; set; }
            public string Value { get; set; }
        }

        public class RequestStatus
        {
            public string Code { get; set; }       // e.g., "SUCCESS"
            public string Message { get; set; }    // e.g., "Booking created"
            public int? HttpStatus { get; set; }   // e.g., 200
        }
    }

}
