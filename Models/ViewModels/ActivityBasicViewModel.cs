using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TourManagementApi.Models.ViewModels
{
    public class ActivityBasicViewModel
    {
        public int? ActivityId { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required(ErrorMessage = "En az bir kategori seçilmelidir.")]
        [MaxLength(3, ErrorMessage = "En fazla 3 kategori seçebilirsiniz.")]
        public List<string>? Categories { get; set; }

        [MaxLength(100)]
        public string? Subcategory { get; set; }

        [Required(ErrorMessage = "Açıklama zorunludur.")]
        public string? Description { get; set; }

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

        // Temel Bilgiler
        public string? Highlights { get; set; }
        public List<string>? Inclusions { get; set; }
        public List<string>? Exclusions { get; set; }
        public List<string>? ImportantInfo { get; set; }
        public string? Itinerary { get; set; }

        // Lokasyon
        public string? CountryCode { get; set; }
        public string? DestinationCode { get; set; }
        public string? DestinationName { get; set; }
        public string? Category { get; set; }

        public string? Status { get; set; }
        public string? Label { get; set; }
        public string? TourCompany { get; set; }
        public int? TourCompanyId { get; set; }
        public List<SelectListItem>? TourCompanies { get; set; }

        // ✅ Yeni eklenen kolonlar (Listeleme için)
        public int OptionCount { get; set; }
        public int AddonCount { get; set; }
        public int ReservationCount { get; set; }
        public int AvailabilityCount { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public double? Rating { get; set; }
        public bool? IsFreeCancellation { get; set; }

        // İlişkisel veriler (sayfalarda direkt erişim gerekiyorsa)
        public ICollection<Option>? Options { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Availability>? Availabilities { get; set; }

        public int ReservationsCount { get; set; }
        public int AvailabilitiesCount { get; set; }

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
