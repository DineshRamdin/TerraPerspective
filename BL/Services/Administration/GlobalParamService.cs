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
    public class GlobalParamService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public GlobalParamService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<GlobalParamDTO>> GetAll()
        {
            BaseResponseDTO<List<GlobalParamDTO>> dto = new BaseResponseDTO<List<GlobalParamDTO>>();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<GlobalParamDTO> result = (from a in context.SYS_GlobalParam
                                               where a.DeleteStatus == false
                                               select new GlobalParamDTO
                                               {
                                                   Id = a.Id.ToString(),
                                                   Name = a.Name,
                                                   Value = a.Value.ToString(),
                                                   AdditionalValue = a.AdditionalValue.ToString(),
                                                   Comment = a.Comment,
                                                   AdminOnly = a.AdminOnly,
                                                   Enable = a.Enable
                                               }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<GlobalParamDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<GlobalParamDTO> GetById(string Id)
        {
            BaseResponseDTO<GlobalParamDTO> dto = new BaseResponseDTO<GlobalParamDTO>();
            GlobalParamDTO result = new GlobalParamDTO();

            try
            {
                result = (from a in context.SYS_GlobalParam
                          where a.DeleteStatus == false && a.Id.ToString() == Id
                          select new GlobalParamDTO()
                          {
                              Id = a.Id.ToString(),
                              Name = a.Name,
                              Value = a.Value,
                              AdditionalValue = a.AdditionalValue.ToString(),
                              Comment = a.Comment,
                              AdminOnly = a.AdminOnly,
                              Enable = a.Enable
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
                dto.Data = new GlobalParamDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveGlobalParam(GlobalParamDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                if (!context.SYS_GlobalParam.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    Guid newid = Guid.NewGuid();

                    SYS_GlobalParam DSS = new SYS_GlobalParam()
                    {
                        //Id = Guid.NewGuid().ToString(),
                        Id = newid,
                        Name = dataToSave.Name,
                        Value = dataToSave.Value,
                        AdditionalValue = dataToSave.AdditionalValue == null ? " " : dataToSave.AdditionalValue.ToString(),
                        Comment = dataToSave.Comment == null ? " " : dataToSave.Comment.ToString(),
                        AdminOnly = dataToSave.AdminOnly,
                        Enable = dataToSave.Enable
                    };
                    context.SYS_GlobalParam.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Global Param save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Global Param Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateGlobalParam(GlobalParamDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_GlobalParam.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id.ToString() != dataToUpdate.Id))
                {
                    SYS_GlobalParam DSS = context.SYS_GlobalParam.Where(x => x.Id.ToString() == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Value = dataToUpdate.Value;
                    DSS.AdditionalValue = dataToUpdate.AdditionalValue == null ? " " : dataToUpdate.AdditionalValue.ToString();
                    DSS.Comment = dataToUpdate.Comment == null ? " " : dataToUpdate.Comment.ToString();
                    DSS.AdminOnly = dataToUpdate.AdminOnly;
                    DSS.Enable = dataToUpdate.Enable;

                    context.SYS_GlobalParam.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Global Param update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Global Param Already Exist";
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
