using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class Country
{
    public int CountryId { get; set; }

    public string CountryName { get; set; } = null!;

    public string Iso2 { get; set; } = null!;

    public string? Iso3 { get; set; }

    public string? PhoneCode { get; set; }

    public int? GeoNameId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
