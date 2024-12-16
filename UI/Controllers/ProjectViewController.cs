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
    public class ProjectViewController : BaseController
    {
        #region constructor
        public ProjectsService projectsService;
        public TaskService taskService;

        public ProjectViewController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            projectsService = new ProjectsService();
            taskService = new TaskService();
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetEventforDates(DateTime start, DateTime End)
        {
            BaseResponseDTO<List<ProjectViewEventDTO>> dt = new BaseResponseDTO<List<ProjectViewEventDTO>>();
            dt = taskService.GetProjectViewEvent(start, End); 
            return Json(dt.Data);
        }

    }
}
