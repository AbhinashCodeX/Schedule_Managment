using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class CountryEditViewModel
    {
        [Required]
        public int CountryId { get; set; }

        [Required]
        [StringLength(150)]
        public string CountryName { get; set; } = string.Empty;

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Iso2 { get; set; } = string.Empty;

        [StringLength(3)]
        public string? Iso3 { get; set; }

        [StringLength(20)]
        public string? PhoneCode { get; set; }
    }
}
