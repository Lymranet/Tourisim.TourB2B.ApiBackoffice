namespace TourManagementApi.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string ExperienceBankTicketId { get; set; }
        public string InternalTicketId { get; set; }
        public string TicketCode { get; set; }
        public string TicketCodeType { get; set; }
    }

}
