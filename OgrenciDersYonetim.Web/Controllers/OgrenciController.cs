using Microsoft.AspNetCore.Mvc;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Web.Filters;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class OgrenciController : Controller
    {
        private readonly IOgrenciService _ogrenciService;
        private readonly INotService _notService;
        private readonly IDevamsizlikService _devamsizlikService;
        private readonly IKullaniciService _kullaniciService;

        public OgrenciController(IOgrenciService ogrenciService, INotService notService,
            IDevamsizlikService devamsizlikService, IKullaniciService kullaniciService)
        {
            _ogrenciService = ogrenciService;
            _notService = notService;
            _devamsizlikService = devamsizlikService;
            _kullaniciService = kullaniciService;
        }

        // GET: /Ogrenci - Listeleme (Ekran 4)
        public async Task<IActionResult> Index(string? ara)
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            List<Ogrenci> ogrenciler;

            // Öğrenci rolü sadece kendi profilini görür
            if (rol == "Ogrenci")
            {
                var kendi = await _ogrenciService.KullaniciIdIleGetirAsync(kullaniciId!.Value);
                ogrenciler = kendi != null ? new List<Ogrenci> { kendi } : new List<Ogrenci>();
            }
            else if (!string.IsNullOrEmpty(ara))
            {
                ogrenciler = await _ogrenciService.AraAsync(ara);
            }
            else
            {
                ogrenciler = await _ogrenciService.TumunuGetirAsync();
            }

            ViewBag.AramaMetni = ara;
            ViewBag.Rol = rol;
            return View(ogrenciler);
        }

        // GET: /Ogrenci/Detay/5 (Ekran 7)
        public async Task<IActionResult> Detay(int id)
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            var ogrenci = await _ogrenciService.IdIleGetirAsync(id);
            if (ogrenci == null)
                return NotFound();

            // Öğrenci sadece kendi profilini görebilir
            if (rol == "Ogrenci" && ogrenci.KullaniciId != kullaniciId)
                return RedirectToAction("YetkiHatasi", "Home");

            // Not ve devamsızlık verilerini yükle
            var notlar = await _notService.OgrenciIleGetirAsync(id);
            var devamsizliklar = await _devamsizlikService.OgrenciIleGetirAsync(id);

            ViewBag.Rol = rol;
            ViewBag.Notlar = notlar;
            ViewBag.Devamsizliklar = devamsizliklar;
            ViewBag.ToplamDevamsizlik = devamsizliklar.Count;
            return View(ogrenci);
        }

        // GET: /Ogrenci/Ekle (Ekran 5)
        [AdminKontrol]
        public IActionResult Ekle()
        {
            return View(new Ogrenci());
        }

        // POST: /Ogrenci/Ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Ekle(Ogrenci ogrenci)
        {
            // Navigation property'leri model state'den çıkar
            ModelState.Remove("OgrenciDersler");
            ModelState.Remove("Kullanici");

            if (!ModelState.IsValid)
                return View(ogrenci);

            // Öğrenci no benzersizlik kontrolü
            if (await _ogrenciService.OgrenciNoMevcutMuAsync(ogrenci.OgrenciNo))
            {
                ModelState.AddModelError("OgrenciNo", "Bu öğrenci numarası zaten kullanılıyor.");
                return View(ogrenci);
            }

            // Öğrenciyi kaydet
            await _ogrenciService.EkleAsync(ogrenci);

            // Otomatik Kullanıcı Hesabı Oluştur
            // Kullanıcı adı: öğrenci numarası (küçük harfle)
            var kullaniciAdi = ogrenci.OgrenciNo.ToLower();

            // Eğer aynı kullanıcı adı varsa (nadir durum) sonuna _ ekle
            var baseAdi = kullaniciAdi;
            var sayac = 1;
            while (await _kullaniciService.KullaniciAdiMevcutMuAsync(kullaniciAdi))
            {
                kullaniciAdi = baseAdi + "_" + sayac;
                sayac++;
            }

            var ogrenciEmail = !string.IsNullOrEmpty(ogrenci.Email) ? ogrenci.Email : $"{kullaniciAdi}@okul.edu.tr";

            // Email benzersizlik kontrolü
            if (await _kullaniciService.EmailMevcutMuAsync(ogrenciEmail))
                ogrenciEmail = $"{kullaniciAdi}@okul.edu.tr";

            var yeniKullanici = new Kullanici
            {
                Ad = ogrenci.Ad,
                Soyad = ogrenci.Soyad,
                KullaniciAdi = kullaniciAdi,
                Email = ogrenciEmail,
                Rol = "Ogrenci",
                KayitTarihi = DateTime.Now
            };

            await _kullaniciService.KayitOlAsync(yeniKullanici, "Kampus123!");

            // Öğrenciyi bu kullanıcıyla ilişkilendir
            ogrenci.KullaniciId = yeniKullanici.Id;
            await _ogrenciService.GuncelleAsync(ogrenci);

            TempData["Basari"] = $"{ogrenci.Ad} {ogrenci.Soyad} eklendi. Giriş: kullanıcı adı '{kullaniciAdi}', şifre 'Kampus123!'";
            return RedirectToAction("Index");
        }

        // GET: /Ogrenci/Guncelle/5 (Ekran 6)
        [AdminKontrol]
        public async Task<IActionResult> Guncelle(int id)
        {
            var ogrenci = await _ogrenciService.IdIleGetirAsync(id);
            if (ogrenci == null)
                return NotFound();

            return View(ogrenci);
        }

        // POST: /Ogrenci/Guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Guncelle(Ogrenci ogrenci)
        {
            ModelState.Remove("OgrenciDersler");
            ModelState.Remove("Kullanici");

            if (!ModelState.IsValid)
                return View(ogrenci);

            // Öğrenci no benzersizlik kontrolü (kendisi hariç)
            if (await _ogrenciService.OgrenciNoMevcutMuAsync(ogrenci.OgrenciNo, ogrenci.Id))
            {
                ModelState.AddModelError("OgrenciNo", "Bu öğrenci numarası başka bir öğrenciye ait.");
                return View(ogrenci);
            }

            await _ogrenciService.GuncelleAsync(ogrenci);
            TempData["Basari"] = $"{ogrenci.Ad} {ogrenci.Soyad} başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // POST: /Ogrenci/Sil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Sil(int id)
        {
            var ogrenci = await _ogrenciService.IdIleGetirAsync(id);
            if (ogrenci == null)
                return NotFound();

            await _ogrenciService.SilAsync(id);
            TempData["Basari"] = $"{ogrenci.Ad} {ogrenci.Soyad} başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }
}
