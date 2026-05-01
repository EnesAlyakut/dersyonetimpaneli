using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Web.Filters;
using OgrenciDersYonetim.Web.Models.ViewModels;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class NotController : Controller
    {
        private readonly INotService _notService;
        private readonly IOgrenciService _ogrenciService;
        private readonly IDersService _dersService;
        private readonly IOgrenciDersService _ogrenciDersService;
        private readonly IOgretmenService _ogretmenService;

        public NotController(
            INotService notService,
            IOgrenciService ogrenciService,
            IDersService dersService,
            IOgrenciDersService ogrenciDersService,
            IOgretmenService ogretmenService)
        {
            _notService = notService;
            _ogrenciService = ogrenciService;
            _dersService = dersService;
            _ogrenciDersService = ogrenciDersService;
            _ogretmenService = ogretmenService;
        }

        // GET: /Not/DersNot - Ders bazlı not yönetimi (her ders ayrı sekme)
        [AdminOgretmenKontrol]
        public async Task<IActionResult> DersNot()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            List<Ders> dersler;

            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen == null) return RedirectToAction("YetkiHatasi", "Home");
                dersler = await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id);
            }
            else
            {
                dersler = await _dersService.TumunuGetirAsync();
            }

            // Her ders için kayıtlı öğrenciler ve notlar
            var dersOgrencileri = new Dictionary<int, List<OgrenciDers>>();
            var dersNotlari = new Dictionary<int, List<Not>>();

            foreach (var ders in dersler)
            {
                dersOgrencileri[ders.Id] = await _ogrenciDersService.DerseGoreOgrencilerGetirAsync(ders.Id);
                dersNotlari[ders.Id] = await _notService.DersIleGetirAsync(ders.Id);
            }

            ViewBag.Rol = rol;
            ViewBag.DersOgrencileri = dersOgrencileri;
            ViewBag.DersNotlari = dersNotlari;
            return View(dersler);
        }

        // POST: /Not/HizliNotEkle - Ders sayfasından tek tıkla not ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> HizliNotEkle(int ogrenciId, int dersId,
            double notDegeri, string notTuru, string tarih, string? aciklama)
        {
            if (!DateTime.TryParse(tarih, out var tarihDt))
                tarihDt = DateTime.Today;

            var not = new Not
            {
                OgrenciId = ogrenciId,
                DersId = dersId,
                NotDegeri = Math.Clamp(notDegeri, 0, 100),
                NotTuru = notTuru,
                Tarih = tarihDt,
                Aciklama = aciklama,
                GirenKullaniciId = HttpContext.Session.GetInt32("KullaniciId")
            };

            await _notService.EkleAsync(not);
            TempData["Basari"] = $"{notTuru} notu kaydedildi.";
            return RedirectToAction("DersNot");
        }

        // POST: /Not/HizliNotSil
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> HizliNotSil(int notId)
        {
            var not = await _notService.IdIleGetirAsync(notId);
            if (not != null)
            {
                var rol = HttpContext.Session.GetString("KullaniciRol");
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

                if (rol == "Ogretmen" && kullaniciId.HasValue)
                {
                    var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                    if (ogretmen != null)
                    {
                        var dersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                            .Select(d => d.Id).ToList();
                        if (!dersIdleri.Contains(not.DersId))
                        {
                            TempData["Hata"] = "Bu notu silme yetkiniz yok.";
                            return RedirectToAction("DersNot");
                        }
                    }
                }

                await _notService.SilAsync(not.Id);
                TempData["Basari"] = "Not silindi.";
            }
            return RedirectToAction("DersNot");
        }



        // GET: /Not/OgrenciNot/5 - Öğrencinin tüm notlarını listele
        // Admin ve Ogretmen görür; öğrenci kendi notlarını Ogrenci/Detay üzerinden görür
        [AdminOgretmenKontrol]
        public async Task<IActionResult> OgrenciNot(int ogrenciId)
        {
            var ogrenci = await _ogrenciService.IdIleGetirAsync(ogrenciId);
            if (ogrenci == null) return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen yetki kontrolü: sadece kendi derslerindeki öğrencilere erişebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen == null)
                    return RedirectToAction("YetkiHatasi", "Home");

                var ogrenciIdleri = await _ogretmenService.OgretmeninOgrenciIdleriniGetirAsync(ogretmen.Id);
                if (!ogrenciIdleri.Contains(ogrenciId))
                {
                    TempData["Hata"] = "Bu öğrenci sizin derslerinize kayıtlı değil.";
                    return RedirectToAction("Panelim", "Ogretmen");
                }
            }

            var notlar = await _notService.OgrenciIleGetirAsync(ogrenciId);

            // Öğretmen için sadece kendi derslerindeki notları göster
            List<SelectListItem> dersSelectList;
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    // Öğrencinin bu öğretmenin derslerindeki atamaları
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    var dersler = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(ogrenciId);
                    dersSelectList = dersler
                        .Where(od => ogretmenDersIdleri.Contains(od.DersId))
                        .Select(od => new SelectListItem
                        {
                            Value = od.DersId.ToString(),
                            Text = $"{od.Ders.DersAdi} ({od.Ders.DersKodu})"
                        }).ToList();
                }
                else
                {
                    dersSelectList = new List<SelectListItem>();
                }
            }
            else
            {
                var dersler = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(ogrenciId);
                dersSelectList = dersler.Select(od => new SelectListItem
                {
                    Value = od.DersId.ToString(),
                    Text = $"{od.Ders.DersAdi} ({od.Ders.DersKodu})"
                }).ToList();
            }

            ViewBag.Ogrenci = ogrenci;
            ViewBag.Dersler = dersSelectList;
            ViewBag.Rol = rol;

            return View(notlar);
        }

        // GET: /Not/Ekle?ogrenciId=5&dersId=2
        [AdminOgretmenKontrol]
        public async Task<IActionResult> Ekle(int ogrenciId, int? dersId)
        {
            var ogrenci = await _ogrenciService.IdIleGetirAsync(ogrenciId);
            if (ogrenci == null) return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen yetki kontrolü
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen == null) return RedirectToAction("YetkiHatasi", "Home");

                var ogrenciIdleri = await _ogretmenService.OgretmeninOgrenciIdleriniGetirAsync(ogretmen.Id);
                if (!ogrenciIdleri.Contains(ogrenciId))
                {
                    TempData["Hata"] = "Bu öğrenci sizin derslerinize kayıtlı değil.";
                    return RedirectToAction("Panelim", "Ogretmen");
                }
            }

            List<SelectListItem> dersSelectList;
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                var ogretmenDersIdleri = ogretmen != null
                    ? (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id)).Select(d => d.Id).ToList()
                    : new List<int>();

                var dersler = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(ogrenciId);
                dersSelectList = dersler
                    .Where(od => ogretmenDersIdleri.Contains(od.DersId))
                    .Select(od => new SelectListItem
                    {
                        Value = od.DersId.ToString(),
                        Text = $"{od.Ders.DersAdi} ({od.Ders.DersKodu})",
                        Selected = od.DersId == dersId
                    }).ToList();
            }
            else
            {
                var dersler = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(ogrenciId);
                dersSelectList = dersler.Select(od => new SelectListItem
                {
                    Value = od.DersId.ToString(),
                    Text = $"{od.Ders.DersAdi} ({od.Ders.DersKodu})",
                    Selected = od.DersId == dersId
                }).ToList();
            }

            var not = new Not
            {
                OgrenciId = ogrenciId,
                DersId = dersId ?? 0,
                Tarih = DateTime.Now,
                NotTuru = "Vize"
            };

            ViewBag.Ogrenci = ogrenci;
            ViewBag.Dersler = dersSelectList;

            return View(not);
        }

        // POST: /Not/Ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> Ekle(Not not)
        {
            ModelState.Remove("Ogrenci");
            ModelState.Remove("Ders");

            if (!ModelState.IsValid)
            {
                var ogrenci = await _ogrenciService.IdIleGetirAsync(not.OgrenciId);
                var dersler = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(not.OgrenciId);
                ViewBag.Ogrenci = ogrenci;
                ViewBag.Dersler = dersler.Select(od => new SelectListItem
                {
                    Value = od.DersId.ToString(),
                    Text = $"{od.Ders.DersAdi} ({od.Ders.DersKodu})"
                }).ToList();
                return View(not);
            }

            not.GirenKullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            await _notService.EkleAsync(not);
            TempData["Basari"] = "Not başarıyla eklendi.";
            return RedirectToAction("OgrenciNot", new { ogrenciId = not.OgrenciId });
        }

        // POST: /Not/Sil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> Sil(int id, int ogrenciId)
        {
            var not = await _notService.IdIleGetirAsync(id);
            if (not == null) return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen sadece kendi dersindeki notları silebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    if (!ogretmenDersIdleri.Contains(not.DersId))
                    {
                        TempData["Hata"] = "Bu notu silme yetkiniz yok.";
                        return RedirectToAction("OgrenciNot", new { ogrenciId });
                    }
                }
            }

            await _notService.SilAsync(id);
            TempData["Basari"] = "Not başarıyla silindi.";
            return RedirectToAction("OgrenciNot", new { ogrenciId });
        }
    }
}
