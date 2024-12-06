using System.Globalization;
using System.Text.RegularExpressions;
using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Common;

namespace UI.Controllers
{
    public class LoginController : BaseController
    {
        #region constructor

        public DeviceService _Deviceservice;
        public UserService _UserService;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            _Deviceservice = new DeviceService();
            _UserService = new UserService();
        }
        #endregion
        public IActionResult Index()
        {

            //var MacAddress = NetworkHelper.GetServerMacAddress();

            //var DeviceData = _Deviceservice.GetByMACAddress(MacAddress);

            //if (DeviceData.Data != null)
            //{
            //    if (DeviceData.Data.DefaultCarousel != null && DeviceData.Data.DefaultCarousel > 0)
            //    {

            //        return RedirectToAction("Preview", "Carousel", new { id = DeviceData.Data.DefaultCarousel });
            //    }
            //    else
            //    {
            //        return View();
            //    }
            //}
            //else
            //{
            return View();
            //}
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ApplicationUser>>> Index(LoginDTO model)
        {
            bool result = false;
            string Message = string.Empty;

            try
            {

                string clientIps = String.Empty;
                var clientIp = Request.Host;
                var MacAddress = NetworkHelper.GetServerMacAddress();


                BaseResponseDTO<ApplicationUser> ncb = await LoginService.Login(model, _signInManager, _userManager, _roleManager, clientIp.Value, MacAddress);
                string image = _UserService.UserProfileImage(ncb.Data.Id);
                string GlobalParamValue = _UserService.GlobalParamValue();
                ncb.Data.ProfileImage = image;
                ncb.Data.GlobalParamValue = GlobalParamValue;

                ISession session = HttpContext.Session;

                if (ncb.QryResult != new QueryResult().FAILED)
                {
                    var key = "b14ca5898a4e4133bbce2ea2315a1916";
                    var encryptedString = AesOperation.EncryptString(key, model.Password);                   
                    session.SetString("OTT", ncb.Data.OTT.ToString());
                    session.SetString("Username", ncb.Data.UserName.ToString());
                    session.SetString("Role", ncb.Role.ToString());
                    session.SetString("RoleId", ncb.RoleId.ToString());
                    session.SetString("UserId", string.IsNullOrEmpty(ncb.Data.Id) ? "" : ncb.Data.Id.ToString());
                    session.SetString("ProfileImage", string.IsNullOrEmpty(ncb.Data.ProfileImage) ? "" : ncb.Data.ProfileImage.ToString());
                }


                string culture = "en-US";

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
               
                session.SetString("Language", culture);

                return Ok(ncb);

            }
            catch (Exception ex)
            {

                return BadRequest();

            }

        }
    }
}
