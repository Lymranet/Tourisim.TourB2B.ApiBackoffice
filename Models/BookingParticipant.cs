using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class BookingParticipant
{
    public long ParticipantId { get; set; }

    public long? ItemId { get; set; }

    public virtual ICollection<BookingParticipantField> BookingParticipantFields { get; set; } = new List<BookingParticipantField>();

    public virtual BookingItem? Item { get; set; }
}
