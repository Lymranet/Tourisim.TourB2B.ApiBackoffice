using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class CancellationPolicyCondition
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public int MinDurationBeforeStartTimeSec { get; set; }

    public int RefundPercentage { get; set; }

    public bool IsFreeCancellation { get; set; }

    public virtual Activity Activity { get; set; } = null!;
}
