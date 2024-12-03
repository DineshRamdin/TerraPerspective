using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Microsoft.EntityFrameworkCore;
using DAL.Models.Administration;
using NetTopologySuite.Algorithm;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DAL.Common;

namespace BL.Services.Administration
{
    public class ZoneManagementService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public ZoneManagementService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ZoneManagementDTO>> GetAll()
        {
            BaseResponseDTO<List<ZoneManagementDTO>> dto = new BaseResponseDTO<List<ZoneManagementDTO>>();
            ZoneManagementDTO user = new ZoneManagementDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                List<ZoneManagementDTO> result = context.SYS_ZoneManagement
                                         .Where(a => a.DeleteStatus == false)
                                         .Select(a => new ZoneManagementDTO
                                         {
                                             Id = a.Id,
                                             Zone = a.Zone,
                                             Type = a.Type,
                                             Folder = a.Folder,
                                             ExternalReference = a.ExternalReference,
                                             FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn) // Convert geometry to GeoJSON
                                         })
                                         .ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ZoneManagementDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<List<ZoneManagementDTO>> GetSelectedZoneData(string selectedZone = "")
        {
            BaseResponseDTO<List<ZoneManagementDTO>> dto = new BaseResponseDTO<List<ZoneManagementDTO>>();
            ZoneManagementDTO user = new ZoneManagementDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                List<long> zones = string.IsNullOrWhiteSpace(selectedZone)
                    ? new List<long>()
                    : selectedZone.Split(',')
                                .Select(z => z.Trim())
                                .Where(z => long.TryParse(z, out _))
                                .Select(long.Parse)
                                .ToList();

                List<ZoneManagementDTO> result = context.SYS_ZoneManagement
                                         .Where(a => a.DeleteStatus == false && zones.Contains(a.Id)
                                         //       (
                                         //           zones.Count == 0 ||  // No zones specified, fetch all
                                         //           zones.Contains(a.Id)  // Match Zone if it exists in the list
                                         //       )
                                         )
                                         .Select(a => new ZoneManagementDTO
                                         {
                                             Id = a.Id,
                                             Zone = a.Zone,
                                             Type = a.Type,
                                             Folder = a.Folder,
                                             ExternalReference = a.ExternalReference,
                                             FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn) // Convert geometry to GeoJSON
                                         })
                                         .ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ZoneManagementDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<List<ZoneDataDTO>> GetAllZone()
        {
            BaseResponseDTO<List<ZoneDataDTO>> dto = new BaseResponseDTO<List<ZoneDataDTO>>();

            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                List<ZoneDataDTO> result = context.SYS_ZoneManagement
                                         .Where(a => a.DeleteStatus == false)
                                         .Select(a => new ZoneDataDTO
                                         {
                                             Id = a.Id,
                                             Zone = a.Zone,
                                             Type = a.Type,
                                         })
                                         .ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ZoneDataDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<ZoneManagementDTO> GetById(long Id)
        {
            BaseResponseDTO<ZoneManagementDTO> dto = new BaseResponseDTO<ZoneManagementDTO>();
            ZoneManagementDTO result = new ZoneManagementDTO();
            try
            {
                result = (from a in context.SYS_ZoneManagement
                          where a.DeleteStatus == false && a.Id == Id
                          select new ZoneManagementDTO()
                          {
                              Id = a.Id,
                              Zone = a.Zone,
                              Type = a.Type,
                              Folder = a.Folder,
                              ExternalReference = a.ExternalReference,
                              FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn) // Convert geometry to GeoJSON
                          }).FirstOrDefault();

                if (result == null)
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data = result;
                    dto.QryResult = queryResult.SUCEEDED;
                }
            }
            catch (Exception ex)
            {
                dto.Data = new ZoneManagementDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(ZoneManagementDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                SYS_ZoneManagement DSS = new SYS_ZoneManagement()
                {
                    Zone = dataToSave.Zone,
                    GeomColumn = dataToSave.geometry,
                    Type = dataToSave.Type,
                    Folder = dataToSave.Folder,
                    ExternalReference = dataToSave.ExternalReference,

                };
                context.SYS_ZoneManagement.Add(DSS);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Zone Management Data save Successfully";
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.ErrorMessage = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public async Task<BaseResponseDTO<bool>> UpdateAsync(ZoneManagementDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {

                SYS_ZoneManagement DSS = context.SYS_ZoneManagement.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                DSS.Zone = dataToUpdate.Zone;
                DSS.Type = dataToUpdate.Type;
                DSS.GeomColumn = dataToUpdate.geometry;
                DSS.Folder = dataToUpdate.Folder;
                DSS.ExternalReference = dataToUpdate.ExternalReference;

                context.SYS_ZoneManagement.Update(DSS);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Zone Management Data update Successfully";
                BaseDto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.ErrorMessage = "Error Updating Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<List<ZoneManagementDTO>> GetZoneAutocompleteData(string term)
        {
            BaseResponseDTO<List<ZoneManagementDTO>> dto = new BaseResponseDTO<List<ZoneManagementDTO>>();
            ZoneManagementDTO user = new ZoneManagementDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                List<ZoneManagementDTO> result = context.SYS_ZoneManagement
                                         .Where(a => a.DeleteStatus == false && a.Zone.Contains(term))
                                         .Select(a => new ZoneManagementDTO
                                         {
                                             Id = a.Id,
                                             Zone = a.Zone,
                                             Type = a.Type,
                                             FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn) // Convert geometry to GeoJSON
                                         })
                                         .ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ZoneManagementDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

    }
}
