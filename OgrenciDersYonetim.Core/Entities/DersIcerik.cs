using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Bir derse ait konu, ödev veya duyuru içerikleri.
    /// Sadece atanmış öğretmen veya admin ekleyebilir.
    /// </summary>
    public class DersIcerik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DersId { get; set; }

        [ForeignKey("DersId")]
        public Ders Ders { get; set; } = null!;

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Baslik { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [Display(Name = "İçerik")]
        public string Icerik { get; set; } = string.Empty;

        /// <summary>Konu, Odev, Duyuru</summary>
        [Required]
        [StringLength(20)]
        [Display(Name = "Tür")]
        public string Tur { get; set; } = "Konu"; // Konu | Odev | Duyuru

        [Display(Name = "Son Teslim Tarihi (Ödev için)")]
        [DataType(DataType.Date)]
        public DateTime? SonTeslimTarihi { get; set; }

        [Display(Name = "Eklenme Tarihi")]
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;

        public int? EkleyenKullaniciId { get; set; }
    }
}
