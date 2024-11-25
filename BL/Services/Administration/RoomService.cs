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
    public class RoomService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public RoomService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }


        public BaseResponseDTO<List<RoomDTO>> GetAll()
        {
            BaseResponseDTO<List<RoomDTO>> dto = new BaseResponseDTO<List<RoomDTO>>();
            RoomDTO user = new RoomDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<RoomDTO> result = (from a in context.SYS_Room
                                        where a.DeleteStatus == false
                                        select new RoomDTO
                                        {
                                            Id = a.Id,
                                            Name = a.Name,
                                            Type = a.Type,
                                            Location = a.Location,
                                        }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<RoomDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<RoomDTO> GetById(long Id)
        {
            BaseResponseDTO<RoomDTO> dto = new BaseResponseDTO<RoomDTO>();
            RoomDTO result = new RoomDTO();
            try
            {
                result = (from a in context.SYS_Room
                          where a.DeleteStatus == false && a.Id == Id
                          select new RoomDTO()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              Type = a.Type,
                              Location = a.Location,
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
                dto.Data = new RoomDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(RoomDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Room.Any(x => x.Name.ToLower() == dataToSave.Name.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Room DSS = new SYS_Room()
                    {
                        Name = dataToSave.Name,
                        Type = dataToSave.Type,
                        Location = dataToSave.Location,
                    };
                    context.SYS_Room.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Room save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Room Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(RoomDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Room.Any(x => x.Name.ToLower() == dataToUpdate.Name.ToLower() && x.DeleteStatus != true
                              && x.Id != dataToUpdate.Id))
                {
                    SYS_Room DSS = context.SYS_Room.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.Name = dataToUpdate.Name;
                    DSS.Type = dataToUpdate.Type;
                    DSS.Location = dataToUpdate.Location;

                    context.SYS_Room.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Room update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Room Already Exist";
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
