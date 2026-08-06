using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } =
            string.Empty;

        [Required]
        public string RegisterAs { get; set; } =
            string.Empty;

        [Required(ErrorMessage = "Please select a district.")]
        public int? DistrictId { get; set; }

        [Required(ErrorMessage = "Full address is required.")]
        [StringLength(
            500,
            ErrorMessage = "Address cannot exceed 500 characters."
        )]
        public string FullAddress { get; set; } = string.Empty;
    }
}
