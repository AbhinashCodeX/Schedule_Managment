using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class CreateCoachAvailabilityViewModel
    {
        [Required(ErrorMessage = "Activity Type is required.")]
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "From Date is required.")]
        public DateOnly FromDate { get; set; }

        [Required(ErrorMessage = "To Date is required.")]
        public DateOnly ToDate { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required.")]
        public TimeOnly EndTime { get; set; }
    }
}
