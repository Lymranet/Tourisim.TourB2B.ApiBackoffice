namespace TourManagementApi.Models
{
    public class ReservationGuest
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public string GuestName { get; set; }
        public string GuestType { get; set; }  // Örn: "Adult", "Child", "Infant"
        public int Age { get; set; }

        // Entegrasyon Detayları ekliyorum
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Occupancy { get; set; }
        public string TicketId { get; set; }
        public string TicketCategory { get; set; }

        public string AdditionalFieldsJson { get; set; } // JSON.stringify
        public string AddonsJson { get; set; }
    }


}
