using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BL.Models.Administration.MatrixDTO;

namespace BL.Services.Administration
{
	public class ProjectsService
	{
		private PerspectiveContext context;
		private QueryResult queryResult;

		public ProjectsService()
		{
			context = new PerspectiveContext();
			queryResult = new QueryResult();
		}

		public BaseResponseDTO<List<ProjectsDTO>> GetAll(string Email)
		{
			BaseResponseDTO<List<ProjectsDTO>> dto = new BaseResponseDTO<List<ProjectsDTO>>();
			ProjectsDTO user = new ProjectsDTO();
			QueryResult queryResult = new QueryResult();
			string errorMsg = "No Data Found";

			try
			{

				string sQryResult = queryResult.FAILED;
				string AID = context.Users.Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();
				long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
				if (Email != "admin@gmail.com")
				{
					List<ProjectsDTO> result = (from pm in context.SYS_ProjectsMatrix
												join gu in context.SYS_GroupMatrixUser on pm.IID equals gu.IID
												join prj in context.SYS_Projects on pm.IID equals prj.Id
												where prj.DeleteStatus == false && gu.IID == UsrId
												select new
												{
													prj.Id,
													prj.UserCode,
													prj.ProjectName,
													prj.ProjectDetails,
													prj.ProjectDescription,
													prj.User,
													prj.Status,
													prj.StatusDetails,
													prj.StartDate,
													prj.EndDate,
													prj.PlannedHours,
													prj.IsVisible,
													prj.CreatedBy,
													prj.CreatedDate
												})
							.Union(
								context.SYS_Projects
									.Where(p => p.DeleteStatus == false && p.CreatedBy.ToString().ToLower() == AID.ToLower())
									.Select(p => new
									{
										p.Id,
										p.UserCode,
										p.ProjectName,
										p.ProjectDetails,
										p.ProjectDescription,
										p.User,
										p.Status,
										p.StatusDetails,
										p.StartDate,
										p.EndDate,
										p.PlannedHours,
										p.IsVisible,
										p.CreatedBy,
										p.CreatedDate,
									})
							)
							.AsEnumerable() // Move to client-side processing
							.Select(x => new ProjectsDTO
							{
								Id = x.Id,
								UserCode = x.UserCode,
								ProjectName = x.ProjectName,
								ProjectDetails = x.ProjectDetails,
								ProjectDescription = x.ProjectDescription,
								ProjectManager = (from c in context.SYS_User
												  join d in context.Users on c.AId equals d.Id
												  where c.Id == x.User.Id
												  select d.Othername + ' ' + d.Surname).FirstOrDefault(),
								PlannedHours = x.PlannedHours.ToString(@"hh\:mm"), // Adjust format for TimeSpan
								StartDate = x.StartDate.ToString("yyyy/MM/dd"),
								EndDate = x.EndDate.ToString("yyyy/MM/dd"),
								Status = context.SYS_LookUpValue
											 .Where(lv => lv.Id == x.Status) // Use a different lambda variable
											 .FirstOrDefault()?.Name, // Use null-safe navigation
								StatusDetails = x.StatusDetails,
								IsVisible = x.IsVisible == true ? "Yes" : "No",
							})
							.ToList();




					dto.Data = result;
				}
				else
				{
					List<ProjectsDTO> result = (from a in context.SYS_Projects
												where a.DeleteStatus == false
												select new ProjectsDTO
												{
													Id = a.Id,
													UserCode = a.UserCode,
													ProjectName = a.ProjectName,
													ProjectDetails = a.ProjectDetails,
													ProjectDescription = a.ProjectDescription,
													ProjectManager = (from c in context.SYS_User
																	  join d in context.Users on c.AId equals d.Id
																	  where c.Id == a.User.Id
																	  select d.Othername + ' ' + d.Surname).FirstOrDefault(),
													PlannedHours = a.PlannedHours.ToString("HH:mm"),
													StartDate = a.StartDate.ToString("yyyy/MM/dd"),
													EndDate = a.EndDate.ToString("yyyy/MM/dd"),
													Status = context.SYS_LookUpValue.Where(x => x.Id == a.Status).FirstOrDefault().Name,
													StatusDetails = a.StatusDetails,
													IsVisible = a.IsVisible == true ? "Yes" : "No",
												}).ToList();

					dto.Data = result;
				}
				

				dto.QryResult = queryResult.SUCEEDED;

			}
			catch (Exception ex)
			{
				dto.Data = new List<ProjectsDTO>();
				dto.ErrorMessage = errorMsg;
				dto.QryResult = queryResult.SUCEEDED;
			}

			return dto;
		}

