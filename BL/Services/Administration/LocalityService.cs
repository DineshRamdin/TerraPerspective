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
    public class LocalityService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public LocalityService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<LocalityDTO>> GetAll()
        {
            BaseResponseDTO<List<LocalityDTO>> dto = new BaseResponseDTO<List<LocalityDTO>>();
            LocalityDTO user = new LocalityDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<LocalityDTO> result = (from a in context.SYS_Locality
                                            where a.DeleteStatus == false
                                            select new LocalityDTO
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
                dto.Data = new List<LocalityDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<LocalityDTO> GetById(long Id)
        {
            BaseResponseDTO<LocalityDTO> dto = new BaseResponseDTO<LocalityDTO>();
            LocalityDTO result = new LocalityDTO();
            try
            {
                result = (from a in context.SYS_Locality
                          where a.DeleteStatus == false && a.Id == Id
                          select new LocalityDTO()
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
                dto.Data = new LocalityDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(LocalityDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Locality.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Locality DSS = new SYS_Locality()
                    {
                        Name = dataToSave.Name,
                        Description = string.IsNullOrEmpty(dataToSave.Description) ? string.Empty : dataToSave.Description,
                    };
                    context.SYS_Locality.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Locality save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Locality Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(LocalityDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Locality.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Locality DSS = context.SYS_Locality.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Description = string.IsNullOrEmpty(dataToUpdate.Description) ? string.Empty : dataToUpdate.Description;

                    context.SYS_Locality.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Locality update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Locality Already Exist";
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
