using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.ViewModels
{
    public class OptionViewModel
    {
        public int? Id { get; set; }
        public int ActivityId { get; set; }
        
        [Required(ErrorMessage = "Seçenek adı zorunludur.")]
        [MaxLength(200)]
        [Display(Name = "Seçenek Adı")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [MaxLength(50)]
        [Display(Name = "Başlangıç Saati")]
        public string StartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlangıç saati zorunludur.")]
        [Display(Name = "Bitiş Saati")]
        [MaxLength(50)]
        public string? EndTime { get; set; }

        [Required(ErrorMessage = "Satış başlangıç tarihi zorunludur")]
        [Display(Name = "Satış Başlangıç Tarihi")]
        public DateTime FromDate { get; set; }
        
        [Required(ErrorMessage = "Satış bitiş tarihi zorunludur")]
        [Display(Name = "Satış Bitiş Tarihi")]
        public DateTime UntilDate { get; set; }
        
        [Required(ErrorMessage = "Rezervasyon kapanış süresi zorunludur")]
        [Display(Name = "Rezervasyon Kapanış Süresi (dakika)")]
        public int CutOff { get; set; }
        
        [Display(Name = "Başlangıç Saatinden Sonra Rezerve Edilebilir")]
        public bool CanBeBookedAfterStartTime { get; set; }
        
        [Display(Name = "Haftanın Günleri")]
        public List<string> Weekdays { get; set; } = new();
        
        [Display(Name = "Açılış Saatleri")]
        public List<OpeningHourViewModel> OpeningHours { get; set; } = new();
        
        [Display(Name = "Bilet Kategorileri")]
        public List<TicketCategoryViewModel> TicketCategories { get; set; } = new();
    }

    public class OpeningHourViewModel
    {
        public int? Id { get; set; }
        
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [MaxLength(50)]
        [Display(Name = "Başlangıç")]
        public string FromTime { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Bitiş saati zorunludur")]
        [MaxLength(50)]
        [Display(Name = "Bitiş")]
        public string ToTime { get; set; } = string.Empty;
    }

    public class TicketCategoryViewModel
    {
        public int? Id { get; set; }
        
        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [MaxLength(200)]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;
        
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
        public string PriceType { get; set; } = string.Empty;
        
        [Display(Name = "Minimum Yaş")]
        public int? MinAge { get; set; }
        
        [Display(Name = "Maksimum Yaş")]
        public int? MaxAge { get; set; }
        
        [Required(ErrorMessage = "Kategori tipi zorunludur")]
        [MaxLength(50)]
        [Display(Name = "Kategori Tipi")]
        public string Type { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Kategori aktiflik durumu zorunludur")]
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; }
    }
} 