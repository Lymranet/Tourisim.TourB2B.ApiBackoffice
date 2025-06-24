using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class RoutePoint
{
    public int ActivityId { get; set; }

    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Latitude { get; set; } = null!;

    public string Longitude { get; set; } = null!;

    public virtual Activity Activity { get; set; } = null!;
}
