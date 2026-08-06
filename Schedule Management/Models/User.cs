using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Schedule_Management.Models;

[Index("RoleId", Name = "IX_Users_RoleId")]
[Index("Email", Name = "UQ_Users_Email", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public int? DistrictId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(500)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(500)]
    public string? FullAddress { get; set; }

    public bool IsActive { get; set; }

    [Precision(0)]
    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    [Precision(0)]
    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Coach")]
    public virtual ICollection<CoachAvailability> CoachAvailabilities { get; set; } = new List<CoachAvailability>();

    [ForeignKey("DistrictId")]
    [InverseProperty("Users")]
    public virtual District? District { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
