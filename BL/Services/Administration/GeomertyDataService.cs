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
    public class GeomertyDataService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public GeomertyDataService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<GeomertyDataDTO>> GetAll()
        {
            BaseResponseDTO<List<GeomertyDataDTO>> dto = new BaseResponseDTO<List<GeomertyDataDTO>>();
            GeomertyDataDTO user = new GeomertyDataDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                List<GeomertyDataDTO> result = context.SYS_GeomertyData
                                         .Where(a => a.DeleteStatus == false)
                                         .Select(a => new GeomertyDataDTO
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
                dto.Data = new List<GeomertyDataDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<List<GeomertyDataDTO>> GetSelectedZoneGeomertyData(string selectedZone = "")
        {
            BaseResponseDTO<List<GeomertyDataDTO>> dto = new BaseResponseDTO<List<GeomertyDataDTO>>();
            GeomertyDataDTO user = new GeomertyDataDTO();
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

                List<GeomertyDataDTO> result = context.SYS_GeomertyData
                                         .Where(a => a.DeleteStatus == false && zones.Contains(a.Id)
                                         //       (
                                         //           zones.Count == 0 ||  // No zones specified, fetch all
                                         //           zones.Contains(a.Id)  // Match Zone if it exists in the list
                                         //       )
                                         )
                                         .Select(a => new GeomertyDataDTO
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
                dto.Data = new List<GeomertyDataDTO>();
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

                List<ZoneDataDTO> result = context.SYS_GeomertyData
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

        public BaseResponseDTO<GeomertyDataDTO> GetById(long Id)
        {
            BaseResponseDTO<GeomertyDataDTO> dto = new BaseResponseDTO<GeomertyDataDTO>();
            GeomertyDataDTO result = new GeomertyDataDTO();
            try
            {
                result = (from a in context.SYS_GeomertyData
                          where a.DeleteStatus == false && a.Id == Id
                          select new GeomertyDataDTO()
                          {
                              Id = a.Id,
                              Zone = a.Zone,
                              Type = a.Type,
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
                dto.Data = new GeomertyDataDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(GeomertyDataDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                SYS_GeomertyData DSS = new SYS_GeomertyData()
                {
                    Zone = dataToSave.Zone,
                    GeomColumn = dataToSave.geometry,
                    Type = dataToSave.Type

                };
                context.SYS_GeomertyData.Add(DSS);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Geomerty Data save Successfully";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(GeomertyDataDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {

                SYS_GeomertyData DSS = context.SYS_GeomertyData.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                DSS.Zone = dataToUpdate.Zone;
                DSS.Type = dataToUpdate.Type;
                DSS.GeomColumn = dataToUpdate.geometry;

                context.SYS_GeomertyData.Update(DSS);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Geomerty Data update Successfully";
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

    }
}
