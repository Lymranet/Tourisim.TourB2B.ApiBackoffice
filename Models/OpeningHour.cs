using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models
{
    public class OpeningHour
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [Display(Name = "Başlangıç Saati")]
        public string FromTime { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        [Display(Name = "Bitiş Saati")]
        public string ToTime { get; set; } = string.Empty;
        
        public int OptionId { get; set; }
        public Option Option { get; set; } = null!;
    }
} 