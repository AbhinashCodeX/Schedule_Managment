namespace Schedule_Management.ViewModels
{
    public class StateViewModel
    {
        public int StateId { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; } = string.Empty;

        public string StateName { get; set; } = string.Empty;

        public string StateCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
