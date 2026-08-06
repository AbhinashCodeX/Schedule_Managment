using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("CountryId", Name = "IX_States_CountryId")]
[Index("CountryId", "StateCode", Name = "UQ_States_Country_StateCode", IsUnique = true)]
public partial class State
{
    [Key]
    public int StateId { get; set; }

    public int CountryId { get; set; }

    [StringLength(200)]
    public string StateName { get; set; } = null!;

    [StringLength(30)]
    public string StateCode { get; set; } = null!;

    public int? GeoNameId { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [ForeignKey("CountryId")]
    [InverseProperty("States")]
    public virtual Country Country { get; set; } = null!;

    [InverseProperty("State")]
    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
