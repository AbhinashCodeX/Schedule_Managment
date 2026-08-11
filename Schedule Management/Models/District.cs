using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("StateId", Name = "IX_Districts_StateId")]
[Index("StateId", "DistrictCode", Name = "UQ_Districts_State_DistrictCode", IsUnique = true)]
public partial class    District
{
    [Key]
    public int DistrictId { get; set; }

    public int StateId { get; set; }

    [StringLength(200)]
    public string DistrictName { get; set; } = null!;

    [StringLength(100)]
    public string DistrictCode { get; set; } = null!;

    public int? GeoNameId { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [ForeignKey("StateId")]
    [InverseProperty("Districts")]
    public virtual State State { get; set; } = null!;

    [InverseProperty("District")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
