namespace Schedule_Management.ViewModels
{
    public class MyBookingViewModel
    {
        public int BookingId { get; set; }

        public string? ActivityName { get; set; }

        public string? CoachName { get; set; }

        public DateOnly BookingDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public string? BookingStatus { get; set; }

        public bool IsActive { get; set; }
    }
}
