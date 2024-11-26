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
    public class CarouselService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public CarouselService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<CarouselDTO>> GetAll()
        {
            BaseResponseDTO<List<CarouselDTO>> dto = new BaseResponseDTO<List<CarouselDTO>>();
            CarouselDTO user = new CarouselDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<CarouselDTO> result = (from a in context.SYS_Carousel
                                          where a.DeleteStatus == false
                                          select new CarouselDTO
                                          {
                                              Id = a.Id,
                                              Name = a.Name,
                                              Duration = a.Duration
                                          }).ToList();
                //List<ApplicationUser> result = context.Users.Where(x => !exEmail.Contains(x.Email) && x.DeleteStatus == false).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<CarouselDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<CarouselDTO> GetById(long Id)
        {
            BaseResponseDTO<CarouselDTO> dto = new BaseResponseDTO<CarouselDTO>();
            CarouselDTO result = new CarouselDTO();
            try
            {
                result = (from a in context.SYS_Carousel
                          where a.DeleteStatus == false && a.Id == Id
                          select new CarouselDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Duration = a.Duration
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
                dto.Data = new CarouselDTO();
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
                Dd.title = "Poster";
                Dd.items = new List<DropDownItem>();
                List<SYS_Poster> Poster = context.SYS_Poster.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Poster lt in Poster)
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(CarouselDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Carousel.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Carousel DSS = new SYS_Carousel()
                    {
                        Name = dataToSave.Name,
                        Duration = dataToSave.Duration
                    };
                    context.SYS_Carousel.Add(DSS);
                    context.SaveChanges();

                    SaveAndUpdateChildTable(dataToSave, DSS.Id);

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Carousel save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Carousel Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(CarouselDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Carousel.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Carousel DSS = context.SYS_Carousel.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Duration = dataToUpdate.Duration;

                    context.SYS_Carousel.Update(DSS);
                    context.SaveChanges();

                    SaveAndUpdateChildTable(dataToUpdate, DSS.Id);

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Carousel update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Carousel Already Exist";
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

        public string SaveAndUpdateChildTable(CarouselDTO data, long Id)
        {
            string result = "";
            try
            {
                // NPF_REG_FarmOccupyLandSpeciesMapping
                List<SYS_CarouselPosterMapping> CarouselPosterMappingData = context.SYS_CarouselPosterMapping.Where(x => x.CarouselId == Id).ToList();

                if (data.PosterData != null && data.PosterData.Count > 0)
                {
                    foreach (var item in data.PosterData)
                    {
                        if (CarouselPosterMappingData.Any(y => y.Id == item.Id))
                        {
                            CarouselPosterMappingData.RemoveAll(x => x.Id == item.Id);
                        }
                        else
                        {
                            SYS_CarouselPosterMapping dt1 = new SYS_CarouselPosterMapping()
                            {
                                Poster = context.SYS_Poster.Where(x => x.Id == item.PosterId).FirstOrDefault(),
                                CarouselId = Id,
                            };
                            context.SYS_CarouselPosterMapping.Add(dt1);
                            context.SaveChanges();
                        }

                    }
                }

                if (CarouselPosterMappingData.Count > 0)
                {
                    foreach (var item in CarouselPosterMappingData)
                    {
                        item.DeleteStatus = true;
                        context.SYS_CarouselPosterMapping.Update(item);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                result = "Error: " + ex.Message;
            }
            return result;
        }

        public BaseResponseDTO<List<CarouselPosterMappingDTO>> CarouselChildDataByParentId(long? id)
        {
            BaseResponseDTO<List<CarouselPosterMappingDTO>> dto = new BaseResponseDTO<List<CarouselPosterMappingDTO>>();
            List<CarouselPosterMappingDTO> result = new List<CarouselPosterMappingDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.SYS_CarouselPosterMapping
                          where a.CarouselId == id && a.DeleteStatus == false
                          select new CarouselPosterMappingDTO()
                          {
                              Id = a.Id,
                              CarouselId = a.CarouselId,
                              PosterId = a.Poster.Id,
                              PosterName = a.Poster.Name,
                              PosterImageBase64 = a.Poster.PosterImageBase64,

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

        public BaseResponseDTO<List<PreviewCarouselPosterMappingDTO>> PreviewCarouselByParentId(long? id)
        {
            BaseResponseDTO<List<PreviewCarouselPosterMappingDTO>> dto = new BaseResponseDTO<List<PreviewCarouselPosterMappingDTO>>();
            List<PreviewCarouselPosterMappingDTO> result = new List<PreviewCarouselPosterMappingDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.SYS_CarouselPosterMapping
                          where a.CarouselId == id && a.DeleteStatus == false
                          select new PreviewCarouselPosterMappingDTO()
                          {
                              Id = a.Id,
                              CarouselId = a.CarouselId,
                              PosterId = a.Poster.Id,
                              PosterName = a.Poster.Name,
                              PosterImageBase64 = a.Poster.PosterImageBase64,
                              Duration = context.SYS_Carousel.Where(x => x.Id == id).FirstOrDefault().Duration
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
