using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("Iso2", Name = "UQ_Countries_ISO2", IsUnique = true)]
public partial class Country
{
    [Key]
    public int CountryId { get; set; }

    [StringLength(150)]
    public string CountryName { get; set; } = null!;

    [Column("ISO2")]
    [StringLength(2)]
    [Unicode(false)]
    public string Iso2 { get; set; } = null!;

    [Column("ISO3")]
    [StringLength(3)]
    [Unicode(false)]
    public string? Iso3 { get; set; }

    [StringLength(20)]
    public string? PhoneCode { get; set; }

    public int? GeoNameId { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [InverseProperty("Country")]
    public virtual ICollection<State> States { get; set; } = new List<State>();
}
