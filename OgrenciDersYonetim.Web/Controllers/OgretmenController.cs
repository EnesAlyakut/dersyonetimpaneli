using Microsoft.AspNetCore.Mvc;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Web.Filters;
using OgrenciDersYonetim.Web.Models.ViewModels;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class OgretmenController : Controller
    {
        private readonly IOgretmenService _ogretmenService;
        private readonly IDersService _dersService;
        private readonly IKullaniciService _kullaniciService;
        private readonly IDevamsizlikService _devamsizlikService;
        private readonly IOgrenciDersService _ogrenciDersService;

        public OgretmenController(
            IOgretmenService ogretmenService,
            IDersService dersService,
            IKullaniciService kullaniciService,
            IDevamsizlikService devamsizlikService,
            IOgrenciDersService ogrenciDersService)
        {
            _ogretmenService = ogretmenService;
            _dersService = dersService;
            _kullaniciService = kullaniciService;
            _devamsizlikService = devamsizlikService;
            _ogrenciDersService = ogrenciDersService;
        }

        // GET: /Ogretmen - Listeleme (Admin görür)
        [AdminKontrol]
        public async Task<IActionResult> Index()
        {
            var ogretmenler = await _ogretmenService.TumunuGetirAsync();
            ViewBag.Rol = HttpContext.Session.GetString("KullaniciRol");
            return View(ogretmenler);
        }

        // GET: /Ogretmen/Detay/5
        [AdminKontrol]
        public async Task<IActionResult> Detay(int id)
        {
            var ogretmen = await _ogretmenService.IdIleGetirAsync(id);
            if (ogretmen == null)
                return NotFound();

            var atanabilecekDersler = await _ogretmenService.OgretmenineAtanmamisDersleriniGetirAsync(id);
            ViewBag.AtanabilecekDersler = atanabilecekDersler;
            ViewBag.Rol = HttpContext.Session.GetString("KullaniciRol");

            // Her ders için kayıtlı öğrenciler ve devamsızlıkları yükle
            // Dictionary<dersId, List<OgrenciDers>>
            var dersOgrencileri = new Dictionary<int, List<OgrenciDers>>();
            // Dictionary<dersId, List<Devamsizlik>>
            var dersDevamsizliklari = new Dictionary<int, List<Devamsizlik>>();

            foreach (var od in ogretmen.OgretmenDersler)
            {
                var ogrenciler = await _ogrenciDersService.DerseGoreOgrencilerGetirAsync(od.DersId);
                dersOgrencileri[od.DersId] = ogrenciler;

                var devamsizliklar = await _devamsizlikService.DersIleGetirAsync(od.DersId);
                dersDevamsizliklari[od.DersId] = devamsizliklar;
            }

            ViewBag.DersOgrencileri = dersOgrencileri;
            ViewBag.DersDevamsizliklari = dersDevamsizliklari;

            return View(ogretmen);
        }

        // GET: /Ogretmen/Ekle
        [AdminKontrol]
        public IActionResult Ekle()
        {
            return View(new Ogretmen());
        }

        // POST: /Ogretmen/Ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Ekle(Ogretmen ogretmen)
        {
            ModelState.Remove("OgretmenDersler");
            ModelState.Remove("Kullanici");

            if (!ModelState.IsValid)
                return View(ogretmen);

            await _ogretmenService.EkleAsync(ogretmen);

            // Otomatik Kullanıcı Hesabı Oluştur
            var kullaniciAdi = (ogretmen.Ad.ToLower().Replace(" ", "") + "." + ogretmen.Soyad.ToLower().Replace(" ", ""))
                .Replace("ı", "i").Replace("ğ", "g").Replace("ü", "u")
                .Replace("ş", "s").Replace("ö", "o").Replace("ç", "c");

            // Aynı kullanıcı adı varsa sonuna sayı ekle
            var baseAdi = kullaniciAdi;
            var sayac = 1;
            while (await _kullaniciService.KullaniciAdiMevcutMuAsync(kullaniciAdi))
            {
                kullaniciAdi = baseAdi + sayac;
                sayac++;
            }

            var yeniKullanici = new Kullanici
            {
                Ad = ogretmen.Ad,
                Soyad = ogretmen.Soyad,
                KullaniciAdi = kullaniciAdi,
                Email = ogretmen.Email ?? $"{kullaniciAdi}@okul.edu.tr",
                Rol = "Ogretmen",
                KayitTarihi = DateTime.Now
            };

            // Email benzersizlik kontrolü
            if (!string.IsNullOrEmpty(yeniKullanici.Email) && await _kullaniciService.EmailMevcutMuAsync(yeniKullanici.Email))
                yeniKullanici.Email = $"{kullaniciAdi}@okul.edu.tr";

            await _kullaniciService.KayitOlAsync(yeniKullanici, "Kampus123!");

            // Öğretmeni bu kullanıcıyla ilişkilendir
            ogretmen.KullaniciId = yeniKullanici.Id;
            await _ogretmenService.GuncelleAsync(ogretmen);

            TempData["Basari"] = $"{ogretmen.Ad} {ogretmen.Soyad} eklendi. Giriş: kullanıcı adı '{kullaniciAdi}', şifre 'Kampus123!'";
            return RedirectToAction("Index");
        }

        // GET: /Ogretmen/Guncelle/5
        [AdminKontrol]
        public async Task<IActionResult> Guncelle(int id)
        {
            var ogretmen = await _ogretmenService.IdIleGetirAsync(id);
            if (ogretmen == null)
                return NotFound();

            return View(ogretmen);
        }

        // POST: /Ogretmen/Guncelle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Guncelle(Ogretmen ogretmen)
        {
            ModelState.Remove("OgretmenDersler");
            ModelState.Remove("Kullanici");

            if (!ModelState.IsValid)
                return View(ogretmen);

            await _ogretmenService.GuncelleAsync(ogretmen);
            TempData["Basari"] = $"{ogretmen.Ad} {ogretmen.Soyad} başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // POST: /Ogretmen/Sil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Sil(int id)
        {
            var ogretmen = await _ogretmenService.IdIleGetirAsync(id);
            if (ogretmen == null)
                return NotFound();

            await _ogretmenService.SilAsync(id);
            TempData["Basari"] = $"{ogretmen.Ad} {ogretmen.Soyad} başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // POST: /Ogretmen/DersAta
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> DersAta(int ogretmenId, int dersId)
        {
            var basarili = await _ogretmenService.DersAtaAsync(ogretmenId, dersId);
            if (basarili)
                TempData["Basari"] = "Ders öğretmene başarıyla atandı.";
            else
                TempData["Hata"] = "Bu ders zaten bu öğretmene atanmış.";

            return RedirectToAction("Detay", new { id = ogretmenId });
        }

        // POST: /Ogretmen/DersAtamaKaldir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> DersAtamaKaldir(int ogretmenId, int dersId)
        {
            var basarili = await _ogretmenService.DersAtamaKaldirAsync(ogretmenId, dersId);
            if (basarili)
                TempData["Basari"] = "Ders ataması kaldırıldı.";
            else
                TempData["Hata"] = "Atama bulunamadı.";

            return RedirectToAction("Detay", new { id = ogretmenId });
        }

        // GET: /Ogretmen/Panelim - Öğretmenin kendi paneli
        public async Task<IActionResult> Panelim()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            if (rol != "Ogretmen" || !kullaniciId.HasValue)
                return RedirectToAction("YetkiHatasi", "Home");

            var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
            if (ogretmen == null)
            {
                TempData["Hata"] = "Öğretmen profili bulunamadı. Lütfen admin ile iletişime geçin.";
                return RedirectToAction("Index", "Home");
            }

            var dersler = await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id);
            ViewBag.Ogretmen = ogretmen;
            return View(dersler);
        }
    }
}
