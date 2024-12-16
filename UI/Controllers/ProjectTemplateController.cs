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
    public class ProjectTemplateController : BaseController
    {
        public ProjectTemplateService service;

        public ProjectTemplateController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new ProjectTemplateService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<ProjectTemplateDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<ProjectTemplateDTO>> dt = new BaseResponseDTO<List<ProjectTemplateDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ProjectTemplateDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<ProjectTemplateDTO> dt = new BaseResponseDTO<ProjectTemplateDTO>();
                dt.Data = new ProjectTemplateDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(ProjectTemplateDTO dto)
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
        public async Task<ActionResult<BaseResponseDTO<List<ProjectTemplateMappingDTO>>>> ProjectTemplateChildDataByParentId(long Id)
        {
            try
            {
                BaseResponseDTO<List<ProjectTemplateMappingDTO>> dt = new BaseResponseDTO<List<ProjectTemplateMappingDTO>>();
                dt.Data = new List<ProjectTemplateMappingDTO>();
                if (Id > 0)
                {
                    dt = service.ProjectTemplateChildDataByParentId(Id);
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
