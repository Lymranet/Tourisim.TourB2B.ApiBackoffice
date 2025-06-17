using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityLocationViewModel
    {
        public int ActivityId { get; set; }

        [Display(Name = "Buluşma Noktaları")]
        public List<MeetingPointViewModel> MeetingPoints { get; set; } = new();

        [Display(Name = "Rota Noktaları")]
        public List<RoutePointViewModel> RoutePoints { get; set; } = new();
    }

    public class MeetingPointViewModel
    {
        [Required(ErrorMessage = "Buluşma noktası adı zorunludur")]
        [Display(Name = "Buluşma Noktası Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enlem bilgisi zorunludur")]
        [Display(Name = "Enlem")]
        public string Latitude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Boylam bilgisi zorunludur")]
        [Display(Name = "Boylam")]
        public string Longitude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres bilgisi zorunludur")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;
    }

    public class RoutePointViewModel
    {
        [Required(ErrorMessage = "Rota noktası adı zorunludur")]
        [Display(Name = "Rota Noktası Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enlem bilgisi zorunludur")]
        [Display(Name = "Enlem")]
        public string Latitude { get; set; } = string.Empty;

        [Required(ErrorMessage = "Boylam bilgisi zorunludur")]
        [Display(Name = "Boylam")]
        public string Longitude { get; set; } = string.Empty;
    }
} 