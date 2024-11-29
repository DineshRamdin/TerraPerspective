using BL.Constants;
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
    public class SystemIconController : BaseController
    {
        public SystemIconService service;
        public SystemIconController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager,
            PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new SystemIconService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<SystemIconDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<SystemIconDTO>> dt = new BaseResponseDTO<List<SystemIconDTO>>();
                dt = service.GetAll();
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        public IActionResult Preview(long Id)
        {
            BaseResponseDTO<List<SystemIconDTO>> dt = new BaseResponseDTO<List<SystemIconDTO>>();
            dt.Data = new List<SystemIconDTO>();
            if (Id > 0)
            {
                dt = service.GetPreviewById(Id);
            }
            return View(dt.Data);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<SystemIconDTO>>> GetById(string Id)
        {
            try
            {
                BaseResponseDTO<SystemIconDTO> dt = new BaseResponseDTO<SystemIconDTO>();
                dt.Data = new SystemIconDTO();
                if (!string.IsNullOrEmpty(Id))
                {
                    dt = service.GetById(Id);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(SystemIconDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (string.IsNullOrEmpty(dto.Id))
                {
                    dt = await service.SaveSystemIcon(dto);
                }
                else
                {
                    dt = await service.UpdateSystemIcon(dto);
                }
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> SystemIconDelete(string Id)
        {
            try
            {

                try
                {
                    BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                    if (!string.IsNullOrEmpty(Id))
                    {
                        dt = await service.SystemIconDelete(Convert.ToInt64(Id));
                    }

                    return Ok(dt);
                }
                catch (Exception)
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }
    }
}
