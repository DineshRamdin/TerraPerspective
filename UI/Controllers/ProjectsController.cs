using BL.Services.Administration;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Common;

namespace UI.Controllers
{
	public class ProjectsController : BaseController
	{
		#region constructor

		public DeviceService _Deviceservice;
		public AccessRightsService _AccessRightsService;
		public RowAccessService _RowAccessService;

		public ProjectsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
		: base(userManager, signInManager, rolemanager, Dbcontext)
		{
			_Deviceservice = new DeviceService();
			_AccessRightsService = new AccessRightsService();
			_RowAccessService = new RowAccessService();
		}
		#endregion

		public IActionResult Index()
		{
			return View();
		}
	}
}
