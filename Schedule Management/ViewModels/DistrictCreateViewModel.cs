using System.ComponentModel.DataAnnotations;

namespace Schedule_Management.ViewModels
{
    public class DistrictCreateViewModel
    {
        [Required]
        public int StateId { get; set; }

        [Required]
        [StringLength(200)]
        public string DistrictName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DistrictCode { get; set; } = string.Empty;
    }
}
