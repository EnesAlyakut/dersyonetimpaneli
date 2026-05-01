using Microsoft.AspNetCore.Mvc;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Web.Models.ViewModels;
using OgrenciDersYonetim.Web.Filters;

namespace OgrenciDersYonetim.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IKullaniciService _kullaniciService;

        public AccountController(IKullaniciService kullaniciService)
        {
            _kullaniciService = kullaniciService;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            // Zaten giriş yapmışsa dashboard'a yönlendir
            if (HttpContext.Session.GetInt32("KullaniciId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var kullanici = await _kullaniciService.GirisYapAsync(model.KullaniciAdi, model.Sifre);

            if (kullanici == null)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            // Session'a kullanıcı bilgilerini kaydet
            HttpContext.Session.SetInt32("KullaniciId", kullanici.Id);
            HttpContext.Session.SetString("KullaniciAdi", kullanici.KullaniciAdi);
            HttpContext.Session.SetString("KullaniciAd", kullanici.Ad);
            HttpContext.Session.SetString("KullaniciSoyad", kullanici.Soyad);
            HttpContext.Session.SetString("KullaniciRol", kullanici.Rol);

            // Rol bazlı yönlendirme
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("KullaniciId") != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Benzersizlik kontrolleri
            if (await _kullaniciService.KullaniciAdiMevcutMuAsync(model.KullaniciAdi))
            {
                ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten kullanılıyor.");
                return View(model);
            }

            if (await _kullaniciService.EmailMevcutMuAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            var yeniKullanici = new Kullanici
            {
                Ad = model.Ad,
                Soyad = model.Soyad,
                KullaniciAdi = model.KullaniciAdi,
                Email = model.Email,
                Rol = model.Rol,
                KayitTarihi = DateTime.Now
            };

            var basarili = await _kullaniciService.KayitOlAsync(yeniKullanici, model.Sifre);

            if (!basarili)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu. Tekrar deneyiniz.");
                return View(model);
            }

            TempData["Basari"] = "Kayıt başarılı! Şimdi giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        // GET: /Account/Logout
        [SessionKontrol]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Basari"] = "Başarıyla çıkış yapıldı.";
            return RedirectToAction("Login");
        }

        // GET: /Account/SifreYonetimi - Admin şifre yönetimi
        [SessionKontrol]
        [AdminKontrol]
        public async Task<IActionResult> SifreYonetimi()
        {
            var kullanicilar = await _kullaniciService.TumunuGetirAsync();
            ViewBag.Kullanicilar = kullanicilar;
            return View(new SifreAyarlaViewModel());
        }

        // POST: /Account/SifreYonetimi
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionKontrol]
        [AdminKontrol]
        public async Task<IActionResult> SifreYonetimi(SifreAyarlaViewModel model)
        {
            var kullanicilar = await _kullaniciService.TumunuGetirAsync();
            ViewBag.Kullanicilar = kullanicilar;

            if (!ModelState.IsValid)
                return View(model);

            var kullanici = await _kullaniciService.IdIleGetirAsync(model.KullaniciId);
            if (kullanici == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(model);
            }

            await _kullaniciService.SifreDegistirAsync(model.KullaniciId, model.YeniSifre);

            TempData["Basari"] = $"{kullanici.Ad} {kullanici.Soyad} ({kullanici.KullaniciAdi}) kullanıcısının şifresi başarıyla güncellendi.";
            return RedirectToAction("SifreYonetimi");
        }

        // GET: /Account/Profil
        [SessionKontrol]
        public async Task<IActionResult> Profil()
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId")!.Value;
            var kullanici = await _kullaniciService.IdIleGetirAsync(kullaniciId);

            if (kullanici == null)
                return RedirectToAction("Login");

            var model = new ProfilViewModel
            {
                Id = kullanici.Id,
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                Email = kullanici.Email,
                KullaniciAdi = kullanici.KullaniciAdi,
                Rol = kullanici.Rol,
                KayitTarihi = kullanici.KayitTarihi
            };

            return View(model);
        }

        // POST: /Account/Profil
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionKontrol]
        public async Task<IActionResult> Profil(ProfilViewModel model)
        {
            var kullaniciId = HttpContext.Session.GetInt32("KullaniciId")!.Value;

            // Şifre değiştirme alanlarını validate'den çıkar (doldurmak zorunlu değil)
            ModelState.Remove("MevcutSifre");
            ModelState.Remove("YeniSifre");
            ModelState.Remove("YeniSifreTekrar");

            if (!ModelState.IsValid)
                return View(model);

            var kullanici = await _kullaniciService.IdIleGetirAsync(kullaniciId);
            if (kullanici == null) return RedirectToAction("Login");

            // Temel bilgileri güncelle
            kullanici.Ad = model.Ad;
            kullanici.Soyad = model.Soyad;
            kullanici.Email = model.Email;

            // Şifre değiştirme isteği varsa
            if (!string.IsNullOrEmpty(model.YeniSifre))
            {
                if (string.IsNullOrEmpty(model.MevcutSifre))
                {
                    ModelState.AddModelError("MevcutSifre", "Mevcut şifrenizi giriniz.");
                    return View(model);
                }

                var mevcutHash = _kullaniciService.SifreyiHashle(model.MevcutSifre);
                if (kullanici.SifreHash != mevcutHash)
                {
                    ModelState.AddModelError("MevcutSifre", "Mevcut şifre hatalı.");
                    return View(model);
                }

                await _kullaniciService.SifreDegistirAsync(kullaniciId, model.YeniSifre);
            }

            await _kullaniciService.GuncelleAsync(kullanici);

            // Session'ı güncelle
            HttpContext.Session.SetString("KullaniciAd", kullanici.Ad);
            HttpContext.Session.SetString("KullaniciSoyad", kullanici.Soyad);

            TempData["Basari"] = "Profil bilgileriniz başarıyla güncellendi.";
            return RedirectToAction("Profil");
        }
    }
}
