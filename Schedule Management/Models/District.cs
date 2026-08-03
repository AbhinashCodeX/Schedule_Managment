using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class District
{
    public int DistrictId { get; set; }

    public int StateId { get; set; }

    public string DistrictName { get; set; } = null!;

    public string DistrictCode { get; set; } = null!;

    public int? GeoNameId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual State State { get; set; } = null!;
}
