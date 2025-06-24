using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class TimeSlot
{
    public int ActivityId { get; set; }

    public int Id { get; set; }

    public string StartTime { get; set; } = null!;

    public string EndTime { get; set; } = null!;

    public string DaysOfWeek { get; set; } = null!;

    public virtual Activity Activity { get; set; } = null!;
}
