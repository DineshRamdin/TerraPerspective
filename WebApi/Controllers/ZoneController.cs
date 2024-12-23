using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class ZoneController : ControllerBase
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
                
                bool isAuthenticated = httpContext.User.Identity.IsAuthenticated;

                if (!isAuthenticated)
                {
                    return Unauthorized(new { message = "User is not authenticated" });
                }
                
                string email = httpContext.User.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;

                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest(new { message = "Email not found in token" });
                }                

                BaseResponseDTO<List<ZoneManagementDTO>> dt = new BaseResponseDTO<List<ZoneManagementDTO>>();                

                dt = service.GetAll(email);
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
