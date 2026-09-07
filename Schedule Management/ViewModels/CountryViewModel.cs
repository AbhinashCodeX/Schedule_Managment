namespace Schedule_Management.ViewModels
{
    public class CountryViewModel
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; } = string.Empty;

        public string Iso2 { get; set; } = string.Empty;

        public string? Iso3 { get; set; }

        public string? PhoneCode { get; set; }

        public bool IsActive { get; set; }
    }
}
