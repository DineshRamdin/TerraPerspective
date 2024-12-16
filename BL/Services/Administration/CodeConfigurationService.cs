using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Context;
using DAL.Models.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BL.Services.Administration
{
	public class CodeConfigurationService
	{
		private PerspectiveContext context;
		private QueryResult queryResult;

		public CodeConfigurationService()
		{
			context = new PerspectiveContext();
			queryResult = new QueryResult();
		}

		public BaseResponseDTO<List<CodeConfigurationDTO>> GetAll()
		{
			BaseResponseDTO<List<CodeConfigurationDTO>> dto = new BaseResponseDTO<List<CodeConfigurationDTO>>();
			CodeConfigurationDTO user = new CodeConfigurationDTO();
			QueryResult queryResult = new QueryResult();
			string errorMsg = "No Data Found";

			try
			{

				string sQryResult = queryResult.FAILED;
				List<CodeConfigurationDTO> result = (from a in context.SYS_CodeConfiguration
											where a.DeleteStatus == false
											select new CodeConfigurationDTO
											{
												Id = a.Id,
												Name = a.Name,
												Date = a.Date == true ? "Yes" : "No",
												DateFormat = a.DateFormat,
												Month = a.Month == true ? "Yes" : "No",
												MonthFormat = a.MonthFormat,
												Year = a.Year == true ? "Yes" : "No",
												YearFormat = a.YearFormat,
												UsePrefix = a.UsePrefix == true ? "Yes" : "No",
												Reset = a.Reset == true ? "Yes" : "No",
												ResetConfig = a.ResetConfig,
												PaddingNo = a.PaddingNo,
												Comment = a.Comment,
											}).ToList();

				dto.Data = result;
				dto.QryResult = queryResult.SUCEEDED;

			}
			catch (Exception ex)
			{
				dto.Data = new List<CodeConfigurationDTO>();
				dto.ErrorMessage = errorMsg;
				dto.QryResult = queryResult.SUCEEDED;
			}

			return dto;
		}

		public BaseResponseDTO<CodeConfigurationCRUDDTO> GetById(long Id)
		{
			BaseResponseDTO<CodeConfigurationCRUDDTO> dto = new BaseResponseDTO<CodeConfigurationCRUDDTO>();
			CodeConfigurationCRUDDTO result = new CodeConfigurationCRUDDTO();
			try
			{
				result = (from a in context.SYS_CodeConfiguration
						  where a.DeleteStatus == false && a.Id == Id
						  select new CodeConfigurationCRUDDTO()
						  {
							  Id = a.Id,
                              Name = a.Name,
                              Date = a.Date,
                              DateFormat = a.DateFormat,
                              Month = a.Month,
                              MonthFormat = a.MonthFormat,
                              Year = a.Year,
                              YearFormat = a.YearFormat,
                              UsePrefix = a.UsePrefix,
                              Reset = a.Reset,
                              ResetConfig = a.ResetConfig,
                              PaddingNo = a.PaddingNo,
                              Comment = a.Comment,
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
				dto.Data = new CodeConfigurationCRUDDTO();
				dto.QryResult = new QueryResult().FAILED;
			}
			return dto;
		}

		public BaseResponseDTO<bool> SaveAsync(CodeConfigurationCRUDDTO dataToSave)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
			try
			{
				SYS_CodeConfiguration dt = new SYS_CodeConfiguration()
				{
					Name = dataToSave.Name,
					Date = dataToSave.Date,
					DateFormat = dataToSave.DateFormat,
					Month = dataToSave.Month,
					MonthFormat = dataToSave.MonthFormat,
					Year = dataToSave.Year,
					YearFormat = dataToSave.YearFormat,
					UsePrefix = dataToSave.UsePrefix,
					Reset = dataToSave.Reset,
					ResetConfig = dataToSave.ResetConfig,
					PaddingNo = dataToSave.PaddingNo,
					Comment = dataToSave.Comment,
				};
				context.SYS_CodeConfiguration.Add(dt);
				context.SaveChanges();
				BaseDto.ExtData = dt.Id.ToString();
				BaseDto.Data = true;
				BaseDto.ErrorMessage = "Code Configuration Added Successfully";
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

		public BaseResponseDTO<bool> UpdateAsync(CodeConfigurationCRUDDTO dataToUpdate)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				if (!context.SYS_CodeConfiguration.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
				&& x.Id != dataToUpdate.Id))
				{
					SYS_CodeConfiguration SYS_CodeConfiguration = context.SYS_CodeConfiguration.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

					SYS_CodeConfiguration.Name = dataToUpdate.Name;
					SYS_CodeConfiguration.Date = dataToUpdate.Date;
					SYS_CodeConfiguration.DateFormat = dataToUpdate.DateFormat;
					SYS_CodeConfiguration.Month = dataToUpdate.Month;
					SYS_CodeConfiguration.MonthFormat = dataToUpdate.MonthFormat;
					SYS_CodeConfiguration.Year = dataToUpdate.Year;
					SYS_CodeConfiguration.YearFormat = dataToUpdate.YearFormat;
					SYS_CodeConfiguration.UsePrefix = dataToUpdate.UsePrefix;
					SYS_CodeConfiguration.Reset = dataToUpdate.Reset;
					SYS_CodeConfiguration.ResetConfig = dataToUpdate.ResetConfig;
					SYS_CodeConfiguration.PaddingNo = dataToUpdate.PaddingNo;
					SYS_CodeConfiguration.Comment = dataToUpdate.Comment;
					context.SYS_CodeConfiguration.Update(SYS_CodeConfiguration);
					context.SaveChanges();
					BaseDto.Data = true;
					BaseDto.ErrorMessage = "Code Configuration update Successfully";
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.ErrorMessage = "Code Configuration Already Exist";
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
