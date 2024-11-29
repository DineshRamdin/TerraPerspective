using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Index.HPRtree;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class SystemIconService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public SystemIconService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<SystemIconDTO>> GetAll()
        {
            BaseResponseDTO<List<SystemIconDTO>> dto = new BaseResponseDTO<List<SystemIconDTO>>();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<SystemIconDTO> result = (from a in context.SYS_SystemIcon
                                              where a.DeleteStatus == false
                                              select new SystemIconDTO
                                              {
                                                  Id = a.Id.ToString(),
                                                  Name = a.Name,
                                                  SystemIconImagebase64 = a.SystemIconImagebase64,
                                                  IsImage = a.SystemIconImagebase64 == null ? false : true,

                                              }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<SystemIconDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<SystemIconDTO> GetById(string Id)
        {
            BaseResponseDTO<SystemIconDTO> dto = new BaseResponseDTO<SystemIconDTO>();
            SystemIconDTO result = new SystemIconDTO();

            try
            {
                result = (from a in context.SYS_SystemIcon
                          where a.DeleteStatus == false && a.Id.ToString() == Id
                          select new SystemIconDTO()
                          {
                              Id = a.Id.ToString(),
                              Name = a.Name,
                              SystemIconImagebase64 = a.SystemIconImagebase64,
                              IsImage = a.SystemIconImagebase64 == null ? false : true,
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
                dto.Data = new SystemIconDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveSystemIcon(SystemIconDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                if (!context.SYS_SystemIcon.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_SystemIcon DSS = new SYS_SystemIcon()
                    {
                        Name = dataToSave.Name,
                        SystemIconImagebase64 = dataToSave.SystemIconImagebase64

                    };
                    context.SYS_SystemIcon.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "System Icon save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "System Icon Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateSystemIcon(SystemIconDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_SystemIcon.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id.ToString() != dataToUpdate.Id))
                {
                    SYS_SystemIcon DSS = context.SYS_SystemIcon.Where(x => x.Id.ToString() == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.SystemIconImagebase64 = dataToUpdate.SystemIconImagebase64;

                    context.SYS_SystemIcon.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "System Icon update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "System Icon Already Exist";
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

        public async Task<BaseResponseDTO<bool>> SystemIconDelete(long Id)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                if (context.SYS_SystemIcon.Any(x => x.Id == Id))
                {
                    SYS_SystemIcon DSS = context.SYS_SystemIcon.Where(x => x.Id == Id).FirstOrDefault();

                    DSS.DeleteStatus = true;
                    context.SYS_SystemIcon.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "System Icon Delete Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Fail";
                    BaseDto.QryResult = queryResult.FAILED;
                }

            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.ErrorMessage = "Fail";
                BaseDto.QryResult = new QueryResult().FAILED;
            }

            return BaseDto;
        }

        public BaseResponseDTO<List<SystemIconDTO>> GetPreviewById(long? id)
        {
            BaseResponseDTO<List<SystemIconDTO>> dto = new BaseResponseDTO<List<SystemIconDTO>>();
            List<SystemIconDTO> result = new List<SystemIconDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.SYS_SystemIcon
                          where a.Id == id && a.DeleteStatus == false
                          select new SystemIconDTO()
                          {
                              Id = a.Id.ToString(),
                              Name = a.Name,
                              SystemIconImagebase64 = a.SystemIconImagebase64,
                              IsImage = a.SystemIconImagebase64 == null ? false : true,

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
    }
}
