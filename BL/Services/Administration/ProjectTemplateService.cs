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
    public class ProjectTemplateService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public ProjectTemplateService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ProjectTemplateDTO>> GetAll()
        {
            BaseResponseDTO<List<ProjectTemplateDTO>> dto = new BaseResponseDTO<List<ProjectTemplateDTO>>();
            ProjectTemplateDTO user = new ProjectTemplateDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<ProjectTemplateDTO> result = (from a in context.SYS_ProjectTemplate
                                                   where a.DeleteStatus == false
                                                   select new ProjectTemplateDTO
                                                   {
                                                       Id = a.Id,
                                                       ProjectTemplateName = a.ProjectTemplateName,
                                                   }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ProjectTemplateDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<ProjectTemplateDTO> GetById(long Id)
        {
            BaseResponseDTO<ProjectTemplateDTO> dto = new BaseResponseDTO<ProjectTemplateDTO>();
            ProjectTemplateDTO result = new ProjectTemplateDTO();
            try
            {
                result = (from a in context.SYS_ProjectTemplate
                          where a.DeleteStatus == false && a.Id == Id
                          select new ProjectTemplateDTO()
                          {
                              Id = a.Id,
                              ProjectTemplateName = a.ProjectTemplateName,
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
                dto.Data = new ProjectTemplateDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(ProjectTemplateDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_ProjectTemplate.Any(x => x.ProjectTemplateName.ToLower() == dataToSave.ProjectTemplateName.ToLower() && x.DeleteStatus != true))
                {
                    SYS_ProjectTemplate DSS = new SYS_ProjectTemplate()
                    {
                        ProjectTemplateName = dataToSave.ProjectTemplateName,
                    };
                    context.SYS_ProjectTemplate.Add(DSS);
                    context.SaveChanges();

                    SaveAndUpdateChildTable(dataToSave, DSS.Id);

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Project Template save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Project Template Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(ProjectTemplateDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_ProjectTemplate.Any(x => x.ProjectTemplateName.ToLower() == dataToUpdate.ProjectTemplateName.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_ProjectTemplate DSS = context.SYS_ProjectTemplate.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.ProjectTemplateName = dataToUpdate.ProjectTemplateName;

                    context.SYS_ProjectTemplate.Update(DSS);
                    context.SaveChanges();

                    SaveAndUpdateChildTable(dataToUpdate, DSS.Id);

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Project Template update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Project Template Already Exist";
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

        public string SaveAndUpdateChildTable(ProjectTemplateDTO data, long Id)
        {
            string result = "";
            try
            {
                // NPF_REG_FarmOccupyLandSpeciesMapping
                List<SYS_ProjectTemplateMapping> ProjectTemplateMappingData = context.SYS_ProjectTemplateMapping.Where(x => x.ProjectTemplateID == Id).ToList();

                if (data.ProjectTemplateData != null && data.ProjectTemplateData.Count > 0)
                {
                    foreach (var item in data.ProjectTemplateData)
                    {
                        if (ProjectTemplateMappingData.Any(y => y.Id == item.Id))
                        {
                            ProjectTemplateMappingData.RemoveAll(x => x.Id == item.Id);
                        }
                        else
                        {
                            SYS_ProjectTemplateMapping dt1 = new SYS_ProjectTemplateMapping()
                            {
                                TaskName = item.TaskName,
                                Duration = item.Duration,
                                Sequence = item.Sequence,
                                ProjectTemplateID = Id,
                                ParentTask=item.ParentTask,
                            };
                            context.SYS_ProjectTemplateMapping.Add(dt1);
                            context.SaveChanges();
                        }

                    }
                }

                if (ProjectTemplateMappingData.Count > 0)
                {
                    foreach (var item in ProjectTemplateMappingData)
                    {
                        item.DeleteStatus = true;
                        context.SYS_ProjectTemplateMapping.Update(item);
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

        public BaseResponseDTO<List<ProjectTemplateMappingDTO>> ProjectTemplateChildDataByParentId(long? id)
        {
            BaseResponseDTO<List<ProjectTemplateMappingDTO>> dto = new BaseResponseDTO<List<ProjectTemplateMappingDTO>>();
            List<ProjectTemplateMappingDTO> result = new List<ProjectTemplateMappingDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.SYS_ProjectTemplateMapping
                          where a.ProjectTemplateID == id && a.DeleteStatus == false
                          select new ProjectTemplateMappingDTO()
                          {
                              Id = a.Id,
                              ProjectTemplateID = a.ProjectTemplateID,
                              TaskName = a.TaskName,
                              Duration = a.Duration,
                              Sequence = a.Sequence,
                              ParentTask=a.ParentTask,

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
