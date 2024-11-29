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
    public class ReportsService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public ReportsService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ReportsDTO>> GetAll()
        {
            BaseResponseDTO<List<ReportsDTO>> dto = new BaseResponseDTO<List<ReportsDTO>>();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<ReportsDTO> result = (from a in context.SYS_Reports
                                           where a.DeleteStatus == false
                                           select new ReportsDTO
                                           {
                                               Id = a.Id.ToString(),
                                               Name = a.Name,
                                               Type = a.Type,
                                               ReportImagebase64 = a.ReportImagebase64,
                                               IsImage = a.ReportImagebase64 == null ? false : true,

                                           }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ReportsDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<ReportsDTO> GetById(string Id)
        {
            BaseResponseDTO<ReportsDTO> dto = new BaseResponseDTO<ReportsDTO>();
            ReportsDTO result = new ReportsDTO();

            try
            {
                result = (from a in context.SYS_Reports
                          where a.DeleteStatus == false && a.Id.ToString() == Id
                          select new ReportsDTO()
                          {
                              Id = a.Id.ToString(),
                              Name = a.Name,
                              Type = a.Type,
                              ReportImagebase64 = a.ReportImagebase64,
                              IsImage = a.ReportImagebase64 == null ? false : true,
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
                dto.Data = new ReportsDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveReports(ReportsDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {

                if (!context.SYS_Reports.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Reports DSS = new SYS_Reports()
                    {
                        Name = dataToSave.Name,
                        Type = dataToSave.Type,
                        ReportImagebase64 = dataToSave.ReportImagebase64

                    };
                    context.SYS_Reports.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Report save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Report Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateReports(ReportsDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Reports.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id.ToString() != dataToUpdate.Id))
                {
                    SYS_Reports DSS = context.SYS_Reports.Where(x => x.Id.ToString() == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Type = dataToUpdate.Type;
                    DSS.ReportImagebase64 = dataToUpdate.ReportImagebase64;


                    context.SYS_Reports.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Report update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Report Already Exist";
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

        public async Task<BaseResponseDTO<bool>> ReportsDelete(long Id)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                if (context.SYS_Reports.Any(x => x.Id == Id))
                {
                    SYS_Reports DSS = context.SYS_Reports.Where(x => x.Id == Id).FirstOrDefault();

                    DSS.DeleteStatus = true;
                    context.SYS_Reports.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Report Delete Successfully";
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

        public BaseResponseDTO<List<ReportsDTO>> GetPreviewById(long? id)
        {
            BaseResponseDTO<List<ReportsDTO>> dto = new BaseResponseDTO<List<ReportsDTO>>();
            List<ReportsDTO> result = new List<ReportsDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.SYS_Reports
                          where a.Id == id && a.DeleteStatus == false
                          select new ReportsDTO()
                          {
                              Id = a.Id.ToString(),
                              Type = a.Type,
                              Name = a.Name,
                              ReportImagebase64 = a.ReportImagebase64,
                              IsImage = a.ReportImagebase64 == null ? false : true,

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
