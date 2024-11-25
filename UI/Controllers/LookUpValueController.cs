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
    public class LookUpValueController : BaseController
    {
        public LookUpValueService service;
        public LookUpValueController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new LookUpValueService();
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult<BaseResponseDTO<List<LookUpValueDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<LookUpValueDTO>> dt = new BaseResponseDTO<List<LookUpValueDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<LookUpValueCRUDDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<LookUpValueCRUDDTO> dt = new BaseResponseDTO<LookUpValueCRUDDTO>();
                dt.Data = new LookUpValueCRUDDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(LookUpValueCRUDDTO dto)
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
