namespace Schedule_Management.ViewModels
{
    public class UserListViewModel
    {

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? DistrictName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
