using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class OpeningHour
{
    public int Id { get; set; }

    public string FromTime { get; set; } = null!;

    public string ToTime { get; set; } = null!;

    public int OptionId { get; set; }

    public virtual Option Option { get; set; } = null!;
}
