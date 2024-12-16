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
    public class CodeConfigurationController : BaseController
    {
        #region constructor
        public CodeConfigurationService CodeConfigurationService;

        public CodeConfigurationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            CodeConfigurationService = new CodeConfigurationService();
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<CodeConfigurationDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<CodeConfigurationDTO>> dt = new BaseResponseDTO<List<CodeConfigurationDTO>>();

                dt = CodeConfigurationService.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<CodeConfigurationCRUDDTO>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<CodeConfigurationCRUDDTO> dt = new BaseResponseDTO<CodeConfigurationCRUDDTO>();
                dt.Data = new CodeConfigurationCRUDDTO();
                if (Id > 0)
                {
                    dt = CodeConfigurationService.GetById(Id);
                }

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<bool>> CreateUpdate(CodeConfigurationCRUDDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (dto.Id == 0)
                {
                    dt = CodeConfigurationService.SaveAsync(dto);
                }
                else
                {
                    dt = CodeConfigurationService.UpdateAsync(dto);
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
