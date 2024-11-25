using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class PosterService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public PosterService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<PosterDTO>> GetAll()
        {
            BaseResponseDTO<List<PosterDTO>> dto = new BaseResponseDTO<List<PosterDTO>>();
            PosterDTO user = new PosterDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<PosterDTO> result = (from a in context.SYS_Poster
                                          where a.DeleteStatus == false
                                          select new PosterDTO
                                          {
                                              Id = a.Id,
                                              Name = a.Name,
                                              PosterImageBase64 = a.PosterImageBase64,
                                              Type = (long)a.Type,
                                              TypeName = a.Type.ToString(),
                                              Status = a.Status
                                          }).ToList();
                //List<ApplicationUser> result = context.Users.Where(x => !exEmail.Contains(x.Email) && x.DeleteStatus == false).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<PosterDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<PosterDTO> GetById(long Id)
        {
            BaseResponseDTO<PosterDTO> dto = new BaseResponseDTO<PosterDTO>();
            PosterDTO result = new PosterDTO();
            try
            {
                result = (from a in context.SYS_Poster
                          where a.DeleteStatus == false && a.Id == Id
                          select new PosterDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              PosterImageBase64 = a.PosterImageBase64,
                              Type = (long)a.Type,
                              TypeName = a.Type.ToString(),
                              Status = a.Status
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
                dto.Data = new PosterDTO();
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
                //Poster Type
                Dd = new DropDown();
                Dd.title = "Type";
                Dd.items = new List<DropDownItem>();

                var enumList = Enum.GetValues(typeof(PosterType))
                    .Cast<PosterType>()
                    .Select(e => new DropDownItem { Id = (int)e, text = e.ToString() })
                    .ToList();

                Dd.items.AddRange(enumList);

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
        public async Task<BaseResponseDTO<bool>> SaveAsync(PosterDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Poster.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Poster DSS = new SYS_Poster()
                    {
                        Name = dataToSave.Name,
                        PosterImageBase64 = dataToSave.PosterImageBase64,
                        Type = (PosterType)dataToSave.Type,
                        Status = dataToSave.Status
                    };
                    context.SYS_Poster.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Poster save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Poster Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(PosterDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Poster.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Poster DSS = context.SYS_Poster.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.PosterImageBase64 = dataToUpdate.PosterImageBase64;
                    DSS.Type = (PosterType)dataToUpdate.Type;
                    DSS.Status = dataToUpdate.Status;

                    context.SYS_Poster.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Poster update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Poster Already Exist";
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
