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
                                                       FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn) // Convert geometry to GeoJSON
                                                   }).AsEnumerable() // Switch to client-side processing
    .Select(a => new GeomertyDataDTO
    {
        Id = a.Id,
        Zone = a.Zone,
        FeatureGeoJson = ReverseCoordinates(a.FeatureGeoJson) // Reverse coordinates after data is loaded
    }).ToList();

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

        public void AddGeometryData(string zone, string geoJson)
        {
            // Step 1: Deserialize GeoJSON to Geometry
            var geoJsonReader = new GeoJsonReader();
            Geometry geometry = geoJsonReader.Read<Geometry>(geoJson);  //

            // Ensure polygon orientation (only for polygons or multi-polygons)
            if (geometry is Polygon polygon)
            {
                if (!Orientation.IsCCW(polygon.Coordinates))
                {
                    polygon = (Polygon)polygon.Reverse(); // Reverse to make it counter-clockwise
                }
                geometry = polygon;
            }
            else if (geometry is MultiPolygon multiPolygon)
            {
                for (int i = 0; i < multiPolygon.NumGeometries; i++)
                {
                    Polygon part = (Polygon)multiPolygon.GetGeometryN(i);
                    if (!Orientation.IsCCW(part.Coordinates))
                    {
                        part = (Polygon)part.Reverse();
                        multiPolygon.Geometries[i] = part;
                    }
                }
                geometry = multiPolygon;
            }

            // Step 2: Create a new GeomertyData entity
            var newGeometryData = new SYS_GeomertyData
            {
                Zone = zone,
                GeomColumn = geometry  // Set the geometry (SQL Server geometry type)
            };

            // Step 3: Add to context and save changes
            context.SYS_GeomertyData.Add(newGeometryData);
            context.SaveChanges();
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
