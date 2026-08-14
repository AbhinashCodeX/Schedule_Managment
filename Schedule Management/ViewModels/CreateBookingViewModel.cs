using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class CreateBookingViewModel
    {
        [Required]
        public int ActivityTypeId { get; set; }

        [Required]
        public int CoachId { get; set; }

        [Required]
        public DateOnly BookingDate { get; set; }

        [Required]
        public int AvailabilityId { get; set; }
    }
}
