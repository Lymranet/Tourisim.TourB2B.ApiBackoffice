using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class AddonTranslation
{
    public int Id { get; set; }

    public int AddonId { get; set; }

    public string Language { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public virtual Addon Addon { get; set; } = null!;
}
