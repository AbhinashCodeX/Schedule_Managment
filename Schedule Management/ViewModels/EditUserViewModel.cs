using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class EditUserViewModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        public string? FullAddress { get; set; }

        public int? DistrictId { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }
}
