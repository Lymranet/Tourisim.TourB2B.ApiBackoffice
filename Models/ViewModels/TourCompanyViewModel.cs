using System.ComponentModel.DataAnnotations;

namespace TourManagementApi.Models.ViewModels
{
    public class TourCompanyViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Şirket ismi zorunludur.")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Yetkili kişi zorunludur.")]
        public string AuthorizedPerson { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon zorunludur.")]
        public string Phone { get; set; }

        public string? LogoPath { get; set; }
        public string? ImzaDocumentPath { get; set; }
        public string? FaaliyetBelgesiPath { get; set; }
        public string? OdaSicilKaydiPath { get; set; }
        public string? TicaretSicilGazetesiPath { get; set; }
        public string? VergiLevhasıPath { get; set; }
        public string? SigortaBelgesiPath { get; set; }
        public string? HizmetDetayiPath { get; set; }
        public string? AracD2BelgesiPath { get; set; }
        public string? SportifFaaliyetBelgesiPath { get; set; }


        public IFormFile? Logo { get; set; }
        public IFormFile? ImzaDocument { get; set; }
        public IFormFile? FaaliyetBelgesi { get; set; }
        public IFormFile? OdaSicilKaydi { get; set; }
        public IFormFile? TicaretSicilGazetesi { get; set; }
        public IFormFile? VergiLevhası { get; set; }
        public IFormFile? SigortaBelgesi { get; set; }
        public IFormFile? HizmetDetayi { get; set; }
        public IFormFile? AracD2Belgesi { get; set; }
        public IFormFile? SportifFaaliyetBelgesi { get; set; }
    }

}
