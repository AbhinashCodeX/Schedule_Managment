using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("ActivityTypeId", "CoachId", "AvailableDate", "IsBooked", "IsActive", Name = "IX_CoachAvailabilities_Search")]
[Index("CoachId", "ActivityTypeId", "AvailableDate", "StartTime", "EndTime", Name = "UQ_CoachAvailabilities_Schedule", IsUnique = true)]
public partial class CoachAvailability
{
    [Key]
    public int AvailabilityId { get; set; }

    public int CoachId { get; set; }

    public int ActivityTypeId { get; set; }

    public DateOnly AvailableDate { get; set; }

    [Precision(0)]
    public TimeOnly StartTime { get; set; }

    [Precision(0)]
    public TimeOnly EndTime { get; set; }

    public bool IsBooked { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("ActivityTypeId")]
    [InverseProperty("CoachAvailabilities")]
    public virtual ActivityType ActivityType { get; set; } = null!;

    [InverseProperty("Availability")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CoachId")]
    [InverseProperty("CoachAvailabilities")]
    public virtual User Coach { get; set; } = null!;
}
