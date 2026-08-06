using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("AvailabilityId", "BookingStatus", Name = "IX_Bookings_AvailabilityId")]
[Index("UserId", "BookingStatus", Name = "IX_Bookings_UserId")]
public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int AvailabilityId { get; set; }

    [StringLength(20)]
    public string BookingStatus { get; set; } = null!;

    [Precision(0)]
    public DateTime BookedOn { get; set; }

    [Precision(0)]
    public DateTime? CancelledOn { get; set; }

    [StringLength(300)]
    public string? CancellationReason { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [ForeignKey("AvailabilityId")]
    [InverseProperty("Bookings")]
    public virtual CoachAvailability Availability { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User User { get; set; } = null!;
}
