using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityBasicViewModel
    {
        public int? ActivityId { get; set; }

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

        // İletişim bilgileri - zorunluluk kaldırıldı
        public ContactInfoViewModel ContactInfo { get; set; } = new();

        // Medya
        public IFormFile? CoverImage { get; set; }
        public IFormFile? PreviewImage { get; set; }
        public List<IFormFile>? GalleryImages { get; set; }
        public List<string> ExistingGalleryImages { get; set; } = new();
        public List<string>? VideoUrls { get; set; }

        public string? CoverImageUrl { get; set; }
        public string? PreviewImageUrl { get; set; }
        public List<string>? GalleryImageUrls { get; set; }

        // Temel Bilgiler - zorunluluklar kaldırıldı
        public string? Highlights { get; set; }
        public List<string>? Inclusions { get; set; }
        public List<string>? Exclusions { get; set; }
        public List<string>? ImportantInfo { get; set; }
        public string? Itinerary { get; set; }

        // Lokasyon
        public string? CountryCode { get; set; }
        public string? DestinationCode { get; set; }
        public string? DestinationName { get; set; }
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
    public class DeleteGalleryImageRequest
    {
        public string ImagePath { get; set; }
        public int ActivityId { get; set; }
    }

    public class DeleteImageRequest
    {
        public int ActivityId { get; set; }
        public string ImageType { get; set; } // "cover" veya "preview"
    }
    public class StatusUpdateDto
    {
        public string Status { get; set; }
    }

}