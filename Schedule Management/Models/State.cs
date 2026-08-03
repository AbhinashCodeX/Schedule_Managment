using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class State
{
    public int StateId { get; set; }

    public int CountryId { get; set; }

    public string StateName { get; set; } = null!;

    public string StateCode { get; set; } = null!;

    public int? GeoNameId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
