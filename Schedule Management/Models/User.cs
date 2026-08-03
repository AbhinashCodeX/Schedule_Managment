using System;
using System.Collections.Generic;

namespace Schedule_Management.Models;

public partial class User
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public int? DistrictId { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string? FullAddress { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CoachAvailability> CoachAvailabilities { get; set; } = new List<CoachAvailability>();

    public virtual Role Role { get; set; } = null!;
}
