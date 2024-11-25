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
    public class LookUpTypeService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public LookUpTypeService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<LookUpTypeDTO>> GetAll()
        {
            BaseResponseDTO<List<LookUpTypeDTO>> dto = new BaseResponseDTO<List<LookUpTypeDTO>>();
            LookUpTypeDTO user = new LookUpTypeDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<LookUpTypeDTO> result = (from a in context.SYS_LookUpType
                                              where a.DeleteStatus == false
                                              select new LookUpTypeDTO
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
                dto.Data = new List<LookUpTypeDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<LookUpTypeDTO> GetById(long Id)
        {
            BaseResponseDTO<LookUpTypeDTO> dto = new BaseResponseDTO<LookUpTypeDTO>();
            LookUpTypeDTO result = new LookUpTypeDTO();
            try
            {
                result = (from a in context.SYS_LookUpType
                          where a.DeleteStatus == false && a.Id == Id
                          select new LookUpTypeDTO()
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
                dto.Data = new LookUpTypeDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(LookUpTypeDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_LookUpType.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_LookUpType DSS = new SYS_LookUpType()
                    {
                        Name = dataToSave.Name,
                        Description = string.IsNullOrEmpty(dataToSave.Description) ? string.Empty : dataToSave.Description,
                    };
                    context.SYS_LookUpType.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "LookUp Type save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "LookUp Type Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(LookUpTypeDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_LookUpType.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_LookUpType DSS = context.SYS_LookUpType.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Description = string.IsNullOrEmpty(dataToUpdate.Description) ? string.Empty : dataToUpdate.Description;

                    context.SYS_LookUpType.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "LookUp Type update Successfully";
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
