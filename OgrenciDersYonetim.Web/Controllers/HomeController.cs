using Microsoft.AspNetCore.Mvc;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Web.Models.ViewModels;
using OgrenciDersYonetim.Web.Filters;

namespace OgrenciDersYonetim.Web.Controllers
{
    [SessionKontrol]
    public class HomeController : Controller
    {
        private readonly IOgrenciService _ogrenciService;
        private readonly IDersService _dersService;
        private readonly IOgrenciDersService _ogrenciDersService;
        private readonly IOgretmenService _ogretmenService;

        public HomeController(
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

        // GET: /Home/Index - Dashboard
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetString("KullaniciRol");
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId");

            // Öğrenci rolü için kendi detay sayfasına yönlendir
            if (rol == "Ogrenci" && kullaniciId.HasValue)
            {
                var ogrenci = await _ogrenciService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogrenci != null)
                    return RedirectToAction("Detay", "Ogrenci", new { id = ogrenci.Id });
            }

            DashboardViewModel model;

            if (rol == "Ogretmen" && kullaniciId.HasValue)
            {
                // Öğretmen paneli — kendi istatistiklerini göster
                var ogretmen = await _ogretmenService.KullaniciIdIleGetirAsync(kullaniciId.Value);
                if (ogretmen == null)
                {
                    TempData["Hata"] = "Öğretmen profiliniz henüz oluşturulmamış. Lütfen yönetici ile iletişime geçin.";
                    return RedirectToAction("YetkiHatasi", "Home");
                }

                var dersler = await _ogretmenService.OgretmeninDersleriniGetirAsync(ogretmen.Id);
                var ogrenciIdleri = await _ogretmenService.OgretmeninOgrenciIdleriniGetirAsync(ogretmen.Id);

                model = new DashboardViewModel
                {
                    ToplamDers = dersler.Count,
                    ToplamOgrenci = ogrenciIdleri.Count,
                    ToplamOgretmen = 0,
                    ToplamAtama = dersler.Sum(d => d.OgrenciDersler.Count),
                    KullaniciAdi = HttpContext.Session.GetString("KullaniciAdi") ?? "",
                    Rol = rol ?? "",
                    SonEklenenOgrenciler = new List<string>(),
                    SonEklenenDersler = dersler
                        .OrderBy(d => d.DersAdi)
                        .Take(5)
                        .Select(d => d.DersAdi)
                        .ToList()
                };
            }
            else
            {
                // Admin dashboard
                var ogrenciler = await _ogrenciService.TumunuGetirAsync();
                var dersler = await _dersService.TumunuGetirAsync();
                var ogretmenler = await _ogretmenService.TumunuGetirAsync();

                model = new DashboardViewModel
                {
                    ToplamOgrenci = ogrenciler.Count,
                    ToplamDers = dersler.Count,
                    ToplamAtama = ogrenciler.Sum(o => o.OgrenciDersler.Count),
                    ToplamOgretmen = ogretmenler.Count,
                    KullaniciAdi = HttpContext.Session.GetString("KullaniciAdi") ?? "",
                    Rol = rol ?? "",
                    SonEklenenOgrenciler = ogrenciler
                        .OrderByDescending(o => o.KayitTarihi)
                        .Take(5)
                        .Select(o => $"{o.Ad} {o.Soyad}")
                        .ToList(),
                    SonEklenenDersler = dersler
                        .OrderByDescending(d => d.EklenmeTarihi)
                        .Take(5)
                        .Select(d => d.DersAdi)
                        .ToList()
                };
            }

            return View(model);
        }

        // GET: /Home/YetkiHatasi
        public IActionResult YetkiHatasi()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
