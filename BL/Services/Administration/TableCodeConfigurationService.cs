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
	public class TableCodeConfigurationService
	{
		private PerspectiveContext context;
		private QueryResult queryResult;

		public TableCodeConfigurationService()
		{
			context = new PerspectiveContext();
			queryResult = new QueryResult();
		}

		public BaseResponseDTO<List<TableCodeConfigurationDTO>> GetAll()
		{
			BaseResponseDTO<List<TableCodeConfigurationDTO>> dto = new BaseResponseDTO<List<TableCodeConfigurationDTO>>();
			TableCodeConfigurationDTO user = new TableCodeConfigurationDTO();
			QueryResult queryResult = new QueryResult();
			string errorMsg = "No Data Found";

			try
			{

				string sQryResult = queryResult.FAILED;
				List<TableCodeConfigurationDTO> result = (from a in context.SYS_TableCodeConfigurations
											where a.DeleteStatus == false
											select new TableCodeConfigurationDTO
											{
												Id = a.Id,
												TableName = a.TableName,
												Prefix = a.Prefix,
												CompanyName = context.SYS_Company.Where(x => x.Id == a.CompanyId).FirstOrDefault().NameofCompany,
												ConfigurationName = context.SYS_CodeConfiguration.Where(x => x.Id == a.ConfigurationId).FirstOrDefault().Name,
												Comment = a.Comment,
												HasAddi = a.HasAddi,
											}).ToList();

				dto.Data = result;
				dto.QryResult = queryResult.SUCEEDED;

			}
			catch (Exception ex)
			{
				dto.Data = new List<TableCodeConfigurationDTO>();
				dto.ErrorMessage = errorMsg;
				dto.QryResult = queryResult.SUCEEDED;
			}

			return dto;
		}

		public BaseResponseDTO<TableCodeConfigurationCRUDDTO> GetById(long Id)
		{
			BaseResponseDTO<TableCodeConfigurationCRUDDTO> dto = new BaseResponseDTO<TableCodeConfigurationCRUDDTO>();
			TableCodeConfigurationCRUDDTO result = new TableCodeConfigurationCRUDDTO();
			try
			{
				result = (from a in context.SYS_TableCodeConfigurations
						  where a.DeleteStatus == false && a.Id == Id
						  select new TableCodeConfigurationCRUDDTO()
						  {
							  Id = a.Id,
							  TableName = a.TableName,
							  Prefix = a.Prefix,
							  CompanyId = a.CompanyId,
							  ConfigurationId = a.ConfigurationId,
							  HasAddi = a.HasAddi,
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
				dto.Data = new TableCodeConfigurationCRUDDTO();
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

				//Configuration
				Dd = new DropDown();
				Dd.title = "Configuration";
				Dd.items = new List<DropDownItem>();
				List<SYS_CodeConfiguration> User = context.SYS_CodeConfiguration.Where(x => x.DeleteStatus == false).ToList();
				foreach (SYS_CodeConfiguration lt in User)
				{
					DropDownItem Ddi = new DropDownItem();
					Ddi.Id = lt.Id;
					Ddi.text = lt.Name;
					Dd.items.Add(Ddi);
				}
				Ddl.Add(Dd);

				//Company
				Dd = new DropDown();
				Dd.title = "Company";
				Dd.items = new List<DropDownItem>();
				List<SYS_Company> companies = context.SYS_Company.Where(x => x.DeleteStatus == false).ToList();
				foreach (SYS_Company lt in companies)
				{
					DropDownItem Ddi = new DropDownItem();
					Ddi.Id = lt.Id;
					Ddi.text = lt.NameofCompany;
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

		public BaseResponseDTO<bool> SaveAsync(TableCodeConfigurationCRUDDTO dataToSave)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				SYS_TableCodeConfigurations dt = new SYS_TableCodeConfigurations()
				{
					TableName = dataToSave.TableName,
					Prefix = dataToSave.Prefix,
					CompanyId = dataToSave.CompanyId,
					ConfigurationId = dataToSave.ConfigurationId,
					HasAddi = dataToSave.HasAddi,
					Comment = dataToSave.Comment,
				};
				context.SYS_TableCodeConfigurations.Add(dt);
				context.SaveChanges();
				BaseDto.ExtData = dt.Id.ToString();
				BaseDto.Data = true;
				BaseDto.ErrorMessage = "Table Code Configurations Added Successfully";
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

		public BaseResponseDTO<bool> UpdateAsync(TableCodeConfigurationCRUDDTO dataToUpdate)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				if (!context.SYS_TableCodeConfigurations.Any(x => x.TableName.ToLower() == dataToUpdate.TableName.ToLower() && x.DeleteStatus != true
				&& x.Id != dataToUpdate.Id))
				{
					SYS_TableCodeConfigurations SYS_TableCodeConfigurations = context.SYS_TableCodeConfigurations.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

					SYS_TableCodeConfigurations.TableName = dataToUpdate.TableName;
					SYS_TableCodeConfigurations.Prefix = dataToUpdate.Prefix;
					SYS_TableCodeConfigurations.CompanyId = dataToUpdate.CompanyId;
					SYS_TableCodeConfigurations.ConfigurationId = dataToUpdate.ConfigurationId;
					SYS_TableCodeConfigurations.HasAddi = dataToUpdate.HasAddi;
					SYS_TableCodeConfigurations.Comment = dataToUpdate.Comment;
					context.SYS_TableCodeConfigurations.Update(SYS_TableCodeConfigurations);
					context.SaveChanges();
					BaseDto.Data = true;
					BaseDto.ErrorMessage = "Table Code Configurations update Successfully";
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.ErrorMessage = "Table Code Configurations Already Exist";
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
