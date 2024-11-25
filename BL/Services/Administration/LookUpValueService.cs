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
    public class LookUpValueService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public LookUpValueService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<LookUpValueDTO>> GetAll()
        {
            BaseResponseDTO<List<LookUpValueDTO>> dto = new BaseResponseDTO<List<LookUpValueDTO>>();
            LookUpValueDTO user = new LookUpValueDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<LookUpValueDTO> result = (from a in context.SYS_LookUpValue
                                               where a.DeleteStatus == false
                                               select new LookUpValueDTO
                                               {
                                                   Id = a.Id,
                                                   Name = a.Name,
                                                   Description = a.Description,
                                                   LookUpType = a.LookUpType.Name
                                               }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<LookUpValueDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<LookUpValueCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<LookUpValueCRUDDTO> dto = new BaseResponseDTO<LookUpValueCRUDDTO>();
            LookUpValueCRUDDTO result = new LookUpValueCRUDDTO();
            try
            {
                result = (from a in context.SYS_LookUpValue
                          where a.DeleteStatus == false && a.Id == Id
                          select new LookUpValueCRUDDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Description = a.Description,
                              LookUpType = a.LookUpType.Id
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
                dto.Data = new LookUpValueCRUDDTO();
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
             
                Dd = new DropDown();
                Dd.title = "LookUp Type";
                Dd.items = new List<DropDownItem>();
                List<SYS_LookUpType> LookUpType = context.SYS_LookUpType.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_LookUpType lt in LookUpType)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(LookUpValueCRUDDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_LookUpValue.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.LookUpType.Id == dataToSave.LookUpType && x.DeleteStatus != true))
                {
                    SYS_LookUpValue DSS = new SYS_LookUpValue()
                    {
                        Name = dataToSave.Name,
                        Description = string.IsNullOrEmpty(dataToSave.Description) ? string.Empty : dataToSave.Description,
                        LookUpType = context.SYS_LookUpType.Where(x => x.Id == dataToSave.LookUpType).FirstOrDefault()
                    };
                    context.SYS_LookUpValue.Add(DSS);
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(LookUpValueCRUDDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_LookUpValue.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_LookUpValue DSS = context.SYS_LookUpValue.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Description = string.IsNullOrEmpty(dataToUpdate.Description) ? string.Empty : dataToUpdate.Description;
                    DSS.LookUpType = context.SYS_LookUpType.Where(x => x.Id == dataToUpdate.LookUpType).FirstOrDefault();

                    context.SYS_LookUpValue.Update(DSS);
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
