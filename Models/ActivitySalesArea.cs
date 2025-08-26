using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagementApi.Models
{
    [Table("ActivitySalesAreas")]
    public class ActivitySalesArea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ActivityId { get; set; }

        // Tek bir dikdörtgen alan için bounding box (WGS84)
        [Range(-90, 90)]
        public decimal SouthLat { get; set; }

        [Range(-90, 90)]
        public decimal NorthLat { get; set; }

        [Range(-180, 180)]
        public decimal WestLng { get; set; }

        [Range(-180, 180)]
        public decimal EastLng { get; set; }

        // İsteğe bağlı ad/etiket
        [MaxLength(150)]
        public string? Label { get; set; }
    }
}
