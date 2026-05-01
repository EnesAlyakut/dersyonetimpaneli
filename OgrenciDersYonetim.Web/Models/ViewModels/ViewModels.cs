using System.ComponentModel.DataAnnotations;

namespace OgrenciDersYonetim.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kullanıcı adı zorunludur.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Kullanıcı adı 3-50 karakter olmalıdır.")]
        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Sifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre Tekrar")]
        public string SifreTekrar { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rol seçimi zorunludur.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Ogrenci";
    }

    public class ProfilViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Ad")]
        public string Ad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Soyad")]
        public string Soyad { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Kullanıcı Adı")]
        public string KullaniciAdi { get; set; } = string.Empty;

        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; }

        // Şifre değiştirme (opsiyonel)
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string? MevcutSifre { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = "Yeni Şifre")]
        public string? YeniSifre { get; set; }

        [DataType(DataType.Password)]
        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Yeni Şifre Tekrar")]
        public string? YeniSifreTekrar { get; set; }
    }

    public class DashboardViewModel
    {
        public int ToplamOgrenci { get; set; }
        public int ToplamDers { get; set; }
        public int ToplamAtama { get; set; }
        public int ToplamOgretmen { get; set; }
        public string KullaniciAdi { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public List<string> SonEklenenOgrenciler { get; set; } = new();
        public List<string> SonEklenenDersler { get; set; } = new();
    }

    public class AtamaViewModel
    {
        public int? SeciliOgrenciId { get; set; }
        public int? SeciliDersId { get; set; }
        public List<OgrenciDersYonetim.Core.Entities.Ogrenci> Ogrenciler { get; set; } = new();
        public List<OgrenciDersYonetim.Core.Entities.Ders> AtanabilecekDersler { get; set; } = new();
        public List<OgrenciDersYonetim.Core.Entities.OgrenciDers> MevcutAtamalar { get; set; } = new();
    }

    public class SifreAyarlaViewModel
    {
        [Required(ErrorMessage = "Kullanıcı seçimi zorunludur.")]
        [Display(Name = "Kullanıcı")]
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string YeniSifre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare("YeniSifre", ErrorMessage = "Şifreler eşleşmiyor.")]
        [Display(Name = "Şifre Tekrar")]
        public string YeniSifreTekrar { get; set; } = string.Empty;
    }
}
