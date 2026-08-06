using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class ActivityTypeViewModel
    {
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "Activity name is required.")]
        [StringLength(
           100,
           MinimumLength = 2,
           ErrorMessage = "Activity name must be between 2 and 100 characters."
       )]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedOn { get; set; }
    }
}
