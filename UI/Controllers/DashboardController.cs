using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using UI.Controllers.Common;
using UI.Resources;


namespace UI.Controllers
{
    public class DashboardController : BaseController
    {

        #region constructor

        public DashboardController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {

        }
        #endregion
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            // Pass any necessary data to the view
            return PartialView("_ChangePassword");
        }
    }
}
