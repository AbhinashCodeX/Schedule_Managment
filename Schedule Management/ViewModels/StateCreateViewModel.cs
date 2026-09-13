using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class StateCreateViewModel
    {
        [Required]
        public int CountryId { get; set; }

        [Required]
        [StringLength(200)]
        public string StateName { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string StateCode { get; set; } = string.Empty;
    }
}
