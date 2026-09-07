namespace Schedule_Management.ViewModels
{
    public class DistrictViewModel
    {
        public int DistrictId { get; set; }

        public int StateId { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; } = string.Empty;

        public string StateName { get; set; } = string.Empty;

        public string DistrictName { get; set; } = string.Empty;

        public string DistrictCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
