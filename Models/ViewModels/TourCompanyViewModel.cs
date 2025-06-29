namespace TourManagementApi.Models.ViewModels
{
    public class TourCompanyViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string AuthorizedPerson { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string LogoPath { get; set; }
        public string ImzaDocumentPath { get; set; }
        public string FaaliyetBelgesiPath { get; set; }
        public string OdaSicilKaydiPath { get; set; }
        public string TicaretSicilGazetesiPath { get; set; }
        public string VergiLevhasıPath { get; set; }
        public string SigortaBelgesiPath { get; set; }
        public string HizmetDetayiPath { get; set; }
        public string AracD2BelgesiPath { get; set; }
        public string SportifFaaliyetBelgesiPath { get; set; }


        public IFormFile Logo { get; set; }
        public IFormFile ImzaDocument { get; set; }
        public IFormFile FaaliyetBelgesi { get; set; }
        public IFormFile OdaSicilKaydi { get; set; }
        public IFormFile TicaretSicilGazetesi { get; set; }
        public IFormFile VergiLevhası { get; set; }
        public IFormFile SigortaBelgesi { get; set; }
        public IFormFile HizmetDetayi { get; set; }
        public IFormFile AracD2Belgesi { get; set; }
        public IFormFile SportifFaaliyetBelgesi { get; set; }
    }

}
