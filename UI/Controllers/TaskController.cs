using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
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

        public TaskController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
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
                ISession session = HttpContext.Session;
                string Email = HttpContext.Session.GetString("Username");
                List = TaskService.GetAllDropDownValues(Email);
                return Ok(List);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        //[HttpPost]
        //public ActionResult<BaseResponseDTO<bool>> CreateUpdate(TaskCRUDDTO dto)
        //{
        //    try
        //    {
        //        BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

        //        if (dto.Id == 0)
        //        {
        //            dt = TaskService.SaveAsync(dto);
        //        }
        //        else
        //        {
        //            dt = TaskService.UpdateAsync(dto);
        //        }

        //        return Ok(dt);
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }

        //}

        [HttpPost]
        public ActionResult<BaseResponseDTO<bool>> CreateUpdate(TaskCRUDDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                //if (!string.IsNullOrEmpty(dto.Folder))
                //{
                //    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Projects", dto.Folder);

                //    // Check if the folder exists, and create it if not
                //    if (!Directory.Exists(folderPath))
                //    {
                //        Directory.CreateDirectory(folderPath);
                //    }
                //}

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
        public IActionResult AddTask(long Id)
        {
            try
            {
                if (Id > 0)
                {
                    BaseResponseDTO<TaskCRUDDTO> dt = new BaseResponseDTO<TaskCRUDDTO>();
                    dt = TaskService.GetById(Id);
                    return View(dt.Data);
                }
                else
                {
                    TaskCRUDDTO dt = new TaskCRUDDTO();
                    return View(dt);
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

    }
}
