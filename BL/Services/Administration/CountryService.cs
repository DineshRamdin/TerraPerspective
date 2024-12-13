using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models.Administration;
using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class CountryService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public CountryService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<CountryDTO>> GetAll()
        {
            BaseResponseDTO<List<CountryDTO>> dto = new BaseResponseDTO<List<CountryDTO>>();
            CountryDTO user = new CountryDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<CountryDTO> result = (from a in context.SYS_Country
                                           where a.DeleteStatus == false
                                           select new CountryDTO
                                           {
                                               Id = a.Id,
                                               Name = a.Name,
                                               Description = a.Description,
                                               Currency = a.Currency,
                                               TimeZone = a.TimeZone,
                                               Language = a.Language,
                                           }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<CountryDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<CountryDTO> GetById(long Id)
        {
            BaseResponseDTO<CountryDTO> dto = new BaseResponseDTO<CountryDTO>();
            CountryDTO result = new CountryDTO();
            try
            {
                result = (from a in context.SYS_Country
                          where a.DeleteStatus == false && a.Id == Id
                          select new CountryDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Description = a.Description,
                              Currency = a.Currency,
                              TimeZone = a.TimeZone,
                              Language = a.Language,
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
                dto.Data = new CountryDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(CountryDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Country.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Country DSS = new SYS_Country()
                    {
                        Name = dataToSave.Name,
                        Description = string.IsNullOrEmpty(dataToSave.Description) ? string.Empty : dataToSave.Description,
                        Currency = dataToSave.Currency,
                        TimeZone = dataToSave.TimeZone,
                        Language = dataToSave.Language,
                    };
                    context.SYS_Country.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Country save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Country Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(CountryDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Country.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Country DSS = context.SYS_Country.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Description = string.IsNullOrEmpty(dataToUpdate.Description) ? string.Empty : dataToUpdate.Description;
                    DSS.Currency = string.IsNullOrEmpty(dataToUpdate.Currency) ? string.Empty : dataToUpdate.Currency;
                    DSS.TimeZone = dataToUpdate.TimeZone;
                    DSS.Language = string.IsNullOrEmpty(dataToUpdate.Language) ? string.Empty : dataToUpdate.Language;

                    context.SYS_Country.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Country update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Country Already Exist";
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
