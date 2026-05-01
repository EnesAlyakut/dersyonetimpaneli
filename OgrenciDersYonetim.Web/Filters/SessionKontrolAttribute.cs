using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OgrenciDersYonetim.Web.Filters
{
    /// <summary>
    /// Session kontrolü yapan custom Authorization Filter.
    /// Login olmadan sayfalara erişimi engeller.
    /// </summary>
    public class SessionKontrolAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var kullaniciId = context.HttpContext.Session.GetInt32("KullaniciId");
            if (kullaniciId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }
            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Sadece Admin rolüne izin veren filter.
    /// </summary>
    public class AdminKontrolAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var kullaniciId = context.HttpContext.Session.GetInt32("KullaniciId");
            var rol = context.HttpContext.Session.GetString("KullaniciRol");

            if (kullaniciId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (rol != "Admin")
            {
                context.Result = new RedirectToActionResult("YetkiHatasi", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Admin veya Ogretmen rolüne izin veren filter.
    /// </summary>
    public class AdminOgretmenKontrolAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var kullaniciId = context.HttpContext.Session.GetInt32("KullaniciId");
            var rol = context.HttpContext.Session.GetString("KullaniciRol");

            if (kullaniciId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (rol != "Admin" && rol != "Ogretmen")
            {
                context.Result = new RedirectToActionResult("YetkiHatasi", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
