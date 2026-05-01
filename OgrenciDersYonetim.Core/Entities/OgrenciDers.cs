using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Öğrenci-Ders Many-to-Many ilişkisini temsil eden ara tablo.
    /// </summary>
    public class OgrenciDers
    {
        // Composite Primary Key - DbContext'te Fluent API ile tanımlanacak
        public int OgrenciId { get; set; }

        [ForeignKey("OgrenciId")]
        public Ogrenci Ogrenci { get; set; } = null!;

        public int DersId { get; set; }

        [ForeignKey("DersId")]
        public Ders Ders { get; set; } = null!;

        [Display(Name = "Atama Tarihi")]
        public DateTime AtamaTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Not")]
        [Range(0, 100, ErrorMessage = "Not 0-100 arasında olmalıdır.")]
        public double? Not { get; set; }
    }
}
