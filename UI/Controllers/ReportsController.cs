using System.Data;
using BL.Constants;
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
    public class ReportsController : BaseController
    {
        public ReportsService service;
        public ReportsController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager,
            PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new ReportsService();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<ReportsDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<ReportsDTO>> dt = new BaseResponseDTO<List<ReportsDTO>>();
                dt = service.GetAll();
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        public async Task<IActionResult> Preview(string viewName)
        {
            try
            {
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(viewName))
                {
                    dt = await service.GetPreviewById(viewName);

                    if (dt.Rows.Count > 0 && dt.Columns.Contains("ErrorMessage") && dt.Rows[0]["ErrorMessage"] != DBNull.Value)
                    {
                        // Extract the error message from the DataTable
                        string errorMessage = dt.Rows[0]["ErrorMessage"].ToString();
                    }
                }

                // Convert DataTable to List of Dictionaries
                var data = dt.AsEnumerable()
                          .Select(row => dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => row[col]))
                          .ToList();

                return View(data); // Pass data to the view
                //return Ok(dt);
            }
            catch (Exception ex)
            {
                return View("Error", new { message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ReportsDTO>>> GetById(string Id)
        {
            try
            {
                BaseResponseDTO<ReportsDTO> dt = new BaseResponseDTO<ReportsDTO>();
                dt.Data = new ReportsDTO();
                if (!string.IsNullOrEmpty(Id))
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(ReportsDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                if (string.IsNullOrEmpty(dto.Id))
                {
                    dt = await service.SaveReports(dto);
                }
                else
                {
                    dt = await service.UpdateReports(dto);
                }
                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> ReportsDelete(string Id)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();
                if (!string.IsNullOrEmpty(Id))
                {
                    dt = await service.ReportsDelete(Convert.ToInt64(Id));
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
