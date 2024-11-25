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
    public class AccessLogService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public AccessLogService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<AccessLogDTO>> GetAll()
        {
            BaseResponseDTO<List<AccessLogDTO>> dto = new BaseResponseDTO<List<AccessLogDTO>>();
            AccessLogDTO user = new AccessLogDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<AccessLogDTO> result = (from a in context.SYS_AccessLog
                                             where a.DeleteStatus == false 
                                             select new AccessLogDTO
                                             {
                                                 Id = a.Id,
                                                 RoomName = a.Room.Name,
                                                 ResourceName = a.Resource.Surname + " " + a.Resource.Othername,
                                                 TimeIn = a.TimeIn.ToString("HH:mm"),
                                                 TimeOut = a.TimeOut.HasValue ? a.TimeOut.Value.ToString("HH:mm") : string.Empty,
                                                 Purpose = a.Purpose.Name,
                                                 Remarks = a.Remarks,
                                                 Visitor = a.Visitor,
                                             }).ToList();
                
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<AccessLogDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<List<AccessLogDTO>> GetAllActiveSession()
        {
            BaseResponseDTO<List<AccessLogDTO>> dto = new BaseResponseDTO<List<AccessLogDTO>>();
            AccessLogDTO user = new AccessLogDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<AccessLogDTO> result = (from a in context.SYS_AccessLog
                                             where a.DeleteStatus == false && a.TimeOut == null
                                             select new AccessLogDTO
                                             {
                                                 Id = a.Id,
                                                 RoomName = a.Room.Name,
                                                 ResourceName = a.Resource.Surname + " " + a.Resource.Othername,
                                                 TimeIn = a.TimeIn.ToString("HH:mm"),
                                                 Purpose = a.Purpose.Name,
                                                 Remarks = a.Remarks,
                                                 Visitor = a.Visitor,
                                             }).ToList();
                //List<ApplicationUser> result = context.Users.Where(x => !exEmail.Contains(x.Email) && x.DeleteStatus == false).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<AccessLogDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<AccessLogDTO> GetById(long Id)
        {
            BaseResponseDTO<AccessLogDTO> dto = new BaseResponseDTO<AccessLogDTO>();
            AccessLogDTO result = new AccessLogDTO();
            try
            {
                result = (from a in context.SYS_AccessLog
                          where a.DeleteStatus == false && a.Id == Id
                          select new AccessLogDTO()
                          {
                              Id = a.Id,
                              RoomName = a.Room.Name,
                              ResourceName = a.Resource.Surname + " " + a.Resource.Othername,
                              TimeIn = a.TimeIn.ToString("HH:mm"),
                              TimeOut = a.TimeOut.HasValue ? a.TimeOut.Value.ToString("HH:mm") : string.Empty,
                              Purpose = a.Purpose.Name,
                              Remarks = a.Remarks,
                              Visitor = a.Visitor,
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
                dto.Data = new AccessLogDTO();
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
                //Lookup Type Value
                string[] values = new string[] { "InterventionReason" };
                List<SYS_LookUpType> Ltl = context.SYS_LookUpType.Where(x => values.Contains(x.Name) && x.DeleteStatus == false).ToList();
                foreach (SYS_LookUpType lt in Ltl)
                {
                    Dd = new DropDown();
                    Dd.title = lt.Name;
                    List<SYS_LookUpValue> Lvl = context.SYS_LookUpValue.Where(x => x.LookUpType.Id == lt.Id && x.DeleteStatus == false).ToList();
                    Dd.items = new List<DropDownItem>();
                    foreach (SYS_LookUpValue lv in Lvl)
                    {
                        DropDownItem Ddi = new DropDownItem();
                        Ddi.Id = lv.Id;
                        Ddi.text = lv.Name;
                        Dd.items.Add(Ddi);
                    }

                    Ddl.Add(Dd);
                }

                //Room
                Dd = new DropDown();
                Dd.title = "Room";
                Dd.items = new List<DropDownItem>();
                List<SYS_Room> Rooms = context.SYS_Room.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Room lt in Rooms)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Resource
                Dd = new DropDown();
                Dd.title = "Resource";
                Dd.items = new List<DropDownItem>();
                List<SYS_Resource> Resource = context.SYS_Resource.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Resource lt in Resource)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Surname + " " + lt.Othername;
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

        public async Task<BaseResponseDTO<bool>> Checkin(CheckInAccessLogDTO dataToSave, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                //string Email = context.SYS_Resource.Where(x => x.Id == dataToSave.Resource).FirstOrDefault().Email;
                //var emailexists = await _userManager.FindByEmailAsync(Email);

                string UserId = (from c in context.SYS_User
                                 join a in context.Users on c.AId equals a.Id
                                 join b in context.SYS_Resource on c.Id equals b.User.Id
                                 where c.DeleteStatus == false && c.Type == UserType.Resource && b.Id == dataToSave.Resource
                                 select c.AId.ToString()).FirstOrDefault();

                var user = await _userManager.FindByIdAsync(UserId);
                if (UserId == null)
                {
                    BaseDto.ErrorMessage = "Login Failed. You don't have access on this system";
                    BaseDto.Data = false;
                    BaseDto.QryResult = queryResult.FAILED;
                    return BaseDto;
                }

                bool PasswordValid = await _userManager.CheckPasswordAsync(user, dataToSave.Password);

                if (PasswordValid)
                {

                    SYS_AccessLog DSS = new SYS_AccessLog()
                    {
                        Room = context.SYS_Room.Where(x => x.Id == dataToSave.Room).FirstOrDefault(),
                        Resource = context.SYS_Resource.Where(x => x.Id == dataToSave.Resource).FirstOrDefault(),
                        Purpose = dataToSave.Purpose.HasValue ? context.SYS_LookUpValue.Where(x => x.Id == dataToSave.Purpose).FirstOrDefault() : null,
                        Remarks = string.IsNullOrEmpty(dataToSave.Remarks) ? string.Empty : dataToSave.Remarks,
                        Visitor = string.IsNullOrEmpty(dataToSave.Visitor) ? string.Empty : dataToSave.Visitor,
                        TimeIn = DateTime.Now,
                    };
                    context.SYS_AccessLog.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Check in Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;

                }
                else
                {

                    BaseDto.ErrorMessage = "Login Failed. You don't have access on this system";
                    BaseDto.Data = false;
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

        public async Task<BaseResponseDTO<bool>> Checkout(CheckOutAccessLogDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {

                SYS_AccessLog DSS = context.SYS_AccessLog.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                DSS.TimeOut = DateTime.Now;

                context.SYS_AccessLog.Update(DSS);
                context.SaveChanges();

                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Check out Successfully";
                BaseDto.QryResult = queryResult.SUCEEDED;

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
