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
    public class UserController : BaseController
    {
        public UserService service;
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
                : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new UserService();
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult<BaseResponseDTO<List<UserDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<UserDTO>> dt = new BaseResponseDTO<List<UserDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<UserDTO>>> GetById(string Id)
        {
            try
            {
                BaseResponseDTO<UserDTO> dt = new BaseResponseDTO<UserDTO>();
                dt.Data = new UserDTO();
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
        public ActionResult<BaseResponseDTO<List<DropDownLogin>>> getUserDropDown()
        {
            try
            {
                BaseResponseDTO<List<DropDownLogin>> List = new BaseResponseDTO<List<DropDownLogin>>();
                List = service.GetAllDropDownValues();
                return Ok(List);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(UserDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (string.IsNullOrEmpty(dto.Id))
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

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> ResetUserPassword(string Id)
        {
            try
            {

                try
                {
                    BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                    if (!string.IsNullOrEmpty(Id))
                    {
                        dt = await service.ResetUserPassword(Id, _userManager);
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
