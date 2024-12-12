using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public BaseResponseDTO<List<ProjectsDTO>> GetAll()
		{
			BaseResponseDTO<List<ProjectsDTO>> dto = new BaseResponseDTO<List<ProjectsDTO>>();
			ProjectsDTO user = new ProjectsDTO();
			QueryResult queryResult = new QueryResult();
			string errorMsg = "No Data Found";

			try
			{

				string sQryResult = queryResult.FAILED;
				List<ProjectsDTO> result = (from a in context.SYS_Projects
											   where a.DeleteStatus == false
											   select new ProjectsDTO
											   {
												   Id = a.Id,
												   UserCode=a.UserCode,
												   ProjectName = a.ProjectName,
												   ProjectDetails = a.ProjectDetails,
												   ProjectDescription = a.ProjectDescription,
												   ProjectManager = (from c in context.SYS_User
																	 join d in context.Users on c.AId equals d.Id
																	 where c.Id == a.User.Id
																	 select d.Othername +' ' +d.Surname).FirstOrDefault(),
												   PlannedHours = a.PlannedHours.ToString("HH:mm"),
												   StartDate = a.StartDate.ToString("yyyy/MM/dd"),
												   EndDate = a.EndDate.ToString("yyyy/MM/dd"),
												   Status = a.Status,
												   StatusDetails = a.StatusDetails,
												   IsVisible = a.IsVisible == true ? "Yes" : "No",
											   }).ToList();

				dto.Data = result;
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
					Sys_Projects.ProjectDescription = dataToUpdate.ProjectDescription;
					Sys_Projects.PlannedHours = dataToUpdate.PlannedHours;
					Sys_Projects.StartDate = dataToUpdate.StartDate;
					Sys_Projects.EndDate = dataToUpdate.EndDate;
					Sys_Projects.Status = dataToUpdate.Status;
					Sys_Projects.StatusDetails = dataToUpdate.StatusDetails;
					Sys_Projects.IsVisible = dataToUpdate.IsVisible;
					context.SYS_Projects.Update(Sys_Projects);
					context.SaveChanges();
					BaseDto.Data = true;
					BaseDto.ErrorMessage = "Project update Successfully";
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.ErrorMessage = "LookUp Type Already Exist";
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
	}
}
