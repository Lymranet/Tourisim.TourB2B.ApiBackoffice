using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingParticipantField
{
    public long FieldId { get; set; }

    public long? ParticipantId { get; set; }

    public string? Label { get; set; }

    public string? Value { get; set; }

    public string? BarcodeType { get; set; }

    public virtual BookingParticipant? Participant { get; set; }
}
