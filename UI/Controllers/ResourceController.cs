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
    public class ResourceController : BaseController
    {
        public ResourceService service;
        public ResourceController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new ResourceService();
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult<BaseResponseDTO<List<ResourceDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<ResourceDTO>> dt = new BaseResponseDTO<List<ResourceDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ResourceDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<ResourceDTO> dt = new BaseResponseDTO<ResourceDTO>();
                dt.Data = new ResourceDTO();
                if (Id > 0)
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(ResourceDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (string.IsNullOrEmpty(dto.AId))
                {
                    dt = await service.SaveUser(dto, _userManager, _roleManager);
                }
                else
                {
                    dt = await service.UpdateUser(dto, _userManager, _roleManager);
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
