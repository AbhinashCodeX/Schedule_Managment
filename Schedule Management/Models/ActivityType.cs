using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class ActivityType
{
    public int ActivityTypeId { get; set; }

    public string ActivityName { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<CoachAvailability> CoachAvailabilities { get; set; } = new List<CoachAvailability>();
}
