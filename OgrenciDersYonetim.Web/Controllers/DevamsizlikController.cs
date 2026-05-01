using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Web.Filters;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class DevamsizlikController : Controller
    {
        private readonly IDevamsizlikService _devamsizlikService;
        private readonly IOgrenciService _ogrenciService;
        private readonly IOgrenciDersService _ogrenciDersService;
        private readonly IOgretmenService _ogretmenService;
        private readonly IDersService _dersService;

        public DevamsizlikController(
            IDevamsizlikService devamsizlikService,
            IOgrenciService ogrenciService,
            IOgrenciDersService ogrenciDersService,
            IOgretmenService ogretmenService,
            IDersService dersService)
        {
            _devamsizlikService = devamsizlikService;
            _ogrenciService = ogrenciService;
            _ogrenciDersService = ogrenciDersService;
            _ogretmenService = ogretmenService;
            _dersService = dersService;
        }

        // GET: /Devamsizlik/DersDevamsizlik - Ders bazlı devamsızlık yönetimi
        [AdminOgretmenKontrol]
        public async Task<IActionResult> DersDevamsizlik()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            List<Ders> dersler;

            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                // Öğretmen sadece kendi atandığı dersleri görür
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen == null) return RedirectToAction("YetkiHatasi", "Home");
                dersler = await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id);
            }
            else
            {
                // Admin tüm dersleri görür
                dersler = await _dersService.TumunuGetirAsync();
            }

            // Her ders için kayıtlı öğrenciler ve devamsızlıklar
            var dersOgrencileri = new Dictionary<int, List<OgrenciDers>>();
            var dersDevamsizliklari = new Dictionary<int, List<Devamsizlik>>();

            foreach (var ders in dersler)
            {
                dersOgrencileri[ders.Id] = await _ogrenciDersService.DerseGoreOgrencilerGetirAsync(ders.Id);
                dersDevamsizliklari[ders.Id] = await _devamsizlikService.DersIleGetirAsync(ders.Id);
            }

            ViewBag.Rol = rol;
            ViewBag.DersOgrencileri = dersOgrencileri;
            ViewBag.DersDevamsizliklari = dersDevamsizliklari;
            return View(dersler);
        }

        // POST: /Devamsizlik/HizliEkle - Ders sayfasından tek tıkla devamsızlık ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> HizliEkle(int ogrenciId, int dersId, string tarih, string? aciklama)
        {
            if (!DateTime.TryParse(tarih, out var tarihDt))
                tarihDt = DateTime.Today;

            var devamsizlik = new Devamsizlik
            {
                OgrenciId = ogrenciId,
                DersId = dersId,
                Tarih = tarihDt,
                Aciklama = aciklama,
                GirenKullaniciId = HttpContext.Session.GetInt32("KullaniciId")
            };

            await _devamsizlikService.EkleAsync(devamsizlik);
            TempData["Basari"] = "Devamsızlık kaydedildi.";
            return RedirectToAction("DersDevamsizlik");
        }

        // POST: /Devamsizlik/HizliSil - Ders sayfasından devamsızlık sil
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> HizliSil(int devamsizlikId)
        {
            var devamsizlik = await _devamsizlikService.IdIleGetirAsync(devamsizlikId);
            if (devamsizlik != null)
            {
                var rol = HttpContext.Session.GetString("KullaniciRol");
                var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

                // Öğretmen yetki kontrolü
                if (rol == "Ogretmen" && kullaniciId.HasValue)
                {
                    var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                    if (ogretmen != null)
                    {
                        var dersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                            .Select(d => d.Id).ToList();
                        if (!dersIdleri.Contains(devamsizlik.DersId))
                        {
                            TempData["Hata"] = "Bu kaydı silme yetkiniz yok.";
                            return RedirectToAction("DersDevamsizlik");
                        }
                    }
                }

                await _devamsizlikService.SilAsync(devamsizlik.Id);
                TempData["Basari"] = "Devamsızlık kaydı silindi.";
            }
            return RedirectToAction("DersDevamsizlik");
        }

        // POST: /Devamsizlik/TopluDevamsizlikEkle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> TopluDevamsizlikEkle(int dersId, string tarih, string? aciklama, List<int>? ogrenciIdler)
        {
            if (ogrenciIdler == null || !ogrenciIdler.Any())
            {
                TempData["Hata"] = "Lütfen en az bir öğrenci seçin.";
                return RedirectToAction("DersDevamsizlik");
            }
            if (!DateTime.TryParse(tarih, out var tarihDt))
                tarihDt = DateTime.Today;

            var girenId = HttpContext.Session.GetInt32("KullaniciId");
            foreach (var ogrenciId in ogrenciIdler)
            {
                await _devamsizlikService.EkleAsync(new Devamsizlik
                {
                    OgrenciId = ogrenciId,
                    DersId = dersId,
                    Tarih = tarihDt,
                    Aciklama = aciklama,
                    GirenKullaniciId = girenId
                });
            }
            TempData["Basari"] = $"{ogrenciIdler.Count} öğrenciye devamsızlık kaydedildi.";
            return RedirectToAction("DersDevamsizlik");
        }

        // GET: /Devamsizlik/OgrenciDevamsizlik/5
        [AdminOgretmenKontrol]
        public async Task<IActionResult> OgrenciDevamsizlik(int ogrenciId)
        {
            var ogrenci = await _ogrenciService.IdIleGetirAsync(ogrenciId);
            if (ogrenci == null) return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen yetki kontrolü: sadece kendi derslerindeki öğrencilere erişebilir
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

            var devamsizliklar = await _devamsizlikService.OgrenciIleGetirAsync(ogrenciId);

            List<SelectListItem> dersSelectList;
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
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
            ViewBag.ToplamDevamsizlik = devamsizliklar.Count;
            ViewBag.Rol = rol;

            return View(devamsizliklar);
        }

        // GET: /Devamsizlik/Ekle?ogrenciId=5&dersId=2
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

            var devamsizlik = new Devamsizlik
            {
                OgrenciId = ogrenciId,
                DersId = dersId ?? 0,
                Tarih = DateTime.Today
            };

            ViewBag.Ogrenci = ogrenci;
            ViewBag.Dersler = dersSelectList;

            return View(devamsizlik);
        }

        // POST: /Devamsizlik/Ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> Ekle(Devamsizlik devamsizlik)
        {
            ModelState.Remove("Ogrenci");
            ModelState.Remove("Ders");

            if (!ModelState.IsValid)
            {
                var ogrenci = await _ogrenciService.IdIleGetirAsync(devamsizlik.OgrenciId);
                var dersler = await _ogrenciDersService.OgrenciyeGoreDerslerGetirAsync(devamsizlik.OgrenciId);
                ViewBag.Ogrenci = ogrenci;
                ViewBag.Dersler = dersler.Select(od => new SelectListItem
                {
                    Value = od.DersId.ToString(),
                    Text = $"{od.Ders.DersAdi} ({od.Ders.DersKodu})"
                }).ToList();
                return View(devamsizlik);
            }

            devamsizlik.GirenKullaniciId = HttpContext.Session.GetInt32("KullaniciId");
            await _devamsizlikService.EkleAsync(devamsizlik);
            TempData["Basari"] = "Devamsızlık kaydı başarıyla eklendi.";
            return RedirectToAction("OgrenciDevamsizlik", new { ogrenciId = devamsizlik.OgrenciId });
        }

        // POST: /Devamsizlik/Sil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> Sil(int id, int ogrenciId)
        {
            var devamsizlik = await _devamsizlikService.IdIleGetirAsync(id);
            if (devamsizlik == null) return NotFound();

            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğretmen sadece kendi dersindeki devamsızlıkları silebilir
            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogretmenDersIdleri = (await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id))
                        .Select(d => d.Id).ToList();
                    if (!ogretmenDersIdleri.Contains(devamsizlik.DersId))
                    {
                        TempData["Hata"] = "Bu devamsızlık kaydını silme yetkiniz yok.";
                        return RedirectToAction("OgrenciDevamsizlik", new { ogrenciId });
                    }
                }
            }

            await _devamsizlikService.SilAsync(id);
            TempData["Basari"] = "Devamsızlık kaydı silindi.";
            return RedirectToAction("OgrenciDevamsizlik", new { ogrenciId });
        }
    }
}
