using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Context;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class MenuService
    {
		private PerspectiveContext context;
		private QueryResult queryResult;
		public MenuService()
		{
			context = new PerspectiveContext();
			queryResult = new QueryResult();
		}

		public BaseResponseDTO<List<SideMenuDTO>> GetAll()
		{
			BaseResponseDTO<List<SideMenuDTO>> dto = new BaseResponseDTO<List<SideMenuDTO>>();
			List<SideMenuDTO> result = new List<SideMenuDTO>();
			string errorMsg = "No Data Found";
			try
			{
				result = (from a in context.SYS_Modules
						  where a.DeleteStatus == false
						  select new SideMenuDTO()
						  {
							  Id = a.Id,
							  DisplayName = a.DisplayName,
							  Order = a.Order,
							  Icon = a.Icon,

						  }).ToList();
				dto.Data = result;
				dto.QryResult = queryResult.SUCEEDED;
			}
			catch (Exception ex)
			{
				dto.Data = result;
				dto.ErrorMessage = errorMsg;
				dto.QryResult = queryResult.FAILED;
			}

			return dto;

		}

		public BaseResponseDTO<SideMenuDTO> GetById(string Id)
		{
			BaseResponseDTO<SideMenuDTO> dto = new BaseResponseDTO<SideMenuDTO>();
			SideMenuDTO result = new SideMenuDTO();
			string errorMsg = "No Data Found";
			try
			{
				Guid Mid = Guid.Parse(Id);
				result = (from a in context.SYS_Modules
						  where a.Id == Mid
						  select new SideMenuDTO()
						  {
							  Id = a.Id,
							  DisplayName = a.DisplayName,
							  Order = a.Order,
							  Icon = a.Icon,
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
			catch (Exception e)
			{
				dto.Data = result;
				dto.ErrorMessage = errorMsg;
				dto.QryResult = queryResult.FAILED;
			}


			return dto;
		}

		public BaseResponseDTO<bool> UpdateAsync(SideMenuDTO dataToUpdate)
		{
			BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
			try
			{
				SYS_Modules Mod = context.SYS_Modules.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();
				Mod.DisplayName = dataToUpdate.DisplayName;
				Mod.Order = dataToUpdate.Order;
				Mod.Icon = dataToUpdate.Icon;
				context.SYS_Modules.Update(Mod);
				context.SaveChanges();
				BaseDto.Data = true;
				BaseDto.QryResult = queryResult.SUCEEDED;

			}
			catch (Exception ex)
			{
				BaseDto.Data = false;
				BaseDto.ErrorMessage = "Error Updating Record";
				BaseDto.QryResult = queryResult.FAILED;
			}
			return BaseDto;
		}
	}
}
