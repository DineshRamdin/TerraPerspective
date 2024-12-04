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
using static BL.Models.Administration.MatrixDTO;

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
                                             Folder = a.Folder,
                                             ExternalReference = a.ExternalReference,
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
                CRUDZM(dataToSave.TreeData, DSS.Id);
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

                CRUDZM(dataToUpdate.TreeData, dataToUpdate.Id);
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

        #region Zone Metrix

        public BaseResponseDTO<List<CRUDMatrix>> GetTreeForZone(long Id)
        {
            BaseResponseDTO<List<CRUDMatrix>> BaseDto = new BaseResponseDTO<List<CRUDMatrix>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                ZoneMatrixDTO UM = sGetMatrixZoneByZoneID(Id);

                mlRoot = (from a in context.SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                          }).ToList();
                ml = (from a in context.SYS_GroupMatrix
                      where UM.IDS.Contains(a.GMID) && !UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                      select new CRUDMatrix()
                      {
                          id = a.GMID.ToString(),
                          text = a.GMDescription,
                          parent = a.ParentGMID.ToString(),
                      }).ToList();
                ml.AddRange(mlRoot);
                BaseDto.Data = ml;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = ml;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public static ZoneMatrixDTO sGetMatrixZoneByZoneID(long Id)
        {
            ZoneMatrixDTO dt = new ZoneMatrixDTO();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                List<MatrixListDTO> MLL = (from a in context.SYS_ZoneMatrix
                                           join b in context.SYS_GroupMatrix on a.GMID equals b.GMID
                                           where a.IID == Id && a.DeleteStatus == false && b.DeleteStatus == false
                                           select new MatrixListDTO()
                                           {
                                               GMID = a.GMID,
                                               GMDescription = b.GMDescription
                                           }).ToList();
                List<MatrixListDTO> ML = new List<MatrixListDTO>();
                ML.AddRange(MLL);
                foreach (MatrixListDTO m in MLL)
                {
                    ML.AddRange(GetMatrixChild(m.GMID));
                }

                if (Id == 0 && dt.IDS.Count == 0)
                {
                    ML.AddRange(GetMatrixChild(3));
                }

                dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
                dt.ZID = Id;
                dt.ZAUID = new GeoJsonWriter().Write(context.SYS_ZoneManagement.Where(x => x.Id == Id).Select(x => x.GeomColumn).FirstOrDefault());

                dt.IDS = ML.Select(y => y.GMID).ToList();


            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public static List<MatrixListDTO> GetMatrixChild(long GMID)
        {
            List<MatrixListDTO> ml = new List<MatrixListDTO>();
            List<MatrixListDTO> MLL = new List<MatrixListDTO>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                ml = (from a in context.SYS_GroupMatrix
                      where a.ParentGMID == GMID && a.DeleteStatus == false
                      select new MatrixListDTO()
                      {
                          GMID = a.GMID,
                          GMDescription = a.GMDescription
                      }).ToList();
                if (ml.Count > 0)
                {
                    MLL.AddRange(ml);
                    foreach (MatrixListDTO m in ml)
                    {
                        MLL.AddRange(GetMatrixChild(m.GMID));
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return MLL;
        }

        public BaseResponseDTO<bool> CRUDZM(List<CRUDMatrix> dt, long Id)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                ZoneMatrixDTO ZM = sGetMatrixZoneByZoneID(Id);
                bool Incoming = HandleIncoming(dt);
                bool Update = HandleUpdate(dt);
                bool Removed = HandleRemovedForZone(ZM, dt);
                BaseDto.Data = true;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
                if (!Incoming && !Removed)
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "An Error Occured";
                    BaseDto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception)
            {
                BaseDto.Data = false;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
        }

        public bool HandleRemovedForZone(ZoneMatrixDTO ZM, List<CRUDMatrix> dt)
        {
            List<long> Removed = new List<long>();
            bool status = false;
            try
            {
                List<long> Existing = GetExisting(dt);
                Removed = ZM.IDS;
                Removed.RemoveAll(x => Existing.Contains(x));
                status = true;
                status = DeleteMatrix(Removed);
            }
            catch (Exception ex)
            {

            }
            return status;
        }

        public bool HandleIncoming(List<CRUDMatrix> dt)
        {
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> Incoming = new List<CRUDMatrix>();
            bool status = false;
            try
            {
                Incoming = dt.Where(x => x.id.Contains("j1_")).ToList();
                mlRoot = Incoming.Where(x => !x.parent.Contains("j1_")).ToList();
                foreach (var matrix in mlRoot)
                {
                    long result = CreateMatrix(Convert.ToInt64(matrix.parent), matrix.text);
                    if (result > 0)
                    {
                        Incoming.Where(x => x.parent == matrix.id).Select(c => { c.parent = result.ToString(); return c; }).ToList();
                        Incoming.Where(x => x.id == matrix.id).Select(c => { c.id = result.ToString(); return c; }).ToList();
                    }
                }
                if (Incoming.Where(x => x.id.Contains("j1_")).ToList().Count > 0)
                {
                    status = HandleIncoming(Incoming);
                    if (!status)
                    {
                        return status;
                    }
                }
                status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        public bool HandleUpdate(List<CRUDMatrix> dt)
        {
            List<CRUDMatrix> update = new List<CRUDMatrix>();
            PerspectiveContext context = new PerspectiveContext();
            bool status = false;
            try
            {
                update = dt.Where(x => !x.id.Contains("j1_")).ToList();
                foreach (CRUDMatrix u in update)
                {
                    SYS_GroupMatrix gm = context.SYS_GroupMatrix.Where(x => x.GMID == Convert.ToInt64(u.id)).FirstOrDefault();
                    if (gm.GMDescription != u.text)
                    {
                        gm.GMDescription = u.text;
                        context.SYS_GroupMatrix.Update(gm);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return status;
        }
        
        public List<long> GetExisting(List<CRUDMatrix> dt)
        {
            List<long> Existing = new List<long>();
            try
            {
                foreach (string e in dt.Where(x => !x.id.Contains("j1_")).Select(y => y.id).ToList())
                {
                    Existing.Add(Convert.ToInt64(e));
                }
            }
            catch (Exception ex)
            {

            }
            return Existing;
        }
        public long CreateMatrix(long pgmid, string gmdes)
        {
            PerspectiveContext context = new PerspectiveContext();
            long id = 0;
            try
            {
                SYS_GroupMatrix gm = new SYS_GroupMatrix()
                {
                    ParentGMID = pgmid,
                    GMDescription = gmdes,
                    Remarks = string.Empty,
                    IsCompany = true,
                    CompanyCode = string.Empty,
                    LegalName = string.Empty,
                };
                context.SYS_GroupMatrix.Add(gm);
                context.SaveChanges();
                id = gm.GMID;
            }
            catch (Exception ex)
            {

            }
            return id;
        }
        public bool DeleteMatrix(List<long> Removed)
        {
            PerspectiveContext context = new PerspectiveContext();
            bool status = false;
            try
            {
                foreach (long Id in Removed)
                {
                    SYS_GroupMatrix gm = context.SYS_GroupMatrix.Where(x => x.GMID == Id).FirstOrDefault();
                    gm.DeleteStatus = true;
                    context.SYS_GroupMatrix.Update(gm);
                    context.SaveChanges();
                }
                status = true;
            }
            catch (Exception ex)
            {

            }
            return status;
        }

        #endregion
    }
}
