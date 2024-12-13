using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using DAL.Context;
using DAL.Models;
using DocumentFormat.OpenXml.ExtendedProperties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UI.Controllers.Common;

namespace UI.Controllers
{
    public class CompanyController : BaseController
    {
        public CompanyService service;
        public CompanyController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new CompanyService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Preview(long Id)
        {
            BaseResponseDTO<List<CompanyDTO>> dt = new BaseResponseDTO<List<CompanyDTO>>();
            dt.Data = new List<CompanyDTO>();
            if (Id > 0)
            {
                dt = service.GetPreviewById(Id);
            }
            return View(dt.Data);
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<CompanyDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<CompanyDTO>> dt = new BaseResponseDTO<List<CompanyDTO>>();
                dt = service.GetAll();
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<CompanyDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<CompanyDTO> dt = new BaseResponseDTO<CompanyDTO>();
                dt.Data = new CompanyDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(CompanyDTO dto)
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
