using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("ActivityName", Name = "UQ_ActivityTypes_ActivityName", IsUnique = true)]
public partial class ActivityType
{
    [Key]
    public int ActivityTypeId { get; set; }

    [StringLength(100)]
    public string ActivityName { get; set; } = null!;

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [InverseProperty("ActivityType")]
    public virtual ICollection<CoachAvailability> CoachAvailabilities { get; set; } = new List<CoachAvailability>();
}
