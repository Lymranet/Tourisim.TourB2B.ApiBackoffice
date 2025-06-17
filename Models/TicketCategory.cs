using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models
{
    public class TicketCategory
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Minimum koltuk sayısı zorunludur")]
        [Display(Name = "Minimum Koltuk")]
        public int MinSeats { get; set; }
        
        [Required(ErrorMessage = "Maksimum koltuk sayısı zorunludur")]
        [Display(Name = "Maksimum Koltuk")]
        public int MaxSeats { get; set; }
        
        [Required(ErrorMessage = "Fiyat zorunludur")]
        [Display(Name = "Fiyat")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Para birimi zorunludur")]
        [Display(Name = "Para Birimi")]
        public string Currency { get; set; } = "TRY";
        
        [Required(ErrorMessage = "Fiyat tipi zorunludur")]
        [Display(Name = "Fiyat Tipi")]
        public string PriceType { get; set; } = "Fixed";
        
        [Required(ErrorMessage = "Bilet tipi zorunludur")]
        [Display(Name = "Bilet Tipi")]
        public string Type { get; set; } = "Adult";
        
        [Display(Name = "Minimum Yaş")]
        public int? MinAge { get; set; }
        
        [Display(Name = "Maksimum Yaş")]
        public int? MaxAge { get; set; }
        
        public int OptionId { get; set; }
        public Option Option { get; set; } = null!;
    }
} 