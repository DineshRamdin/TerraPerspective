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
    public class TestGeomertyDataController : BaseController
    {
        public GeomertyDataService service;
        public TestGeomertyDataController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new GeomertyDataService();
        }

        public IActionResult Index(bool dataAdd = false)
        {
            if (dataAdd)
            {

                string geoJson = "{\"type\":\"Polygon\",\"coordinates\":[[[51.509, -0.08], [51.503, -0.06], [51.51, -0.047], [51.509, -0.08]]]}";
                string zone = "Zone1";

                service.AddGeometryData(zone, geoJson);
            }
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<GeomertyDataDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<GeomertyDataDTO>> dt = new BaseResponseDTO<List<GeomertyDataDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }


        }

        
    }
}
