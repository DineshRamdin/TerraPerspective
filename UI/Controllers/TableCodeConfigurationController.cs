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
	public class TableCodeConfigurationController : BaseController
	{
		#region constructor
		public TableCodeConfigurationService TableCodeConfigurationService;

		public TableCodeConfigurationController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
		: base(userManager, signInManager, rolemanager, Dbcontext)
		{
			TableCodeConfigurationService = new TableCodeConfigurationService();
		}
		#endregion

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<List<TableCodeConfigurationDTO>>> GetAll()
		{
			try
			{
				BaseResponseDTO<List<TableCodeConfigurationDTO>> dt = new BaseResponseDTO<List<TableCodeConfigurationDTO>>();

				dt = TableCodeConfigurationService.GetAll();

				return Ok(dt);
			}
			catch (Exception)
			{
				return BadRequest();
			}


		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<TableCodeConfigurationCRUDDTO>> GetById(long Id)
		{
			try
			{
				BaseResponseDTO<TableCodeConfigurationCRUDDTO> dt = new BaseResponseDTO<TableCodeConfigurationCRUDDTO>();
				dt.Data = new TableCodeConfigurationCRUDDTO();
				if (Id > 0)
				{
					dt = TableCodeConfigurationService.GetById(Id);
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
				List = TableCodeConfigurationService.GetAllDropDownValues();
				return Ok(List);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<bool>> CreateUpdate(TableCodeConfigurationCRUDDTO dto)
		{
			try
			{
				BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

				if (dto.Id == 0)
				{
					dt = TableCodeConfigurationService.SaveAsync(dto);
				}
				else
				{
					dt = TableCodeConfigurationService.UpdateAsync(dto);
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
