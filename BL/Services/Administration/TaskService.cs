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
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;

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

        public BaseResponseDTO<List<TaskDTO>> GetAll(string Email)
        {
            BaseResponseDTO<List<TaskDTO>> dto = new BaseResponseDTO<List<TaskDTO>>();
            TaskDTO user = new TaskDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                string AID = context.Users.Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
                if (Email != "admin@gmail.com")
                {
                    List<TaskDTO> result = (from prj in context.SYS_Projects
                                            join ts in context.SYS_Task on prj.Id equals ts.Projects.Id // Assuming it's ProjectId, not Id
                                            join pm in context.SYS_ProjectsMatrix on prj.Id equals pm.IID
                                            join gu in context.SYS_GroupMatrixUser on pm.IID equals gu.IID
                                            where ts.DeleteStatus == false && gu.IID == UsrId
                                            select new
                                            {
                                                ts.Id,
                                                ts.UserCode,
                                                ts.Taskname,
                                                ts.TaskDescription,
                                                ts.Projects,
                                                ts.Task,
                                                ts.StartDate,
                                                ts.EndDate,
                                                ts.Status,
                                                ts.Percentage,
                                                ts.IsVisible,
                                                ts.Folder,
                                            })
                        .Union(
                            context.SYS_Task
                                .Where(p => p.DeleteStatus == false && p.CreatedBy.ToString().ToLower() == AID.ToLower())
                                .Select(p => new
                                {
                                    p.Id,
                                    p.UserCode,
                                    p.Taskname,
                                    p.TaskDescription,
                                    p.Projects,
                                    p.Task,
                                    p.StartDate,
                                    p.EndDate,
                                    p.Status,
                                    p.Percentage,
                                    p.IsVisible,
                                    p.Folder
                                })
                        )
                        .AsEnumerable() // Move to client-side processing
                        .Select(x => new TaskDTO
                        {
                            Id = x.Id,
                            UserCode = x.UserCode,
                            Taskname = x.Taskname,
                            TaskDescription = x.TaskDescription,
                            ProjectName = (from c in context.SYS_Projects
                                           where c.Id == x.Projects.Id
                                           select c.ProjectName).FirstOrDefault(),
                            ParentTaskName = (x.Task == null) ? "" : (from c in context.SYS_Task
                                                                      where c.Id == x.Task.Id
                                                                      select c.Taskname).FirstOrDefault(),
                            StartDate = x.StartDate.ToString("yyyy/MM/dd"),
                            EndDate = x.EndDate.ToString("yyyy/MM/dd"),
                            Status = context.SYS_LookUpValue
                                             .Where(lv => lv.Id == x.Status) // Use a different lambda variable
                                             .FirstOrDefault()?.Name, // Use null-safe navigation
                            Percentage = x.Percentage,
                            IsVisible = x.IsVisible == true ? "Yes" : "No",
                            Folder = x.Folder,
                        })
                        .ToList();



                    dto.Data = result;
                }
                else
                {
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
                                                ParentTaskName = (from c in context.SYS_Task
                                                                  where c.Id == a.Task.Id
                                                                  select c.Taskname).FirstOrDefault(),
                                                StartDate = a.StartDate.ToString("yyyy/MM/dd"),
                                                EndDate = a.EndDate.ToString("yyyy/MM/dd"),
                                                Status = context.SYS_LookUpValue.Where(x => x.Id == a.Status).FirstOrDefault().Name,
                                                Percentage = a.Percentage,
                                                IsVisible = a.IsVisible == true ? "Yes" : "No",
                                                Folder = a.Folder,
                                            }).ToList();

                    dto.Data = result;
                }

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
                              Percentage = a.Percentage,
                              IsVisible = a.IsVisible,
                              Folder = a.Folder,
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

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues(string Email)
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

                List<SYS_Projects> projects = GetProjects(Email);
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
                List<SYS_Task> Task = GetTasks(Email);
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

        public List<SYS_Projects> GetProjects(string Email)
        {
            List<SYS_Projects> SYS_Projects = new List<SYS_Projects>();
            string AID = context.Users.Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();
            long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
            if (Email != "admin@gmail.com")
            {
                SYS_Projects = (from pm in context.SYS_ProjectsMatrix
                                join gu in context.SYS_GroupMatrixUser on pm.IID equals gu.IID
                                join prj in context.SYS_Projects on pm.IID equals prj.Id
                                where prj.DeleteStatus == false && gu.IID == UsrId
                                select new
                                {
                                    prj.Id,
                                    prj.ProjectName
                                })
                        .Union(
                            context.SYS_Projects
                                .Where(p => p.DeleteStatus == false && p.CreatedBy.ToString().ToLower() == AID.ToLower())
                                .Select(p => new
                                {
                                    p.Id,
                                    p.ProjectName
                                })
                        )
                        .AsEnumerable() // Move to client-side processing
                        .Select(x => new SYS_Projects
                        {
                            Id = x.Id,
                            ProjectName = x.ProjectName,
                        })
                        .ToList();
            }
            else
            {
                SYS_Projects = (from a in context.SYS_Projects
                                where a.DeleteStatus == false
                                select new SYS_Projects
                                {
                                    Id = a.Id,
                                    ProjectName = a.ProjectName,
                                }).ToList();
            }
            return SYS_Projects;
        }

        public List<SYS_Task> GetTasks(string Email)
        {
            List<SYS_Task> sYS_Tasks = new List<SYS_Task>();

            string AID = context.Users.Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();
            long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;
            if (Email != "admin@gmail.com")
            {
                sYS_Tasks = (from prj in context.SYS_Projects
                             join ts in context.SYS_Task on prj.Id equals ts.Projects.Id // Assuming it's ProjectId, not Id
                             join pm in context.SYS_ProjectsMatrix on prj.Id equals pm.IID
                             join gu in context.SYS_GroupMatrixUser on pm.IID equals gu.IID
                             where ts.DeleteStatus == false && gu.IID == UsrId
                             select new
                             {
                                 ts.Id,
                                 ts.Taskname,
                             })
                    .Union(
                        context.SYS_Task
                            .Where(p => p.DeleteStatus == false && p.CreatedBy.ToString().ToLower() == AID.ToLower())
                            .Select(p => new
                            {
                                p.Id,
                                p.Taskname,
                            })
                    )
                    .AsEnumerable() // Move to client-side processing
                    .Select(x => new SYS_Task
                    {
                        Id = x.Id,
                        Taskname = x.Taskname,
                    })
                    .ToList();
            }
            else
            {
                sYS_Tasks = (from a in context.SYS_Task
                             where a.DeleteStatus == false
                             select new SYS_Task
                             {
                                 Id = a.Id,
                                 Taskname = a.Taskname,
                             }).ToList();
            }

            return sYS_Tasks;
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
                    Percentage = dataToSave.Percentage,
                    IsVisible = dataToSave.IsVisible,
                    Folder = dataToSave.Folder,
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
                    Sys_Task.Percentage = dataToUpdate.Percentage;
                    Sys_Task.IsVisible = dataToUpdate.IsVisible;
                    Sys_Task.Folder = dataToUpdate.Folder;
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

        public BaseResponseDTO<List<ProjectViewEventDTO>> GetProjectViewEvent(DateTime startDate, DateTime endDate, string Email)
        {
            BaseResponseDTO<List<ProjectViewEventDTO>> dto = new BaseResponseDTO<List<ProjectViewEventDTO>>();

            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {
                string sQryResult = queryResult.FAILED;

                string AID = context.Users.Where(x => x.Email.ToLower() == Email.ToLower()).Select(x => x.Id).FirstOrDefault();
                long UsrId = context.SYS_User.Where(x => x.AId == AID).FirstOrDefault().Id;

                if (Email != "admin@gmail.com")
                {
                    List<ProjectViewEventDTO> result = (from prj in context.SYS_Projects
                                                        join ts in context.SYS_Task on prj.Id equals ts.Projects.Id // Assuming it's ProjectId, not Id
                                                        join pm in context.SYS_ProjectsMatrix on prj.Id equals pm.IID
                                                        join gu in context.SYS_GroupMatrixUser on pm.IID equals gu.IID
                                                        where ts.DeleteStatus == false && gu.IID == UsrId
                                                         &&
                                                  (
                                                       (ts.StartDate.Date >= startDate.Date &&
                                                       ts.StartDate.Date <= endDate.Date)
                                                   ||
                                                       (ts.EndDate.Date >= startDate.Date &&
                                                       ts.EndDate.Date <= endDate.Date)
                                                   ||
                                                       (startDate.Date >= ts.StartDate.Date &&
                                                       startDate.Date <= ts.EndDate.Date)
                                                   ||
                                                       (endDate.Date >= ts.StartDate.Date &&
                                                       endDate.Date <= ts.EndDate.Date)
                                                  )
                                                        select new
                                                        {
                                                            ts.Id,
                                                            ts.UserCode,
                                                            ts.Taskname,
                                                            ts.TaskDescription,
                                                            ts.Projects,
                                                            ts.Task,
                                                            ts.StartDate,
                                                            ts.EndDate,
                                                            ts.Status,
                                                            ts.Percentage,
                                                            ts.IsVisible,
                                                            ts.Folder,
                                                        })
                        .Union(
                            context.SYS_Task
                                .Where(p => p.DeleteStatus == false && p.CreatedBy.ToString().ToLower() == AID.ToLower())
                                .Select(p => new
                                {
                                    p.Id,
                                    p.UserCode,
                                    p.Taskname,
                                    p.TaskDescription,
                                    p.Projects,
                                    p.Task,
                                    p.StartDate,
                                    p.EndDate,
                                    p.Status,
                                    p.Percentage,
                                    p.IsVisible,
                                    p.Folder
                                })
                        )
                        .AsEnumerable() // Move to client-side processing
                        .Select(x => new ProjectViewEventDTO
                        {
                            Title = x.Taskname,
                            Description = x.TaskDescription,
                            ProjectName = x.Projects.ProjectName,
                            Start = x.StartDate.ToString("yyyy-MM-dd"),
                            End = x.EndDate.ToString("yyyy-MM-dd"),
                            Status = context.SYS_LookUpValue.Where(y => y.Id == x.Status).FirstOrDefault().Name,
                            color = string.IsNullOrEmpty(x.Projects.ProjectColorCode) ? "#3a87ad" : x.Projects.ProjectColorCode,
                        })
                        .ToList();

                    dto.Data = result;
                }
                else
                {
                    List<ProjectViewEventDTO> result = (from a in context.SYS_Task
                                                        where a.DeleteStatus == false
                                                        &&
                                   (
                                       a.StartDate.Date >= startDate.Date &&
                                       a.StartDate.Date <= endDate.Date ||

                                       a.EndDate.Date >= startDate.Date &&
                                       a.EndDate.Date <= endDate.Date ||

                                       startDate.Date >= a.StartDate.Date &&
                                       startDate.Date <= a.EndDate.Date ||

                                       endDate.Date >= a.StartDate.Date &&
                                       endDate.Date <= a.EndDate.Date
                                   )
                                                        select new ProjectViewEventDTO
                                                        {

                                                            Title = a.Taskname,
                                                            Description = a.TaskDescription,
                                                            ProjectName = a.Projects.ProjectName,
                                                            Start = a.StartDate.ToString("yyyy-MM-dd"),
                                                            End = a.EndDate.ToString("yyyy-MM-dd"),
                                                            Status = context.SYS_LookUpValue.Where(x => x.Id == a.Status).FirstOrDefault().Name,
                                                            color = string.IsNullOrEmpty(a.Projects.ProjectColorCode) ? "#3a87ad" : a.Projects.ProjectColorCode,
                                                        }).ToList();
                    dto.Data = result;
                }
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ProjectViewEventDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }
    }
}
