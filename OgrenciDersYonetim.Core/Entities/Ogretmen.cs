using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Okuldaki ogretmenleri temsil eder.
    /// </summary>
    public class Ogretmen
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Brans")]
        public string? Brans { get; set; }

        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [StringLength(15)]
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [Display(Name = "Kayit Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Kullanici hesabi ile ilisi (1-to-1 opsiyonel)
        public int? KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; }

        // Navigation - bir ogretmenin verdigi dersler
        public ICollection<OgretmenDers> OgretmenDersler { get; set; } = new List<OgretmenDers>();

        [NotMapped]
        public string TamAd => $"{Ad} {Soyad}";
    }
}
