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
    public class TaskController : BaseController
    {
        #region constructor
        public TaskService TaskService;

        public TaskController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            TaskService = new TaskService();
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<TaskDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<TaskDTO>> dt = new BaseResponseDTO<List<TaskDTO>>();
				ISession session = HttpContext.Session;
				string Email = HttpContext.Session.GetString("Username");
				dt = TaskService.GetAll(Email);

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<TaskCRUDDTO>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<TaskCRUDDTO> dt = new BaseResponseDTO<TaskCRUDDTO>();
                dt.Data = new TaskCRUDDTO();
                if (Id > 0)
                {
                    dt = TaskService.GetById(Id);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }


        [HttpPost]
        public ActionResult<BaseResponseDTO<List<DropDown>>> GetAllDropdownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = TaskService.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<bool>> CreateUpdate(TaskCRUDDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (dto.Id == 0)
                {
                    dt = TaskService.SaveAsync(dto);
                }
                else
                {
                    dt = TaskService.UpdateAsync(dto);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }
    }
}
