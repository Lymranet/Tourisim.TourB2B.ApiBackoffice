using Microsoft.AspNetCore.Mvc.Rendering;
namespace TourManagementApi.Models.ViewModels
{
    public class CreateAvailabilityViewModel
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int OptionId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public int AvailableCapacity { get; set; }
        public int MaximumCapacity { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<SelectListItem> OptionList { get; set; } = new();
        public List<TicketCategoryInputModel> TicketCategories { get; set; } = new();
    }

    public class TicketCategoryInputModel
    {
        public int TicketCategoryId { get; set; }
        public string Name { get; set; } = "";
        public int? Capacity { get; set; }
    }
}