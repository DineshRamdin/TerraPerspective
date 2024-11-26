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
    public class TestingController : BaseController
    {
        public TestingService service;
        public TestingController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager,
            PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new TestingService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<TestingDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<TestingDTO>> dt = new BaseResponseDTO<List<TestingDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<TestingCRUDDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<TestingCRUDDTO> dt = new BaseResponseDTO<TestingCRUDDTO>();
                dt.Data = new TestingCRUDDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(TestingCRUDDTO dto)
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

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> TestingDelete(string Id)
        {
            try
            {

                try
                {
                    BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                    if (!string.IsNullOrEmpty(Id))
                    {
                        dt = await service.TestingDelete(Convert.ToInt64(Id), _userManager);
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
    }
}