		public BaseResponseDTO<ProjectsCRUDDTO> GetById(long Id)
		{
			BaseResponseDTO<ProjectsCRUDDTO> dto = new BaseResponseDTO<ProjectsCRUDDTO>();
			ProjectsCRUDDTO result = new ProjectsCRUDDTO();
			try
			{
				result = (from a in context.SYS_Projects
						  where a.DeleteStatus == false && a.Id == Id
						  select new ProjectsCRUDDTO()
						  {
							  Id = a.Id,
							  UserCode = a.UserCode,
							  ProjectName = a.ProjectName,
							  ProjectDetails = a.ProjectDetails,
							  ProjectDescription = a.ProjectDescription,
							  AssignTo = a.User.Id,
							  PlannedHours = a.PlannedHours,
							  StartDate = a.StartDate,
							  EndDate = a.EndDate,
							  Status = a.Status,
							  StatusDetails = a.StatusDetails,
							  IsVisible = a.IsVisible,
							  ProjectTemplateId=(a.ProjectTemplate == null)? 0 : a.ProjectTemplate.Id,
							  ProjectColorCode=(string.IsNullOrEmpty(a.ProjectColorCode))? "" : a.ProjectColorCode,
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
				dto.Data = new ProjectsCRUDDTO();
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
				//Lookup Type Value
				string[] values = new string[] { "Project Status" };
				List<SYS_LookUpType> Ltl = context.SYS_LookUpType.Where(x => values.Contains(x.Name) && x.DeleteStatus == false).ToList();
				foreach (SYS_LookUpType lt in Ltl)
				{
					Dd = new DropDown();
					Dd.title = lt.Name;
					List<SYS_LookUpValue> Lvl = context.SYS_LookUpValue.Where(x => x.LookUpType.Id == lt.Id && x.DeleteStatus == false).ToList();
					Dd.items = new List<DropDownItem>();
					foreach (SYS_LookUpValue lv in Lvl)
					{
						DropDownItem Ddi = new DropDownItem();
						Ddi.Id = lv.Id;
						Ddi.text = lv.Name;
						Dd.items.Add(Ddi);
					}

					Ddl.Add(Dd);
				}

				//User
				ApplicationUser result = new ApplicationUser();
				Dd = new DropDown();
				Dd.title = "User";
				Dd.items = new List<DropDownItem>();
				List<SYS_User> User = context.SYS_User.Where(x => x.DeleteStatus == false).ToList();
				foreach (SYS_User lt in User)
				{
					result = context.Users.Where(x => x.Id == lt.AId).FirstOrDefault();

					DropDownItem Ddi = new DropDownItem();
					Ddi.Id = lt.Id;
					Ddi.text = result.Othername + ' ' +result.Surname;
					Dd.items.Add(Ddi);
				}
				Ddl.Add(Dd);

                //ProjectTemplate
                Dd = new DropDown();
                Dd.title = "Project Template";
                Dd.items = new List<DropDownItem>();
                List<SYS_ProjectTemplate> SYS_ProjectTemplate = context.SYS_ProjectTemplate.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_ProjectTemplate lt in SYS_ProjectTemplate)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.ProjectTemplateName;
                    Dd.items.Add(Ddi);
                }
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

		public BaseResponseDTO<bool> SaveAsync(ProjectsCRUDDTO dataToSave)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
			try
			{
				IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
				string claimsPrincipal = null;
				string UserToken = "";
				if (httpContextAccessor.HttpContext != null)
				{
					claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
					UserToken = httpContextAccessor.HttpContext.Session.GetString("OTT");
					if (claimsPrincipal == null)
					{
						claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
					}
				}
				BaseDtoS = new GenerateCode().GCodeMain(UserToken, "SYS_Projects");
				SYS_Projects dt = new SYS_Projects()
				{
					UserCode = BaseDtoS.Data,
					ProjectName = dataToSave.ProjectName,
					ProjectDetails = dataToSave.ProjectDetails,
					ProjectDescription = dataToSave.ProjectDescription,
					User=context.SYS_User.Where(x=>x.Id == dataToSave.AssignTo).FirstOrDefault(),
					PlannedHours = dataToSave.PlannedHours,
					StartDate = dataToSave.StartDate,
					EndDate = dataToSave.EndDate,
					Status = dataToSave.Status,
					StatusDetails = dataToSave.StatusDetails,
					ProjectColorCode = dataToSave.ProjectColorCode,
					ProjectTemplate= context.SYS_ProjectTemplate.Where(x => x.Id == dataToSave.ProjectTemplateId).FirstOrDefault(),
					IsVisible = dataToSave.IsVisible,
				};
				context.SYS_Projects.Add(dt);
				context.SaveChanges();
				BaseDto.ExtData = dt.Id.ToString();
				BaseDto.Data = true;
				BaseDto.ErrorMessage = "Project Added Successfully";
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

		public BaseResponseDTO<bool> UpdateAsync(ProjectsCRUDDTO dataToUpdate)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				if (!context.SYS_LookUpValue.Any(x => x.Name.ToLower() == dataToUpdate.ProjectName.ToLower() && x.DeleteStatus != true
				&& x.Id != dataToUpdate.Id))
				{
					SYS_Projects Sys_Projects = context.SYS_Projects.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

					Sys_Projects.UserCode = dataToUpdate.UserCode;
					Sys_Projects.ProjectName = dataToUpdate.ProjectName;
					Sys_Projects.ProjectDetails = string.IsNullOrEmpty(dataToUpdate.ProjectDetails) ? string.Empty : dataToUpdate.ProjectDetails;
					Sys_Projects.User = context.SYS_User.Where(x => x.Id == dataToUpdate.AssignTo).FirstOrDefault();
                    Sys_Projects.ProjectDescription = dataToUpdate.ProjectDescription;
					Sys_Projects.PlannedHours = dataToUpdate.PlannedHours;
					Sys_Projects.StartDate = dataToUpdate.StartDate;
					Sys_Projects.EndDate = dataToUpdate.EndDate;
					Sys_Projects.Status = dataToUpdate.Status;
					Sys_Projects.StatusDetails = dataToUpdate.StatusDetails;
					Sys_Projects.ProjectColorCode = dataToUpdate.ProjectColorCode;
					Sys_Projects.ProjectTemplate= context.SYS_ProjectTemplate.Where(x => x.Id == dataToUpdate.ProjectTemplateId).FirstOrDefault();
					Sys_Projects.IsVisible = dataToUpdate.IsVisible;
					context.SYS_Projects.Update(Sys_Projects);
					context.SaveChanges();
					BaseDto.Data = true;
					BaseDto.ExtData = Sys_Projects.Id.ToString();
					BaseDto.ErrorMessage = "Project update Successfully";
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.ErrorMessage = "Project Already Exist";
					BaseDto.QryResult = queryResult.FAILED;
				}
			}
			catch (Exception)
			{
				BaseDto.Data = false;
				BaseDto.ErrorMessage = "Error Updating Record";
				BaseDto.QryResult = queryResult.FAILED;
			}
			return BaseDto;
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

        public static ProjectMatrixDTO sGetMatrixZoneByProjectID(long Id)
        {
            ProjectMatrixDTO dt = new ProjectMatrixDTO();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                List<MatrixListDTO> MLL = (from a in context.SYS_ProjectsMatrix
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
                dt.PID = Id;
                dt.PAUID = new GeoJsonWriter().Write(context.SYS_Projects.Where(x => x.Id == Id).Select(x => x.User.Id).FirstOrDefault());

                dt.IDS = ML.Select(y => y.GMID).ToList();


            }
            catch (Exception ex)
            {

            }
            return dt;
        }

        public BaseResponseDTO<List<OutputNode>> GetTreeDropDownProject(long Id)
        {
            BaseResponseDTO<List<OutputNode>> BaseDto = new BaseResponseDTO<List<OutputNode>>();
            List<CRUDMatrix> mlRoot = new List<CRUDMatrix>();
            List<CRUDMatrix> ml = new List<CRUDMatrix>();
            List<OutputNode> outputNodes = new List<OutputNode>();
            PerspectiveContext context = new PerspectiveContext();
            try
            {
                UserMatrixDTO UM = sGetMatrixUserByUserID();
                ProjectMatrixDTO UMId = sGetMatrixZoneByProjectID(Id);

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
                var nodeMap = ml.ToDictionary(node => node.id, node => new OutputNode
                {
                    Title = node.text,
                    Checked = node.state.Checked,
                    Href = $"#{node.id}",
                    DataAttrs = new List<DataAttr>
                    {
                        new DataAttr { Title = "value", Data = node.id }
                    }
                });

                foreach (var node in ml)
                {
                    if (node.parent != "#")
                    {
                        nodeMap[node.parent].Data.Add(nodeMap[node.id]);
                    }
                }

                outputNodes = nodeMap.Values
                    .Where(node => ml.Any(n => n.parent == "#" && n.id == node.Href.TrimStart('#')))
                    .ToList();
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
    }
}
