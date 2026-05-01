using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Ogrencilerin ders bazli not kayitlari.
    /// </summary>
    public class Not
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OgrenciId { get; set; }

        [ForeignKey("OgrenciId")]
        public Ogrenci Ogrenci { get; set; } = null!;

        [Required]
        public int DersId { get; set; }

        [ForeignKey("DersId")]
        public Ders Ders { get; set; } = null!;

        [Required(ErrorMessage = "Not degeri zorunludur.")]
        [Range(0, 100, ErrorMessage = "Not 0 ile 100 arasinda olmalidir.")]
        [Display(Name = "Not")]
        public double NotDegeri { get; set; }

        [Required(ErrorMessage = "Not turu zorunludur.")]
        [StringLength(50)]
        [Display(Name = "Not Turu")]
        // Ornek: "Vize", "Final", "Odev", "Kisa Sinav"
        public string NotTuru { get; set; } = "Vize";

        [Display(Name = "Tarih")]
        public DateTime Tarih { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Aciklama")]
        public string? Aciklama { get; set; }

        // Notu giren kullanici (ogretmen veya admin)
        public int? GirenKullaniciId { get; set; }
    }
}
