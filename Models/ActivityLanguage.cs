using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class ActivityLanguage
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public string LanguageCode { get; set; } = null!;

    public virtual Activity Activity { get; set; } = null!;
}
