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
    public class AccessRightsController : BaseController
    {
        #region constructor

        public DeviceService _Deviceservice;
        public AccessRightsService _AccessRightsService;
		public RowAccessService _RowAccessService;

        public AccessRightsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            _Deviceservice = new DeviceService();
            _AccessRightsService = new AccessRightsService();
			_RowAccessService = new RowAccessService();
        }
		#endregion

		public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<AccessRightsDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<AccessRightsDTO>> dt = new BaseResponseDTO<List<AccessRightsDTO>>();

                dt = _AccessRightsService.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

		[HttpPost]
		public ActionResult<BaseResponseDTO<AccessRightsDTO>> GetById(string Id)
		{
			try
			{
				BaseResponseDTO<AccessRightsDTO> dt = new BaseResponseDTO<AccessRightsDTO>();
				dt.Data = new AccessRightsDTO();
				if (!string.IsNullOrEmpty(Id))
				{
					dt = _AccessRightsService.GetById(Id);
				}

				return Ok(dt);
			}
			catch (Exception)
			{
				return BadRequest();
			}


		}

		[HttpPost]
		public ActionResult<BaseResponseDTO<MainARDTO>> GetARByRoleId(string Id)
		{
			try
			{
				BaseResponseDTO<MainARDTO> dt = new BaseResponseDTO<MainARDTO>();
				if (string.IsNullOrEmpty(Id))
				{
					return Ok(dt);
				}
				else
				{
					dt=_AccessRightsService.GetARById(Id);
					return Ok(dt);
				}


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}

		public ActionResult<BaseResponseDTO<bool>> GetRowAccess(string ModuleId)
		{
			try
			{
				BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
				dt.Access = _RowAccessService.GetMenuAccessByIds(Guid.Parse(ModuleId));
				return Ok(dt);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}
		public ActionResult<BaseResponseDTO<bool>> createUpdateAR(MainARDTO dataToSave)
		{
			try
			{
				ISession session = HttpContext.Session;
				string Token = HttpContext.Session.GetString("OTT");
				return Ok(_AccessRightsService.SaveUpdateAR(dataToSave,Token));
			}
			catch (Exception ex)
			{
				return BadRequest();
			}
		}

		public ActionResult GetModules()
		{
			try
			{
				BaseResponseDTO<MainARDTO> Ma = new BaseResponseDTO<MainARDTO>();
				Ma.Data = new MainARDTO();
				BaseResponseDTO<List<AccessRightDTO>> al = _AccessRightsService.GetModules();
				Ma.Data.AccessRightDTO = al.Data;
				Ma.Data.RoleName = string.Empty;
				Ma.Data.RoleId = string.Empty;
				return Ok(Ma);


			}
			catch (Exception ex)
			{

				return BadRequest();

			}
		}

	}
}
