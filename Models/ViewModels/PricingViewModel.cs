namespace TourManagementApi.Models.ViewModels
{
    public class TicketCategoryPricingViewModel
    {
        public int TicketCategoryId { get; set; }
        public string TicketCategoryName { get; set; } = "";
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "TRY";
        public decimal SupplierCost { get; set; } // Taşeron alış fiyatı
    }

    public class OptionPricingViewModel
    {
        public int OptionId { get; set; }
        public string OptionName { get; set; } = "";

        public List<TicketCategoryPricingViewModel> TicketCategories { get; set; } = new();

        public decimal PpCost => ToplamMaliyet/TicketCategories.Count;
        public decimal TasaronMaliyeti => TicketCategories.Sum(tc => tc.SupplierCost);

        public decimal AracMaliyeti { get; set; }
        public decimal TopMaliyeti { get; set; }

        public decimal KomisyonMaliyeti { get; set; }
        public decimal GelirVergisi { get; set; }
        public decimal PlatformKomisyonTutari { get; set; }
        public decimal RehberBonus { get; set; }
        public decimal PlatformKomOrani { get; set; } = 0.3m;

        public decimal Karlilik { get; set; } 

        public decimal ToplamMaliyet => AracMaliyeti + TopMaliyeti + TasaronMaliyeti + KomisyonMaliyeti + GelirVergisi ;

        public decimal HesaplananSatisFiyati =>  (ToplamMaliyet + PlatformKomisyonTutari + RehberBonus) / (1 - PlatformKomOrani);
    }

    public class FiyatlandirmaViewModel
    {
        public int ActivityId { get; set; }
        public string ActivityTitle { get; set; } = "";
        public List<OptionPricingViewModel> Options { get; set; } = new();
    }

}
