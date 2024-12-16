using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers.API
{
	[Route("api/[controller]")]
	[Authorize]
	public class ZoneController : BaseController
	{
		public ZoneManagementService service;
		public MatrixService _MatrixService;
		private QueryResult _queryResult;

		public ZoneController()
		{
			service = new ZoneManagementService();
			_MatrixService = new MatrixService();

			_queryResult = new QueryResult();
		}

		[HttpGet("GetAllZone")]
		public ActionResult<BaseResponseDTO<List<ZoneManagementDTO>>> GetAll()
        {
            try
			{
                
                // Access HttpContext
                var httpContext = HttpContext;

                // Check if the user is authenticated
                bool isAuthenticated = httpContext.User.Identity.IsAuthenticated;



                BaseResponseDTO<List<ZoneManagementDTO>> dt = new BaseResponseDTO<List<ZoneManagementDTO>>();

				string Email = GetClaimsFromToken("Email");

				dt = service.GetAll(Email);

				return Ok(dt);
			}
			catch (Exception)
			{
				return BadRequest();
			}
		}
	}
}
