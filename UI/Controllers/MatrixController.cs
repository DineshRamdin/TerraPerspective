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
    public class MatrixController : BaseController
    {
        #region constructor

        public DeviceService _Deviceservice;
        public AccessRightsService _AccessRightsService;
        public RowAccessService _RowAccessService;
        public MatrixService _MatrixService;

        public MatrixController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            _Deviceservice = new DeviceService();
            _AccessRightsService = new AccessRightsService();
            _RowAccessService = new RowAccessService();
            _MatrixService = new MatrixService();
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult <List<CRUDMatrix>> GetTree()
        {
            try
            {
                BaseResponseDTO<List<CRUDMatrix>> tree = new BaseResponseDTO<List<CRUDMatrix>>();
                tree =  _MatrixService.GetTree();
                return Ok(tree);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
        [HttpPost]
        public ActionResult CRUDMatrixList(List<CRUDMatrix> treeData)
        {
            try
            {
                BaseResponseDTO<bool> BaseResponseDTO = new BaseResponseDTO<bool>();
                BaseResponseDTO =  _MatrixService.CRUDM(treeData);
                return Ok(BaseResponseDTO);
            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }
    }
}
