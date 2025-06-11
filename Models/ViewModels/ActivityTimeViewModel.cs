using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityTimeViewModel
    {
        public int ActivityId { get; set; }
        [Required]
        public int Duration { get; set; } // dakika
        public SeasonalAvailabilityViewModel SeasonalAvailability { get; set; } = new();
        public List<TimeSlotViewModel> TimeSlots { get; set; } = new();
    }

    public class SeasonalAvailabilityViewModel
    {
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }

    public class TimeSlotViewModel
    {
        [Required]
        public string StartTime { get; set; } = string.Empty; // "HH:mm"
        [Required]
        public string EndTime { get; set; } = string.Empty; // "HH:mm"
        public List<string> DaysOfWeek { get; set; } = new();
    }
} 