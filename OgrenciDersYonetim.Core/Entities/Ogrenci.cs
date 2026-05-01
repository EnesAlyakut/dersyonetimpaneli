using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Eğitim kurumundaki öğrenci profillerini temsil eder.
    /// </summary>
    public class Ogrenci
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Öğrenci numarası zorunludur.")]
        [StringLength(20, ErrorMessage = "Öğrenci no en fazla 20 karakter olabilir.")]
        [Display(Name = "Öğrenci No")]
        public string OgrenciNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [StringLength(100)]
        [Display(Name = "E-posta")]
        public string? Email { get; set; }

        [StringLength(15)]
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Foreign Key - Kullanici ile ilişki (opsiyonel)
        public int? KullaniciId { get; set; }

        [ForeignKey("KullaniciId")]
        public Kullanici? Kullanici { get; set; }

        // Navigation Property - Many-to-Many
        public ICollection<OgrenciDers> OgrenciDersler { get; set; } = new List<OgrenciDers>();

        [NotMapped]
        public string TamAd => $"{Ad} {Soyad}";
    }
}
