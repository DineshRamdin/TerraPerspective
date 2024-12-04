using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Data;
using System.Data.SqlTypes;
using UI.Controllers.Common;
using static BL.Models.Administration.MatrixDTO;

namespace UI.Controllers
{
    public class ZoneManagementController : BaseController
    {
        public ZoneManagementService service;
        public MatrixService _MatrixService;
        private QueryResult _queryResult;

        private readonly IWebHostEnvironment _IWebHostEnvironment;
        public ZoneManagementController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext,
            IWebHostEnvironment _webHostEnvironment)
            : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new ZoneManagementService();
            _MatrixService = new MatrixService();

            _IWebHostEnvironment = _webHostEnvironment;
            _queryResult = new QueryResult();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<ZoneManagementDTO>>> GetAll()
        {
            try
            {
                BaseResponseDTO<List<ZoneManagementDTO>> dt = new BaseResponseDTO<List<ZoneManagementDTO>>();

                dt = service.GetAll();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<ZoneManagementDTO>>> GetSelectedZoneData(string selectedZone = "")
        {
            try
            {
                BaseResponseDTO<List<ZoneManagementDTO>> dt = new BaseResponseDTO<List<ZoneManagementDTO>>();

                dt = service.GetSelectedZoneData(selectedZone);

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public ActionResult<BaseResponseDTO<List<ZoneDataDTO>>> GetAllZone()
        {
            try
            {
                BaseResponseDTO<List<ZoneDataDTO>> dt = new BaseResponseDTO<List<ZoneDataDTO>>();

                dt = service.GetAllZone();

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<ZoneManagementDTO>>> GetById(long Id)
        {
            try
            {
                BaseResponseDTO<ZoneManagementDTO> dt = new BaseResponseDTO<ZoneManagementDTO>();
                dt.Data = new ZoneManagementDTO();
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(ZoneManagementDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                var reader = new WKTReader();
                dto.geometry = reader.Read(dto.FeatureGeoJson);

                // Ensure the geometry is a polygon and check orientation
                if (dto.geometry is Polygon polygon)
                {
                    bool Resu = GeometryDataHelper.IsGeometryDataCounterClockwise(polygon);
                    if (!GeometryDataHelper.IsGeometryDataCounterClockwise(polygon))
                    {
                        dto.geometry = GeometryDataHelper.GeometryDataReversePolygon(polygon); // Reverse to clockwise
                    }
                }
                dto.geometry.SRID = 4326;


                if (!string.IsNullOrEmpty(dto.Folder))
                {
                    string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", dto.Folder);

                    // Check if the folder exists, and create it if not
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }


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
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public ActionResult<BaseResponseDTO<List<ZoneManagementDTO>>> GetZoneAutocompleteData(string term)
        {
            BaseResponseDTO<List<ZoneManagementDTO>> dt = new BaseResponseDTO<List<ZoneManagementDTO>>();

            dt = service.GetZoneAutocompleteData(term);

            return Ok(dt);

        }

        public async Task<IActionResult> Fileview(string folder)
        {

            // Define the folder path where the files are stored
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", folder);

            // Create a new DataTable
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("File Name", typeof(string));

            // Get all file names in the folder
            string[] fileNames = Directory.GetFiles(folderPath)
                                          .Select(filePath => Path.GetFileName(filePath))
                                          .ToArray();

            // Populate the DataTable with file names
            foreach (var fileName in fileNames)
            {
                DataRow row = dataTable.NewRow();
                row["File Name"] = fileName;
                dataTable.Rows.Add(row);
            }

            // Optionally: Return the DataTable as JSON for use in the client-side or for further processing
            return View(dataTable);

        }

        [HttpPost]
        public ActionResult<List<CRUDMatrix>> GetTree(long Id)
        {
            try
            {
                BaseResponseDTO<List<CRUDMatrix>> tree = new BaseResponseDTO<List<CRUDMatrix>>();
                tree = _MatrixService.GetTreeForZone(Id);
                return Ok(tree);


            }
            catch (Exception ex)
            {

                return BadRequest();

            }
        }

        [HttpGet]
        public IActionResult DownloadZoneSampleExcel()
        {
            var filePath = Path.Combine(_IWebHostEnvironment.WebRootPath, "SampleFile", "ImportZoneSampleFile.xlsx");
            var fileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "ImportZoneSampleFile.xlsx";

            return PhysicalFile(filePath, fileType, fileName);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponseDTO<bool>>> ImportExcel(IFormFile excelFile)
        {
            BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

            if (excelFile == null || excelFile.Length == 0)
            {
                dt.Data = false;
                dt.ErrorMessage = "Please Upload Valid Excel..!";
                dt.QryResult = _queryResult.FAILED;
                return Ok(dt);
            }
            try
            {
               
                var dtSourceData = ExcelToDataTableService.GetDataTableFromExcel_data(excelFile); // First worksheet

                if (dtSourceData != null && dtSourceData.Rows.Count > 0)
                {
                    foreach (DataRow row in dtSourceData.Rows)
                    {
                        var Type = Convert.ToString(row["Type"]);
                        var Zone = Convert.ToString(row["Zone"]);
                        var ExternalReference = Convert.ToString(row["ExternalReference"]);
                        string FeatureGeoJson = Convert.ToString(row["Map"]);
                        var reader = new WKTReader();
                        var geometry = reader.Read(FeatureGeoJson);

                        // Ensure the geometry is a polygon and check orientation
                        if (geometry is Polygon polygon)
                        {
                            //bool Resu = GeometryDataHelper.IsGeometryDataCounterClockwise(polygon);
                            if (!GeometryDataHelper.IsGeometryDataCounterClockwise(polygon))
                            {
                                geometry = GeometryDataHelper.GeometryDataReversePolygon(polygon); // Reverse to clockwise
                            }
                        }
                        geometry.SRID = 4326;

                        var ZoneManagementDTO = new ZoneManagementDTO
                        {
                            Zone = Zone ?? string.Empty,
                            Type = Type ?? string.Empty,
                            ExternalReference = ExternalReference ?? string.Empty,
                            Folder = string.Empty,
                            geometry = geometry,
                        };
                        await service.SaveAsync(ZoneManagementDTO);
                    }

                    dt.Data = true;
                    dt.ErrorMessage = "Data successfully imported.";
                    dt.QryResult = _queryResult.SUCEEDED;
                    return Ok(dt);
                }
                else
                {
                    dt.Data = false;
                    dt.ErrorMessage = "No Data Found in Excel.";
                    dt.QryResult = _queryResult.FAILED;
                    return Ok(dt);
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }


    }
}
