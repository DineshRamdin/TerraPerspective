﻿using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Administration;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using System.Data.SqlTypes;
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

        public IActionResult Index()
        {
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
        public async Task<ActionResult<BaseResponseDTO<bool>>> CreateUpdate(GeomertyDataDTO dto)
        {
            try
            {
                BaseResponseDTO<bool> dt = new BaseResponseDTO<bool>();

                var reader = new WKTReader();
                dto.geometry = reader.Read(dto.FeatureGeoJson);
                

                // Log geometry type for debugging
                Console.WriteLine($"Geometry Type: {dto.geometry.GeometryType}");

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
                dt = await service.SaveAsync(dto);

                return Ok(dt);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

    }
}
