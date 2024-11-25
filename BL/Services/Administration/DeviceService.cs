using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models.Administration;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class DeviceService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public DeviceService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<DeviceDTO>> GetAll()
        {
            BaseResponseDTO<List<DeviceDTO>> dto = new BaseResponseDTO<List<DeviceDTO>>();
            DeviceDTO user = new DeviceDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<DeviceDTO> result = (from a in context.SYS_Device
                                        where a.DeleteStatus == false
                                        select new DeviceDTO
                                        {
                                            Id = a.Id,
                                            Name = a.Name,
                                            MACAddress = a.MACAddress,
                                            Type = a.Type.ToString(),
                                            DefaultCarousel=a.DefaultCarousel.Name,
                                        }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<DeviceDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<DeviceCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<DeviceCRUDDTO> dto = new BaseResponseDTO<DeviceCRUDDTO>();
            DeviceCRUDDTO result = new DeviceCRUDDTO();
            try
            {
                result = (from a in context.SYS_Device
                          where a.DeleteStatus == false && a.Id == Id
                          select new DeviceCRUDDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              MACAddress = a.MACAddress,
                              Type = (int)a.Type,
                              DefaultCarousel = a.DefaultCarousel.Id,
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
                dto.Data = new DeviceCRUDDTO();
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

                var enumList = Enum.GetValues(typeof(DeviceType))
                    .Cast<DeviceType>()
                    .Select(e => new DropDownItem { Id = (int)e, text = e.ToString() })
                    .ToList();

                Dd.items.AddRange(enumList);

                Ddl.Add(Dd);

                //Animal Microchip No
                Dd = new DropDown();
                Dd.title = "Carousel";
                Dd.items = new List<DropDownItem>();
                List<SYS_Carousel> Carousel = context.SYS_Carousel.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Carousel lt in Carousel)
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

        public async Task<BaseResponseDTO<bool>> SaveAsync(DeviceCRUDDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Device.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.MACAddress.ToLower() == dataToSave.MACAddress.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Device DSS = new SYS_Device()
                    {
                        Name = dataToSave.Name,
                        MACAddress = dataToSave.MACAddress,
                        Type = (DeviceType)dataToSave.Type,
                        DefaultCarousel = context.SYS_Carousel.Where(x => x.Id == dataToSave.DefaultCarousel).FirstOrDefault()
                    };
                    context.SYS_Device.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Device save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Device Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(DeviceCRUDDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Device.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.MACAddress.ToLower() == dataToUpdate.MACAddress.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Device DSS = context.SYS_Device.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.MACAddress = dataToUpdate.MACAddress;
                    DSS.Type = (DeviceType)dataToUpdate.Type;
                    DSS.DefaultCarousel = context.SYS_Carousel.Where(x => x.Id == dataToUpdate.DefaultCarousel).FirstOrDefault();

                    context.SYS_Device.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Device update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Device Already Exist";
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

        public BaseResponseDTO<DeviceCRUDDTO> GetByMACAddress(string MACAddress)
        {
            BaseResponseDTO<DeviceCRUDDTO> dto = new BaseResponseDTO<DeviceCRUDDTO>();
            DeviceCRUDDTO result = new DeviceCRUDDTO();
            try
            {
                result = (from a in context.SYS_Device
                          where a.DeleteStatus == false && a.MACAddress == MACAddress
                          select new DeviceCRUDDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              MACAddress = a.MACAddress,
                              Type = (int)a.Type,
                              DefaultCarousel = a.DefaultCarousel.Id,
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
                dto.Data = new DeviceCRUDDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
    }
}
