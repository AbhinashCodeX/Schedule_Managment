using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class CoachAvailabilityViewModel
    {
        public int AvailabilityId { get; set; }

        public int ActivityTypeId { get; set; }

        public string? ActivityName { get; set; }

        [Required(ErrorMessage = "Available Date is required.")]
        public DateOnly AvailableDate { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required.")]
        public TimeOnly EndTime { get; set; }

        public bool IsBooked { get; set; }

        public bool IsActive { get; set; }
    }
}
