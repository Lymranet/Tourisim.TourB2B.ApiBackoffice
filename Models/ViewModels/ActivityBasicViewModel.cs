using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityBasicViewModel
    {
        public int? ActivityId { get; set; } // Ana Activity kaydına referans

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Kategori zorunludur.")]
        [MaxLength(100)]
        public string? Category { get; set; }

        [Required(ErrorMessage = "Alt kategori zorunludur.")]
        [MaxLength(100)]
        public string? Subcategory { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Durum seçilmelidir.")]
        [MaxLength(50)]
        public string? Status { get; set; }

        [Required(ErrorMessage = "En az bir dil seçilmelidir.")]
        public List<string>? Languages { get; set; }

        // İletişim bilgileri
        [Required]
        public ContactInfoViewModel ContactInfo { get; set; } = new();

        public IFormFile? CoverImage { get; set; }
        public IFormFile? PreviewImage { get; set; }
        public List<IFormFile>? GalleryImages { get; set; }
        public List<string>? VideoUrls { get; set; }

        // Mevcut görsellerin yolları (readonly, edit için)
        public string? CoverImageUrl { get; set; }
        public string? PreviewImageUrl { get; set; }
        public List<string>? GalleryImageUrls { get; set; }
    }

    public class ContactInfoViewModel
    {
        [Required(ErrorMessage = "İletişim ismi zorunludur.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "İletişim rolü zorunludur.")]
        public string? Role { get; set; }

        [Required(ErrorMessage = "İletişim e-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "İletişim telefon zorunludur.")]
        [MinLength(10, ErrorMessage = "Telefon en az 10 karakter olmalıdır.")]
        public string? Phone { get; set; }
    }
} 