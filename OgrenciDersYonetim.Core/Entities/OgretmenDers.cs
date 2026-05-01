using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OgrenciDersYonetim.Core.Entities
{
    /// <summary>
    /// Ogretmen-Ders Many-to-Many iliskisi (hangi ogretmen hangi dersi veriyor).
    /// </summary>
    public class OgretmenDers
    {
        public int OgretmenId { get; set; }

        [ForeignKey("OgretmenId")]
        public Ogretmen Ogretmen { get; set; } = null!;

        public int DersId { get; set; }

        [ForeignKey("DersId")]
        public Ders Ders { get; set; } = null!;

        [Display(Name = "Atama Tarihi")]
        public DateTime AtamaTarihi { get; set; } = DateTime.Now;
    }
}
