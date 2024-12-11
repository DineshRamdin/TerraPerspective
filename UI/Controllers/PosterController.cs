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
    public class PosterController : BaseController
    {
        public PosterService service;
        public PosterController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new PosterService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Preview(long Id)
        {
            BaseResponseDTO<List<PosterDTO>> dt = new BaseResponseDTO<List<PosterDTO>>();
            dt.Data = new List<PosterDTO>();
            if (Id > 0)
            {
                dt = service.GetPreviewById(Id);
            }
            return View(dt.Data);
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<PosterDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<PosterDTO>> dt = new BaseResponseDTO<List<PosterDTO>>();
                dt = service.GetAll();
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<PosterDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<PosterDTO> dt = new BaseResponseDTO<PosterDTO>();
                dt.Data = new PosterDTO();
                dt.Data.Status = true;
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
        public ActionResult<BaseResponseDTO<List<DropDown>>> GetAllDropdownValues()
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(PosterDTO dto)
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
