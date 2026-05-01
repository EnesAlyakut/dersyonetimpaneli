using System.ComponentModel.DataAnnotations;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Sisteme giriş yapabilen kullanıcıları temsil eder.
    /// Roller: Admin, Ogretmen, Ogrenci
    /// </summary>
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir.")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SifreHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50)]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50)]
        public string Soyad { get; set; } = string.Empty;

        /// <summary>
        /// Kullanıcı rolü: Admin | Ogretmen | Ogrenci
        /// </summary>
        [Required]
        public string Rol { get; set; } = "Ogrenci";

        public DateTime KayitTarihi { get; set; } = DateTime.Now;

        // Navigation Properties
        public Ogrenci? OgrenciProfil { get; set; }
    }
}
