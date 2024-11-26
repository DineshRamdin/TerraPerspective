using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class TestingService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public TestingService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<TestingDTO>> GetAll()
        {
            BaseResponseDTO<List<TestingDTO>> dto = new BaseResponseDTO<List<TestingDTO>>();
            TestingDTO user = new TestingDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<TestingDTO> result = (from a in context.SYS_Testing
                                           where a.DeleteStatus == false
                                           select new TestingDTO
                                           {
                                               Id = a.Id,
                                               TestingName = a.TestingName,
                                               TestingType = a.TestingType.ToString(),
                                               TestingDate = a.TestingDate.ToString(),
                                               TestingCode = a.TestingCode,
                                               Description = a.Description
                                           }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<TestingDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;


        }

        public BaseResponseDTO<TestingCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<TestingCRUDDTO> dto = new BaseResponseDTO<TestingCRUDDTO>();
            TestingCRUDDTO result = new TestingCRUDDTO();
            try
            {
                result = (from a in context.SYS_Testing
                          where a.DeleteStatus == false && a.Id == Id
                          select new TestingCRUDDTO()
                          {
                              Id = a.Id,
                              TestingName = a.TestingName,
                              TestingType = (int)a.TestingType,
                              TestingDate = a.TestingDate,
                              TestingCode = a.TestingCode,
                              Description = a.Description
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
                dto.Data = new TestingCRUDDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(TestingCRUDDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Testing.Any(x => x.TestingName.ToLower() == dataToSave.TestingName.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Testing DSS = new SYS_Testing()
                    {
                        TestingName = dataToSave.TestingName,
                        TestingCode = dataToSave.TestingCode,
                        TestingType = (TestingType)dataToSave.TestingType,
                        TestingDate = dataToSave.TestingDate,
                        Description = dataToSave.Description,
                    };
                    context.SYS_Testing.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Testing save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Testing Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(TestingCRUDDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Testing.Any(x => x.TestingName.ToLower() == dataToUpdate.TestingName.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Testing DSS = context.SYS_Testing.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.TestingName = dataToUpdate.TestingName;
                    DSS.TestingCode = dataToUpdate.TestingCode;
                    DSS.TestingType = (TestingType)dataToUpdate.TestingType;
                    DSS.TestingDate = dataToUpdate.TestingDate;
                    DSS.Description = dataToUpdate.Description;

                    context.SYS_Testing.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Testing update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Testing Already Exist";
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

        public async Task<BaseResponseDTO<bool>> TestingDelete(long Id, UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();

            try
            {
                if (context.SYS_Testing.Any(x => x.Id == Id))
                {
                    SYS_Testing DSS = context.SYS_Testing.Where(x => x.Id == Id).FirstOrDefault();

                    DSS.DeleteStatus = true;
                    context.SYS_Testing.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Testing Delete Successfully";
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

    }
}
