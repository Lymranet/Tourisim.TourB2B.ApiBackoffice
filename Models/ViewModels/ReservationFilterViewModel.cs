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

}
