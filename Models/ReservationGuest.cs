namespace TourManagementApi.Models
{
    public class ReservationGuest
    {
        public int Id { get; set; }

        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }

        public string GuestName { get; set; }
        public string GuestType { get; set; } // Adult, Child, Infant
        public int Age { get; set; }
    }

}
