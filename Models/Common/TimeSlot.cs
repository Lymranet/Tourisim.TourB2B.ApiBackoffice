using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Models.Common;

[Owned]
public class TimeSlot
{
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public List<string> DaysOfWeek { get; set; } = new();
} 