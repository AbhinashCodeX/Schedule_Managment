using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class EditCoachAvailabilityViewModel
    {
        public int AvailabilityId { get; set; }

        [Required]
        public int ActivityTypeId { get; set; }

        [Required]
        public DateOnly AvailableDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }
    }
}
