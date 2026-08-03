using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int AvailabilityId { get; set; }

    public string BookingStatus { get; set; } = null!;

    public DateTime BookedOn { get; set; }

    public DateTime? CancelledOn { get; set; }

    public string? CancellationReason { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual CoachAvailability Availability { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
