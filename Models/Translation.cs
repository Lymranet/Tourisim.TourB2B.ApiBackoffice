using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class Translation
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public string Language { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Label { get; set; } = null!;

    public string? Highlights { get; set; }

    public string? Itinerary { get; set; }

    public string InclusionsJson { get; set; } = null!;

    public string ExclusionsJson { get; set; } = null!;

    public string ImportantInfoJson { get; set; } = null!;
}
