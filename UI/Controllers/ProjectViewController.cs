using BL.Services.Administration;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Common;

namespace UI.Controllers
{
    public class ProjectViewController : BaseController
    {
        #region constructor
        public ProjectsService ProjectsService;

        public ProjectViewController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            ProjectsService = new ProjectsService();
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
    }
}
