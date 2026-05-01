using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Ogrencilerin devamsizlik kayitlari (gelmediği günler).
    /// </summary>
    public class Devamsizlik
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

        /// <summary>
        /// Ogrencinin gelmedigi tarih.
        /// </summary>
        [Required(ErrorMessage = "Tarih zorunludur.")]
        [Display(Name = "Devamsizlik Tarihi")]
        [DataType(DataType.Date)]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [StringLength(500)]
        [Display(Name = "Aciklama")]
        public string? Aciklama { get; set; }

        // Kaydi giren kullanici
        public int? GirenKullaniciId { get; set; }

        [Display(Name = "Kayit Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now;
    }
}
