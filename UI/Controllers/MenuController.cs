using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UI.Controllers.Common;

namespace UI.Controllers
{
	public class MenuController : BaseController
	{
		#region constructor

		public DeviceService _Deviceservice;
		public MenuService _MenuService;

		public MenuController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
		: base(userManager, signInManager, rolemanager, Dbcontext)
		{
			_Deviceservice = new DeviceService();
			_MenuService = new MenuService();
		}
		#endregion
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<List<SideMenuDTO>>> GetAll()
		{
			try
			{
				BaseResponseDTO<List<SideMenuDTO>> CusList = new BaseResponseDTO<List<SideMenuDTO>>();
				CusList = _MenuService.GetAll();
				return Ok(CusList);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}
		[HttpPost]
		public ActionResult GetById(string Id)
		{
			try
			{
				if (string.IsNullOrEmpty(Id))
				{
					BaseResponseDTO<SideMenuDTO> dt = new BaseResponseDTO<SideMenuDTO>();
					return Ok(dt);
				}
				else
				{
					BaseResponseDTO<SideMenuDTO> dt = new BaseResponseDTO<SideMenuDTO>();
					dt = _MenuService.GetById(Id);
					return Ok(dt);
				}

			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<bool>> Update(SideMenuDTO dto)
		{
			try
			{
				BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
				BaseResponseDTO = _MenuService.UpdateAsync(dto);

				return Ok(BaseResponseDTO);
			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}
	}
}
