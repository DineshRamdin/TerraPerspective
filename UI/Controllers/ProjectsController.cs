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
	public class ProjectsController : BaseController
	{
		#region constructor
		public ProjectsService ProjectsService;
		public MatrixService _MatrixService;


        public ProjectsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
		: base(userManager, signInManager, rolemanager, Dbcontext)
		{
			ProjectsService = new ProjectsService();
			_MatrixService = new MatrixService();
		}
		#endregion

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<List<ProjectsDTO>>> GetAll()
		{
			try
			{
				BaseResponseDTO<List<ProjectsDTO>> dt = new BaseResponseDTO<List<ProjectsDTO>>();
				ISession session = HttpContext.Session;
				string Email = HttpContext.Session.GetString("Username");
				dt = ProjectsService.GetAll(Email);

				return Ok(dt);
			}
			catch (Exception)
			{
				return BadRequest();
			}


		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<ProjectsCRUDDTO>> GetById(long Id)
		{
			try
			{
				BaseResponseDTO<ProjectsCRUDDTO> dt = new BaseResponseDTO<ProjectsCRUDDTO>();
				dt.Data = new ProjectsCRUDDTO();
				if (Id > 0)
				{
					dt = ProjectsService.GetById(Id);
				}

				return Ok(dt);
			}
			catch (Exception)
			{
				return BadRequest();
			}


		}

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<OutputNode>>> GetTree(long Id)
        {
            try
            {
                BaseResponseDTO<List<OutputNode>> tree = new BaseResponseDTO<List<OutputNode>>();
                if (Id == 0)
                {
                    tree = _MatrixService.GetTreeDropdown();
                }
                else
                {

                    tree = ProjectsService.GetTreeDropDownProject(Id);
                }
                return Ok(tree);


            }
            catch (Exception ex)
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
				List = ProjectsService.GetAllDropDownValues();
				return Ok(List);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<bool>> CreateUpdate(ProjectsCRUDDTO dto)
		{
			try
			{
				BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

				if (dto.Id == 0)
				{
					dt = ProjectsService.SaveAsync(dto);
					if (dt.Data == true)
					{
						dt = _MatrixService.SaveProjectM(dto.ProjectMatrix, Convert.ToInt64(dt.ExtData));
					}
				}
				else
				{
					dt = ProjectsService.UpdateAsync(dto);
					if (dt.Data == true)
					{
						dt = _MatrixService.UpdateProjectM(dto.ProjectMatrix, Convert.ToInt64(dt.ExtData));
					}
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
