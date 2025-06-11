using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityLocationViewModel
    {
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Adres zorunludur.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Şehir zorunludur.")]
        public string? City { get; set; }

        [Required(ErrorMessage = "Ülke zorunludur.")]
        public string? Country { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
} 