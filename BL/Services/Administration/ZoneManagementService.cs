﻿using BL.Constants;
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
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Office2010.Excel;

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

        public BaseResponseDTO<List<ZoneManagementDTO>> GetAll(string Email)
        {
            BaseResponseDTO<List<ZoneManagementDTO>> dto = new BaseResponseDTO<List<ZoneManagementDTO>>();
            ZoneManagementDTO user = new ZoneManagementDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                //IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                //string claimsPrincipal = null;
                //string UserGuidId = "";
                //if (httpContextAccessor.HttpContext != null)
                //{
                //	claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                //	UserGuidId = httpContextAccessor.HttpContext.Session.GetString("UserId");
                //}
                //else
                //{
                //	claimsPrincipal = "admin@gmail.com";
                //}

                string AID = context.Users.Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                if (Email != "admin@gmail.com")
                {
                    List<ZoneManagementDTO> result = (from zm in context.SYS_ZoneMatrix
                                                      join gu in context.SYS_GroupMatrixUser on zm.IID equals gu.IID
                                                      join zmn in context.SYS_ZoneManagement on zm.IID equals zmn.Id
                                                      where zmn.DeleteStatus == false
                                                            && gu.IID == UsrId
                                                      select new
                                                      {
                                                          zmn.Id,
                                                          zmn.Zone,
                                                          zmn.Type,
                                                          zmn.Folder,
                                                          zmn.ExternalReference,
                                                          zmn.Color,
                                                          zmn.FillColor,
                                                          zmn.Transparancy,                                                          
                                                          zmn.LineWidth,
                                                          //zmn.GeomColumn
                                                      })
                                .Union(
                                    context.SYS_ZoneManagement
                                        .Where(a => a.DeleteStatus == false && a.CreatedBy.ToString().ToLower() == AID.ToLower())
                                        .Select(a => new
                                        {
                                            a.Id,
                                            a.Zone,
                                            a.Type,
                                            a.Folder,
                                            a.ExternalReference,
                                            a.Color,
                                            a.FillColor,
                                            a.Transparancy,
                                            a.LineWidth,
                                            //a.GeomColumn
                                        })
                                )
                                .AsEnumerable() // Move to client-side processing
                                .Select(x => new ZoneManagementDTO
                                {
                                    Id = x.Id,
                                    Zone = x.Zone,
                                    Type = x.Type,
                                    Folder = x.Folder,
                                    ExternalReference = x.ExternalReference,
                                    FeatureGeoJson = new GeoJsonWriter().Write(context.SYS_ZoneManagement.Where(a => a.Id == x.Id).Select(a => a.GeomColumn)) // Convert geometry to GeoJSON here
                                                                                                                                                              //FeatureGeoJson = new GeoJsonWriter().Write(x.GeomColumn) // Convert geometry to GeoJSON here
                                })
                                .ToList();


                    dto.Data = result;
                }
                else
                {
                    List<ZoneManagementDTO> result = context.SYS_ZoneManagement
                                         .Where(a => a.DeleteStatus == false)
                                         .Select(a => new ZoneManagementDTO
                                         {
                                             Id = a.Id,
                                             Zone = a.Zone,
                                             Type = a.Type,
                                             Folder = a.Folder,
                                             ExternalReference = a.ExternalReference,
                                             Color = a.Color,
                                             FillColor = a.FillColor,
                                             Transparancy = a.Transparancy,
                                             LineWidth = (int)a.LineWidth,
                                             FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn) // Convert geometry to GeoJSON
                                         })
                                         .ToList();
                    dto.Data = result;
                }

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
                                             Color = a.Color,
                                             FillColor = a.FillColor,
                                             Transparancy = a.Transparancy,
                                             LineWidth = (int)a.LineWidth,
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




                IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                string claimsPrincipal = null;
                string UserGuidId = null;
                if (httpContextAccessor.HttpContext != null)
                {
                    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                    UserGuidId = httpContextAccessor.HttpContext.Session.GetString("UserId");
                    if (claimsPrincipal == null)
                    {
                        claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                    }
                }
                else
                {
                    claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                }
                string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                if (claimsPrincipal != "admin@gmail.com")
                {

                    List<ZoneDataDTO> result = (from zm in context.SYS_ZoneMatrix
                                                join gu in context.SYS_GroupMatrixUser on zm.IID equals gu.IID
                                                join zmn in context.SYS_ZoneManagement on zm.IID equals zmn.Id
                                                where zmn.DeleteStatus == false
                                                      && gu.IID == UsrId
                                                //&& zmn.CreatedBy.ToString().ToLower() == UserGuidId
                                                select new
                                                {
                                                    zmn.Id,
                                                    zmn.Zone,
                                                    zmn.Type,
                                                    zmn.Folder,
                                                    zmn.ExternalReference,
                                                }
                                    )
                                    .Union(
                                        context.SYS_ZoneManagement
                                            .Where(a => a.DeleteStatus == false && a.CreatedBy.ToString().ToLower() == UserGuidId)
                                            .Select(a => new
                                            {
                                                a.Id,
                                                a.Zone,
                                                a.Type,
                                                a.Folder,
                                                a.ExternalReference,
                                            })
                                    )
                                    .AsEnumerable() // Move to client-side processing after Union
                                    .Select(x => new ZoneDataDTO
                                    {
                                        Id = x.Id,
                                        Zone = x.Zone,
                                        Type = x.Type,
                                        Folder = x.Folder,
                                        ExternalReference = x.ExternalReference,
                                    })
                                    .ToList();

                    dto.Data = result;
                }
                else
                {
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
                }

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
                              FeatureGeoJson = new GeoJsonWriter().Write(a.GeomColumn), // Convert geometry to GeoJSON
                              Color = (string.IsNullOrEmpty(a.Color)) ? "" : a.Color,
                              FillColor = (string.IsNullOrEmpty(a.Color)) ? "" : a.FillColor,
                              Transparancy = a.Transparancy,
                              LineWidth = (int)a.LineWidth,
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

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {
                //Line Width
                Dd = new DropDown();
                Dd.title = "LineWidth";
                Dd.items = new List<DropDownItem>();

                var enumList = Enum.GetValues(typeof(LineWidth))
                    .Cast<LineWidth>()
                    .Select(e => new DropDownItem { Id = (int)e, text = e.ToString() })
                    .ToList();

                Dd.items.AddRange(enumList);

                Ddl.Add(Dd);                

                dto.Data = Ddl;
                dto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.ErrorMessage = errorMsg;

                dto.QryResult = queryResult.FAILED;
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
                    Color = dataToSave.Color,
                    FillColor = dataToSave.FillColor,
                    Transparancy = dataToSave.Transparancy,
                    LineWidth = (LineWidth)dataToSave.LineWidth,
                };
                context.SYS_ZoneManagement.Add(DSS);
                context.SaveChanges();
                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Zone Management Data save Successfully";
                BaseDto.ExtData = Convert.ToString(DSS.Id);
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
                DSS.Color = dataToUpdate.Color;
                DSS.FillColor = dataToUpdate.FillColor;
                DSS.Transparancy = dataToUpdate.Transparancy;
                DSS.LineWidth = (LineWidth)dataToUpdate.LineWidth;

                context.SYS_ZoneManagement.Update(DSS);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Zone Management Data update Successfully";
                BaseDto.ExtData = Convert.ToString(dataToUpdate.Id);
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

        public static UserMatrixDTO sGetMatrixUserByUserID()
        {
            UserMatrixDTO dt = new UserMatrixDTO();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                string claimsPrincipal = null;
                if (httpContextAccessor.HttpContext != null)
                {
                    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                    if (claimsPrincipal == null)
                    {
                        claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                    }
                }
                else
                {
                    claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                }
                string AID = context.Users.Where(x => x.Email.ToLower() == claimsPrincipal).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                //ml = (from a in context.NPF_SYS_GroupMatrixUser
                //      join b in context.NPF_SYS_GroupMatrix on a.GMID equals b.GMID
                //      where a.IID == UsrId select a.GMID).ToList();
                List<MatrixListDTO> MLL = (from a in context.SYS_GroupMatrixUser
                                           join b in context.SYS_GroupMatrix on a.GMID equals b.GMID
                                           where a.IID == UsrId && a.DeleteStatus == false && b.DeleteStatus == false
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
                dt.ParentIDS = MLL.Select(y => y.GMID).ToList();
                dt.UID = UsrId;
                dt.AUID = AID;
                dt.IDS = ML.Select(y => y.GMID).ToList();

            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public BaseResponseDTO<List<OutputNode>> GetTreeDropDownZone(long Id)
        {
            BaseResponseDTO<List<OutputNode>> BaseDto = new BaseResponseDTO<List<OutputNode>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            List<OutputNode> outputNodes = new List<OutputNode>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();
                ZoneMatrixDTO UMId = sGetMatrixZoneByZoneID(Id);

                mlRoot = (from a in context.SYS_GroupMatrix
                          where UM.ParentIDS.Contains(a.GMID) && a.DeleteStatus == false
                          select new CRUDMatrix()
                          {
                              id = a.GMID.ToString(),
                              text = a.GMDescription,
                              parent = "#",
                              state = new state()
                              {
                                  Checked = false,
                                  Opened = true
                              }
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
                foreach (long gmid in UMId.ParentIDS)
                {
                    ml.Where(x => x.id == gmid.ToString()).Select(w => { w.state.Checked = true; return w; }).ToList();
                }
                List<CRUDMatrix> mlf = new List<CRUDMatrix>();
                mlf = ml.Where(x => x.state.Checked == true).ToList();

                var nodeMap = mlf.ToDictionary(node => node.id, node => new OutputNode
                {
                    Title = node.text,
                    Checked = node.state.Checked,
                    Href = $"#{node.id}",
                    DataAttrs = new List<DataAttr>
                    {
                        new DataAttr { Title = "value", Data = node.id }
                    }
                });

				foreach (var node in mlf)
				{
					if (node.parent != "#")
					{
						if (node.parent != "#" && nodeMap.ContainsKey(node.parent))
						{
							nodeMap[node.parent].Data.Add(nodeMap[node.id]);
						}
					}
				}

				if (mlf.Any(n => n.parent.Contains("#")))
				{
					// Filter the outputNodes to include only nodes with valid parents or no parents
					outputNodes = nodeMap.Values
					.Where(node => mlf.Any(n => n.parent == "#" && n.id == node.Href.TrimStart('#')))
					.ToList();
				}
				else
				{
					// Filter the outputNodes to include only nodes with valid parents or no parents
					outputNodes = nodeMap.Values
						.Where(node => mlf.All(n => n.parent != "#"))
						.ToList();
				}
				BaseDto.Data = outputNodes;
                BaseDto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = outputNodes;
                BaseDto.ErrorMessage = "An Error Occured";
                BaseDto.QryResult = new QueryResult().FAILED;
            }
            return BaseDto;
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


        #endregion
    }
}
