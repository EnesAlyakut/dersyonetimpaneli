using Microsoft.AspNetCore.Mvc;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Web.Models.ViewModels;
using OgrenciDersYonetim.Web.Filters;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class AtamaController : Controller
    {
        private readonly IOgrenciService _ogrenciService;
        private readonly IDersService _dersService;
        private readonly IOgrenciDersService _ogrenciDersService;
        private readonly IOgretmenService _ogretmenService;

        public AtamaController(
            IOgrenciService ogrenciService,
            IDersService dersService,
            IOgrenciDersService ogrenciDersService,
            IOgretmenService ogretmenService)
        {
            _ogrenciService = ogrenciService;
            _dersService = dersService;
            _ogrenciDersService = ogrenciDersService;
            _ogretmenService = ogretmenService;
        }

        // GET: /Atama - Ders Atama Ekranı (Ekran 10)
        public async Task<IActionResult> Index(int? ogrenciId)
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            List<OgrenciDersYonetim.Core.Entities.Ogrenci> ogrenciler;

            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                // Öğretmen sadece kendi derslerindeki öğrencileri görür
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen != null)
                {
                    var ogrenciIdleri = await _ogretmenService.OgretmeninOgrenciIdleriniGetirAsync(ogretmen.Id);
                    var tumOgrenciler = await _ogrenciService.TumunuGetirAsync();
                    ogrenciler = tumOgrenciler.Where(o => ogrenciIdleri.Contains(o.Id)).ToList();
                }
                else
                {
                    ogrenciler = new List<OgrenciDersYonetim.Core.Entities.Ogrenci>();
                }
            }
            else
            {
                ogrenciler = await _ogrenciService.TumunuGetirAsync();
            }

            var model = new AtamaViewModel
            {
                SeciliOgrenciId = ogrenciId,
                Ogrenciler = ogrenciler
            };

            if (ogrenciId.HasValue)
            {
                model.AtanabilecekDersler = await _ogrenciDersService
                    .OgrencininAtanmamisDersleriniGetirAsync(ogrenciId.Value);

                model.MevcutAtamalar = await _ogrenciDersService
                    .OgrenciyeGoreDerslerGetirAsync(ogrenciId.Value);
            }

            ViewBag.Rol = rol;
            return View(model);
        }

        // POST: /Atama/AtamaYap
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> AtamaYap(int ogrenciId, int dersId)
        {
            var basarili = await _ogrenciDersService.AtamaYapAsync(ogrenciId, dersId);

            if (basarili)
                TempData["Basari"] = "Ders ataması başarıyla yapıldı.";
            else
                TempData["Hata"] = "Bu ders zaten bu öğrenciye atanmış.";

            return RedirectToAction("Index", new { ogrenciId });
        }

        // POST: /Atama/AtamaKaldir
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AdminOgretmenKontrol]
        public async Task<IActionResult> AtamaKaldir(int ogrenciId, int dersId)
        {
            var basarili = await _ogrenciDersService.AtamaKaldirAsync(ogrenciId, dersId);

            if (basarili)
                TempData["Basari"] = "Ders ataması başarıyla kaldırıldı.";
            else
                TempData["Hata"] = "Atama bulunamadı.";

            return RedirectToAction("Index", new { ogrenciId });
        }

        // GET: /Atama/DersDetay/5 - Derse kayıtlı öğrenciler
        public async Task<IActionResult> DersDetay(int dersId)
        {
            var ders = await _dersService.IdIleGetirAsync(dersId);
            if (ders == null)
                return NotFound();

            var ogrenciler = await _ogrenciDersService.DerseGoreOgrencilerGetirAsync(dersId);

            ViewBag.Ders = ders;
            ViewBag.Rol = HttpContext.Session.GetString("KullaniciRol");
            return View(ogrenciler);
        }
    }
}
