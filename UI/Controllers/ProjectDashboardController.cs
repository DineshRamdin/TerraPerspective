using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Common;

namespace UI.Controllers
{
	public class ProjectDashboardController : BaseController
	{
		#region constructor
		public TaskService TaskService;
		public ProjectsService ProjectsService;

		public ProjectDashboardController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
		: base(userManager, signInManager, rolemanager, Dbcontext)
		{
			TaskService = new TaskService();
			ProjectsService = new ProjectsService();
		}
		#endregion

		public IActionResult Index(long Id)
		{
			ViewBag.ProjectId = Id;
			return View();
		}


		[HttpPost]
		public ActionResult<BaseResponseDTO<TaskDeatilsForPieChart>> BarChart(long ProjectId)
		{

			try
			{
				ISession session = HttpContext.Session;
				string Email = HttpContext.Session.GetString("Username");
				BaseResponseDTO<TaskDeatilsForPieChart> dt = new BaseResponseDTO<TaskDeatilsForPieChart>();
				dt.Data = new TaskDeatilsForPieChart();
				dt = ProjectsService.GetAllTaskByProjectidForChart(Email, ProjectId);

				return Ok(dt); // Return JSON data for the chart
			}
			catch (Exception ex)
			{
				return BadRequest();
			}
			
		}

	}
}
