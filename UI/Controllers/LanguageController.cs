using System.Globalization;
using System.Resources;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using UI.Controllers.Common;
using UI.Resources;


namespace UI.Controllers
{
    public class LanguageController : Controller
    {

        public IActionResult ChangeLanguage(string culture)
        {
            if (culture == null)
            {
                culture = "en-US";
            };

            HttpContext myContext = this.HttpContext;

            //this code sets .AspNetCore.Culture cookie
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTimeOffset.UtcNow.AddHours(20);
            cookieOptions.IsEssential = true;

            myContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                cookieOptions
            );

            var requestCulture = new RequestCulture(culture);
            var currentCulture = requestCulture.Culture;
            var currentUICulture = requestCulture.UICulture;
            CultureInfo.CurrentCulture = currentCulture;
            CultureInfo.CurrentUICulture = currentUICulture;

            ISession session = HttpContext.Session;
            session.SetString("Language", culture);

            //var resourceManager = new ResourceManager("UI.Resources.SharedResource", typeof(Program).Assembly);
            //var localizedValue = resourceManager.GetString("Dashboard", CultureInfo.GetCultureInfo(culture));


            // Redirect to the referer page or a specific route
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");  // Default to root if Referer header is empty
        }
    }
}
