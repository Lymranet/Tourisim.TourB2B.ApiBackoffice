using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityMeetingPointsViewModel
    {
        public int ActivityId { get; set; }
        public List<MeetingPointViewModel> MeetingPoints { get; set; } = new();
    }

    public class MeetingPointViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string Latitude { get; set; } = string.Empty;
        [Required]
        public string Longitude { get; set; } = string.Empty;
    }
} 