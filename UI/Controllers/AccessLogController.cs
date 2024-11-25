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
    public class AccessLogController : BaseController
    {
        public AccessLogService service;
        public AccessLogController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new AccessLogService();
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CheckInOut()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<AccessLogDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<AccessLogDTO>> dt = new BaseResponseDTO<List<AccessLogDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<AccessLogDTO>>> GetAllActiveSession()
        {
            try
            {
                BaseResponseDTO<List<AccessLogDTO>> dt = new BaseResponseDTO<List<AccessLogDTO>>();

                dt = service.GetAllActiveSession();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }



        [HttpPost]
        public ActionResult<BaseResponseDTO<List<DropDown>>> GetAllDropDownValues()
        {
            try
            {
                BaseResponseDTO<List<DropDown>> List = new BaseResponseDTO<List<DropDown>>();
                List = service.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CheckInAccess(CheckInAccessLogDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                dt = await service.Checkin(dto, _signInManager, _userManager);
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CheckOutAccess(CheckOutAccessLogDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                dt = await service.Checkout(dto);
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
