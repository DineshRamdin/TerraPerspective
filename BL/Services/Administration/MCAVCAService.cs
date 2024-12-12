using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
	public class MCAVCAService
	{
		private PerspectiveContext context;
		private QueryResult queryResult;

		public MCAVCAService()
		{
			context = new PerspectiveContext();
			queryResult = new QueryResult();
		}

		public BaseResponseDTO<List<MCAVCADTO>> GetAll()
		{
			BaseResponseDTO<List<MCAVCADTO>> dto = new BaseResponseDTO<List<MCAVCADTO>>();
			MCAVCADTO user = new MCAVCADTO();
			QueryResult queryResult = new QueryResult();
			string errorMsg = "No Data Found";

			try
			{

				string sQryResult = queryResult.FAILED;
				List<MCAVCADTO> result = (from a in context.SYS_MCAVCA
										  where a.DeleteStatus == false
										  select new MCAVCADTO
										  {
											  Id = a.Id,
											  Name = a.Name,
											  Description = a.Description,
										  }).ToList();

				dto.Data = result;
				dto.QryResult = queryResult.SUCEEDED;

			}
			catch (Exception ex)
			{
				dto.Data = new List<MCAVCADTO>();
				dto.ErrorMessage = errorMsg;
				dto.QryResult = queryResult.SUCEEDED;
			}

			return dto;
		}

		public BaseResponseDTO<MCAVCADTO> GetById(long Id)
		{
			BaseResponseDTO<MCAVCADTO> dto = new BaseResponseDTO<MCAVCADTO>();
			MCAVCADTO result = new MCAVCADTO();
			try
			{
				result = (from a in context.SYS_MCAVCA
						  where a.DeleteStatus == false && a.Id == Id
						  select new MCAVCADTO()
						  {
							  Id = a.Id,
							  Name = a.Name,
							  Description = a.Description,
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
				dto.Data = new MCAVCADTO();
				dto.QryResult = new QueryResult().FAILED;
			}
			return dto;
		}

		public async Task<BaseResponseDTO<bool>> SaveAsync(MCAVCADTO dataToSave)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
			try
			{
				if (!context.SYS_MCAVCA.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
				{
					SYS_MCAVCA DSS = new SYS_MCAVCA()
					{
						Name = dataToSave.Name,
						Description = string.IsNullOrEmpty(dataToSave.Description) ? string.Empty : dataToSave.Description,
					};
					context.SYS_MCAVCA.Add(DSS);
					context.SaveChanges();

					BaseDto.Data = true;
					BaseDto.ErrorMessage = "MCA/VCA save Successfully";
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.ErrorMessage = "MCA/VCA Already Exist";
					BaseDto.QryResult = queryResult.FAILED;
				}
			}
			catch (Exception ex)
			{
				BaseDto.Data = false;
				BaseDto.ErrorMessage = "Error Adding Record";
				BaseDto.QryResult = queryResult.FAILED;
			}
			return BaseDto;
		}

		public async Task<BaseResponseDTO<bool>> UpdateAsync(MCAVCADTO dataToUpdate)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				if (!context.SYS_MCAVCA.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
				&& x.Id != dataToUpdate.Id))
				{
					SYS_MCAVCA DSS = context.SYS_MCAVCA.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

					DSS.Name = dataToUpdate.Name;
					DSS.Description = string.IsNullOrEmpty(dataToUpdate.Description) ? string.Empty : dataToUpdate.Description;

					context.SYS_MCAVCA.Update(DSS);
					context.SaveChanges();
					BaseDto.Data = true;
					BaseDto.ErrorMessage = "MCA/VCA update Successfully";
					BaseDto.QryResult = queryResult.SUCEEDED;
				}
				else
				{
					BaseDto.Data = false;
					BaseDto.ErrorMessage = "MCA/VCA Already Exist";
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
