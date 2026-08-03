using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class CoachAvailability
{
    public int AvailabilityId { get; set; }

    public int CoachId { get; set; }

    public int ActivityTypeId { get; set; }

    public DateOnly AvailableDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool IsBooked { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public virtual ActivityType ActivityType { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual User Coach { get; set; } = null!;
}
