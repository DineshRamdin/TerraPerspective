using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Context;
using DAL.Models.Administration;
using DAL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class TaskService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public TaskService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<TaskDTO>> GetAll()
        {
            BaseResponseDTO<List<TaskDTO>> dto = new BaseResponseDTO<List<TaskDTO>>();
            TaskDTO user = new TaskDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<TaskDTO> result = (from a in context.SYS_Task
                                            where a.DeleteStatus == false
                                            select new TaskDTO
                                            {
                                                Id = a.Id,
                                                UserCode = a.UserCode,
                                                Taskname = a.Taskname,
                                                TaskDescription = a.TaskDescription,
                                                ProjectName = (from c in context.SYS_Projects
                                                                  where c.Id == a.Projects.Id
                                                                  select c.ProjectName).FirstOrDefault(),
                                                ParentTaskName= (from c in context.SYS_Task
																 where c.Id == a.Task.Id
																 select c.Taskname).FirstOrDefault(),
												StartDate = a.StartDate.ToString("yyyy/MM/dd"),
                                                EndDate = a.EndDate.ToString("yyyy/MM/dd"),
                                                Status = context.SYS_LookUpValue.Where(x => x.Id == a.Status).FirstOrDefault().Name,
                                                StatusDetails = a.StatusDetails,
                                                IsVisible = a.IsVisible == true ? "Yes" : "No",
                                            }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<TaskDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<TaskCRUDDTO> GetById(long Id)
        {
            BaseResponseDTO<TaskCRUDDTO> dto = new BaseResponseDTO<TaskCRUDDTO>();
            TaskCRUDDTO result = new TaskCRUDDTO();
            try
            {
                result = (from a in context.SYS_Task
                          where a.DeleteStatus == false && a.Id == Id
                          select new TaskCRUDDTO()
                          {
                              Id = a.Id,
                              UserCode = a.UserCode,
                              TaskName = a.Taskname,
                              TaskDescription = a.TaskDescription,
                              Project = a.Projects.Id,
                              ParentTask = a.Task.Id,
                              StartDate = a.StartDate,
                              EndDate = a.EndDate,
                              Status = a.Status,
                              StatusDetails = a.StatusDetails,
                              IsVisible = a.IsVisible,
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
                dto.Data = new TaskCRUDDTO();
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
                string[] values = new string[] { "Project Status" };
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

                //User
                ApplicationUser result = new ApplicationUser();
                Dd = new DropDown();
                Dd.title = "Projects";
                Dd.items = new List<DropDownItem>();
                List<SYS_Projects> projects = context.SYS_Projects.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Projects lt in projects)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.ProjectName;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Parenttask
                Dd = new DropDown();
                Dd.title = "ParentTask";
                Dd.items = new List<DropDownItem>();
                List<SYS_Task> Task = context.SYS_Task.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Task lt in Task)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Taskname;
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

        public BaseResponseDTO<bool> SaveAsync(TaskCRUDDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
                string claimsPrincipal = null;
                string UserToken = "";
                if (httpContextAccessor.HttpContext != null)
                {
                    claimsPrincipal = httpContextAccessor.HttpContext.User.Identity.Name;
                    UserToken = httpContextAccessor.HttpContext.Session.GetString("OTT");
                    if (claimsPrincipal == null)
                    {
                        claimsPrincipal = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault();
                    }
                }
                BaseDtoS = new GenerateCode().GCodeMain(UserToken, "SYS_Task");
                SYS_Task dt = new SYS_Task()
                {
                    UserCode = BaseDtoS.Data,
                    Taskname = dataToSave.TaskName,
                    TaskDescription = dataToSave.TaskDescription,
                    Projects = context.SYS_Projects.Where(x => x.Id == dataToSave.Project).FirstOrDefault(),
                    Task = context.SYS_Task.Where(x => x.Id == dataToSave.ParentTask).FirstOrDefault(),
                    StartDate = dataToSave.StartDate,
                    EndDate = dataToSave.EndDate,
                    Status = dataToSave.Status,
                    StatusDetails = dataToSave.StatusDetails,
                    IsVisible = dataToSave.IsVisible,
                };
                context.SYS_Task.Add(dt);
                context.SaveChanges();
                BaseDto.ExtData = dt.Id.ToString();
                BaseDto.Data = true;
                BaseDto.ErrorMessage = "Task Added Successfully";
                BaseDto.QryResult = queryResult.SUCEEDED;
            }
            catch (Exception ex)
            {
                BaseDto.Data = false;
                BaseDto.ErrorMessage = "Error Adding Record";
                BaseDto.QryResult = queryResult.FAILED;
            }
            return BaseDto;
        }

        public BaseResponseDTO<bool> UpdateAsync(TaskCRUDDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_LookUpValue.Any(x => x.Name.ToLower() == dataToUpdate.TaskName.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Task Sys_Task = context.SYS_Task.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    Sys_Task.UserCode = dataToUpdate.UserCode;
                    Sys_Task.Taskname = dataToUpdate.TaskName;
                    Sys_Task.TaskDescription = dataToUpdate.TaskDescription;
                    Sys_Task.Projects = context.SYS_Projects.Where(x => x.Id == dataToUpdate.Project).FirstOrDefault();
                    Sys_Task.Task = context.SYS_Task.Where(x => x.Id == dataToUpdate.ParentTask).FirstOrDefault();
                    Sys_Task.StartDate = dataToUpdate.StartDate;
                    Sys_Task.EndDate = dataToUpdate.EndDate;
                    Sys_Task.Status = dataToUpdate.Status;
                    Sys_Task.StatusDetails = dataToUpdate.StatusDetails;
                    Sys_Task.IsVisible = dataToUpdate.IsVisible;
                    context.SYS_Task.Update(Sys_Task);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Task update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Task Already Exist";
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
