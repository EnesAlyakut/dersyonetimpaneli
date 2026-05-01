using Microsoft.AspNetCore.Mvc;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Web.Filters;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class DersController : Controller
    {
        private readonly IDersService _dersService;
        private readonly IOgrenciDersService _ogrenciDersService;
        private readonly IOgretmenService _ogretmenService;
        private readonly IOgrenciService _ogrenciService;
        private readonly IDersIcerikService _dersIcerikService;
        private readonly IDevamsizlikService _devamsizlikService;

        public DersController(
            IDersService dersService,
            IOgrenciDersService ogrenciDersService,
            IOgretmenService ogretmenService,
            IOgrenciService ogrenciService,
            IDersIcerikService dersIcerikService,
            IDevamsizlikService devamsizlikService)
        {
            _dersService = dersService;
            _ogrenciDersService = ogrenciDersService;
            _ogretmenService = ogretmenService;
            _ogrenciService = ogrenciService;
            _dersIcerikService = dersIcerikService;
            _devamsizlikService = devamsizlikService;
        }

        // GET: /Ders - Listeleme (role göre filtrelenir)
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            List<Ders> dersler;

            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                dersler = ogretmen != null
                    ? await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id)
                    : new List<Ders>();
            }
            else if (rol == "Ogrenci" && kullaniciId.HasValue)
            {
                var ogrenci = await _ogrenciService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogrenci != null)
                {
                    var ogrenciAtamalari = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(ogrenci.Id);
                    dersler = ogrenciAtamalari.Select(od => od.Ders).OrderBy(d => d.DersAdi).ToList();
                }
                else
                {
                    dersler = new List<Ders>();
                }
            }
            else
            {
                dersler = await _dersService.TumunuGetirAsync();
            }

            ViewBag.Rol = rol;
            return View(dersler);
        }

        // GET: /Ders/Detay/5
        public async Task<IActionResult> Detay(int id)
        {
            var ders = await _dersService.IdIleGetirAsync(id);
            if (ders == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğrenci sadece kendi atanmış dersini görebilir
            if (rol == "Ogrenci" && kullaniciId.HasValue)
            {
                var ogrenci = await _ogrenciService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogrenci != null)
                {
                    var atanmisMi = await _ogrenciDersService.AtamaMevcutMuAsync(ogrenci.Id, id);
                    if (!atanmisMi)
                        return RedirectToAction("YetkiHatasi", "Home");
                }
            }

            // Öğretmen sadece kendi atanmış dersini görebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    if (!ogretmenDersIdleri.Contains(id))
                        return RedirectToAction("YetkiHatasi", "Home");
                }
            }

            // İçerikleri yükle
            var icerikler = await _dersIcerikService.DersIleGetirAsync(id);

            // Devamsızlıkları ders bazlı yükle (admin/öğretmen için)
            var devamsizliklar = await _devamsizlikService.DersIleGetirAsync(id);

            // Öğrenci için kendi devamsızlıklarını yükle
            List<Devamsizlik>? ogrenciDevamsizliklari = null;
            if (rol == "Ogrenci" && kullaniciId.HasValue)
            {
                var ogrenci = await _ogrenciService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogrenci != null)
                {
                    ogrenciDevamsizliklari = await _devamsizlikService.OgrenciIleGetirAsync(ogrenci.Id);
                    ogrenciDevamsizliklari = ogrenciDevamsizliklari.Where(d => d.DersId == id).ToList();
                }
            }

            ViewBag.Rol = rol;
            ViewBag.Icerikler = icerikler;
            ViewBag.Devamsizliklar = devamsizliklar;
            ViewBag.OgrenciDevamsizliklari = ogrenciDevamsizliklari;
            return View(ders);
        }

        // GET: /Ders/Ekle - Sadece Admin
        [AdminKontrol]
        public IActionResult Ekle()
        {
            return View(new Ders());
        }

        // POST: /Ders/Ekle - Sadece Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Ekle(Ders ders)
        {
            ModelState.Remove("OgrenciDersler");
            ModelState.Remove("OgretmenDersler");
            ModelState.Remove("DersIcerikler");

            if (!ModelState.IsValid)
                return View(ders);

            if (await _dersService.DersKoduMevcutMuAsync(ders.DersKodu))
            {
                ModelState.AddModelError("DersKodu", "Bu ders kodu zaten kullanılıyor.");
                return View(ders);
            }

            await _dersService.EkleAsync(ders);
            TempData["Basari"] = $"{ders.DersAdi} dersi başarıyla eklendi.";
            return RedirectToAction("Index");
        }

        // GET: /Ders/Guncelle/5 - SADECE Admin
        [AdminKontrol]
        public async Task<IActionResult> Guncelle(int id)
        {
            var ders = await _dersService.IdIleGetirAsync(id);
            if (ders == null)
                return NotFound();

            return View(ders);
        }

        // POST: /Ders/Guncelle - SADECE Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Guncelle(Ders ders)
        {
            ModelState.Remove("OgrenciDersler");
            ModelState.Remove("OgretmenDersler");
            ModelState.Remove("DersIcerikler");

            if (!ModelState.IsValid)
                return View(ders);

            if (await _dersService.DersKoduMevcutMuAsync(ders.DersKodu, ders.Id))
            {
                ModelState.AddModelError("DersKodu", "Bu ders kodu zaten kullanılıyor.");
                return View(ders);
            }

            await _dersService.GuncelleAsync(ders);
            TempData["Basari"] = $"{ders.DersAdi} dersi başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        // GET: /Ders/EkleGuncelle - Geriye dönük uyumluluk
        [AdminKontrol]
        public IActionResult EkleGuncelle(int? id)
        {
            if (id == null)
                return RedirectToAction("Ekle");
            return RedirectToAction("Guncelle", new { id });
        }

        // POST: /Ders/Sil/5 - Sadece Admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminKontrol]
        public async Task<IActionResult> Sil(int id)
        {
            var ders = await _dersService.IdIleGetirAsync(id);
            if (ders == null)
                return NotFound();

            await _dersService.SilAsync(id);
            TempData["Basari"] = $"{ders.DersAdi} dersi başarıyla silindi.";
            return RedirectToAction("Index");
        }

        // ============================================================
        // DERS İÇERİK (Konu/Ödev/Duyuru) - Admin ve Ogretmen
        // ============================================================

        // POST: /Ders/IcerikEkle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> IcerikEkle(DersIcerik icerik)
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen sadece kendi dersine içerik ekleyebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    if (!ogretmenDersIdleri.Contains(icerik.DersId))
                    {
                        TempData["Hata"] = "Bu derse içerik ekleme yetkiniz yok.";
                        return RedirectToAction("Detay", new { id = icerik.DersId });
                    }
                }
            }

            ModelState.Remove("Ders");
            if (!ModelState.IsValid)
            {
                TempData["Hata"] = "İçerik bilgileri eksik veya hatalı.";
                return RedirectToAction("Detay", new { id = icerik.DersId });
            }

            icerik.EkleyenKullaniciId = kullaniciId;
            await _dersIcerikService.EkleAsync(icerik);
            TempData["Basari"] = $"\"{icerik.Baslik}\" başarıyla eklendi.";
            return RedirectToAction("Detay", new { id = icerik.DersId });
        }

        // POST: /Ders/IcerikSil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> IcerikSil(int id, int dersId)
        {
            var icerik = await _dersIcerikService.IdIleGetirAsync(id);
            if (icerik == null)
                return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen sadece kendi dersinin içeriğini silebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    if (!ogretmenDersIdleri.Contains(icerik.DersId))
                    {
                        TempData["Hata"] = "Bu içeriği silme yetkiniz yok.";
                        return RedirectToAction("Detay", new { id = dersId });
                    }
                }
            }

            await _dersIcerikService.SilAsync(id);
            TempData["Basari"] = "İçerik silindi.";
            return RedirectToAction("Detay", new { id = dersId });
        }

        // ============================================================
        // DEVAMSIZLIK - Öğretmen ders bazlı devamsızlık girişi
        // ============================================================

        // POST: /Ders/DevamsizlikGir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> DevamsizlikGir(int dersId, int ogrenciId, string tarih, string? aciklama)
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen sadece kendi dersindeki öğrencilere devamsızlık girebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    if (!ogretmenDersIdleri.Contains(dersId))
                    {
                        TempData["Hata"] = "Bu ders için devamsızlık girme yetkiniz yok.";
                        return RedirectToAction("Detay", new { id = dersId });
                    }
                }
            }

            if (!DateTime.TryParse(tarih, out var tarihDt))
                tarihDt = DateTime.Today;

            var devamsizlik = new Devamsizlik
            {
                OgrenciId = ogrenciId,
                DersId = dersId,
                Tarih = tarihDt,
                Aciklama = aciklama,
                GirenKullaniciId = kullaniciId,
                KayitTarihi = DateTime.Now
            };

            await _devamsizlikService.EkleAsync(devamsizlik);
            TempData["Basari"] = "Devamsızlık kaydedildi.";
            return RedirectToAction("Detay", new { id = dersId });
        }
    }
}
