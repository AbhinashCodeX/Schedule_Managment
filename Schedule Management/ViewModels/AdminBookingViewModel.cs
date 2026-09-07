namespace Schedule_Management.ViewModels
{
    public class AdminBookingViewModel
    {
        public int BookingId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string CoachName { get; set; } = string.Empty;

        public string ActivityName { get; set; } = string.Empty;

        public DateOnly BookingDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string BookingStatus { get; set; } = string.Empty;

        public DateTime BookedOn { get; set; }

        public bool IsActive { get; set; }
    }
}
