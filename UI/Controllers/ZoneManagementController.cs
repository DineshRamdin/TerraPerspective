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
    public class ZoneManagementController : BaseController
    {
        public ZoneManagementService service;
        public ZoneManagementController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext) : base(userManager, signInManager, rolemanager, Dbcontext)
        {
            service = new ZoneManagementService();
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

    }
}
