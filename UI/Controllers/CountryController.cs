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
    public class CountryController : BaseController
    {
        public CountryService service;

        public CountryController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new CountryService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<CountryDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<CountryDTO>> dt = new BaseResponseDTO<List<CountryDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<CountryDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<CountryDTO> dt = new BaseResponseDTO<CountryDTO>();
                dt.Data = new CountryDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(CountryDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (dto.Id == 0)
                {
                    dt = await service.SaveAsync(dto);
                }
                else
                {
                    dt = await service.UpdateAsync(dto);
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
