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
                                             Type=a.Type,
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



        private string ReverseCoordinates(string geoJson)
        {
            try
            {
                // Deserialize the GeoJSON to a dynamic object
                var geoJsonObject = JsonConvert.DeserializeObject<dynamic>(geoJson);

                // Check if the GeoJSON is a FeatureCollection or a direct geometry
                if (geoJsonObject.features != null)
                {
                    // Iterate through features if it's a FeatureCollection
                    foreach (var feature in geoJsonObject.features)
                    {
                        if (feature.geometry != null && feature.geometry.coordinates != null)
                        {
                            ReverseCoordinatesInGeometry(feature.geometry);
                        }
                    }
                }
                else if (geoJsonObject.coordinates != null)
                {
                    // Directly reverse coordinates for a single geometry
                    ReverseCoordinatesInGeometry(geoJsonObject);
                }

                // Serialize the modified GeoJSON object back to a string
                return geoJsonObject.ToString();
            }
            catch (Exception ex)
            {
                // Log or handle the error as needed
                Console.WriteLine($"Error reversing coordinates: {ex.Message}");
                return geoJson; // Return the original GeoJSON if there's an error
            }
        }

        // Helper method to reverse coordinates in the geometry
        private void ReverseCoordinatesInGeometry(dynamic geometry)
        {
            var coordinates = geometry.coordinates;

            // Ensure it's an array of arrays
            if (coordinates is JArray)
            {
                for (int i = 0; i < coordinates.Count; i++)
                {
                    var ring = coordinates[i];

                    // Reverse each coordinate pair (latitude, longitude -> longitude, latitude)
                    for (int j = 0; j < ring.Count; j++)
                    {
                        var point = ring[j];
                        if (point.Count == 2)
                        {
                            var reversedPoint = new JArray(point[1], point[0]); // Reverse lat/lon to lon/lat
                            ring[j] = reversedPoint;
                        }
                    }
                }
            }
        }
    }
}
