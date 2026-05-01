using System.ComponentModel.DataAnnotations;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Eğitim kurumundaki ders bilgilerini temsil eder.
    /// </summary>
    public class Ders
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ders adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Ders adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Ders Adı")]
        public string DersAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ders kodu zorunludur.")]
        [StringLength(20, ErrorMessage = "Ders kodu en fazla 20 karakter olabilir.")]
        [Display(Name = "Ders Kodu")]
        public string DersKodu { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Kredi 1 ile 10 arasında olmalıdır.")]
        [Display(Name = "Kredi")]
        public int Kredi { get; set; }

        [StringLength(500)]
        [Display(Name = "Açıklama")]
        public string? Aciklama { get; set; }

        [Display(Name = "Öğretmen")]
        public string? Ogretmen { get; set; }

        [Display(Name = "Eklenme Tarihi")]
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        // Navigation Property - Many-to-Many (Ogrenci)
        public ICollection<OgrenciDers> OgrenciDersler { get; set; } = new List<OgrenciDers>();

        // Navigation Property - Many-to-Many (Ogretmen)
        public ICollection<OgretmenDers> OgretmenDersler { get; set; } = new List<OgretmenDers>();

        // Navigation Property - Ders İçerikleri (Konu/Ödev/Duyuru)
        public ICollection<DersIcerik> DersIcerikler { get; set; } = new List<DersIcerik>();
    }
}
