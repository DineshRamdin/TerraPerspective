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
    public class GlobalParamController : BaseController
    {
        public GlobalParamService service;
        public GlobalParamController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager,
            PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new GlobalParamService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<GlobalParamDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<GlobalParamDTO>> dt = new BaseResponseDTO<List<GlobalParamDTO>>();
                dt = service.GetAll();
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<GlobalParamDTO>>> GetById(string Id)
        {
            try
            {
                BaseResponseDTO<GlobalParamDTO> dt = new BaseResponseDTO<GlobalParamDTO>();
                dt.Data = new GlobalParamDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(GlobalParamDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (string.IsNullOrEmpty(dto.Id))
                {
                    dt = await service.SaveGlobalParam(dto);
                }
                else
                {
                    dt = await service.UpdateGlobalParam(dto);
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
