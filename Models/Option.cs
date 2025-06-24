using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementApi.Models
{
    public class Option
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Activity")]
        public int ActivityId { get; set; }
        public Activity? Activity { get; set; }

        [Required(ErrorMessage = "Seçenek adı zorunludur")]
        [Display(Name = "Seçenek Adı")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Trekksoft uyumlu yeni alanlar
        public string Duration { get; set; } = string.Empty; // ISO 8601 format (PT3H gibi)
        
        [Required(ErrorMessage = "Başlangıç saati zorunludur")]
        [Display(Name = "Başlangıç Saati")]
        public string StartTime { get; set; } = string.Empty;

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
        
        public List<OpeningHour> OpeningHours { get; set; } = new();
        public List<TicketCategory> TicketCategories { get; set; } = new();
    }
} 