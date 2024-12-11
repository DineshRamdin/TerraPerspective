using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class CompanyService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public CompanyService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<CompanyDTO>> GetAll()
        {
            BaseResponseDTO<List<CompanyDTO>> dto = new BaseResponseDTO<List<CompanyDTO>>();

            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<CompanyDTO> result = (from a in context.SYS_Company
                                           where a.DeleteStatus == false
                                           select new CompanyDTO
                                           {
                                               Id = a.Id,
                                               Code = a.Code,
                                               RegistrationNumber = a.RegistrationNumber,
                                               NameofCompany = a.NameofCompany,
                                               RegistrationDate = a.RegistrationDate.HasValue
                                                                 ? a.RegistrationDate.Value.ToString("dd/MMM/yyyy")
                                                                 : null,
                                               TelephoneNumber = a.TelephoneNumber,
                                               MobileNumber = a.MobileNumber,
                                               Email = a.Email,
                                               Locality = a.Locality,
                                               LocalityName = context.SYS_Locality.Where(x => x.Id == a.Locality).FirstOrDefault().Name,
                                               Country = a.Country,
                                               CountryName = context.SYS_Country.Where(x => x.Id == a.Country).FirstOrDefault().Name,
                                               PostalCode = a.PostalCode,
                                               MCAVCA = a.MCAVCA,
                                               MCAVCAName = context.SYS_MCAVCA.Where(x => x.Id == a.MCAVCA).FirstOrDefault().Name,
                                               CompanyIcon = a.CompanyIcon,
                                               CompanyIconFlag = a.CompanyIcon == null ? false : true,
                                               Colour1 = a.Colour1,
                                               Colour2 = a.Colour2,
                                               Colour3 = a.Colour3,
                                               Colour4 = a.Colour4,
                                               Colour5 = a.Colour5,

                                           }).ToList();


                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<CompanyDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }


        public BaseResponseDTO<List<CompanyDTO>> GetPreviewById(long? id)
        {
            BaseResponseDTO<List<CompanyDTO>> dto = new BaseResponseDTO<List<CompanyDTO>>();
            List<CompanyDTO> result = new List<CompanyDTO>();
            string errorMsg = "No Data Found";
            try
            {
                result = (from a in context.SYS_Company
                          where a.Id == id && a.DeleteStatus == false
                          select new CompanyDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              RegistrationNumber = a.RegistrationNumber,
                              NameofCompany = a.NameofCompany,
                              RegistrationDate = a.RegistrationDate.HasValue
                                         ? a.RegistrationDate.Value.ToString("dd/MMM/yyyy")
                                         : null,
                              TelephoneNumber = a.TelephoneNumber,
                              MobileNumber = a.MobileNumber,
                              Email = a.Email,
                              Locality = a.Locality,
                              LocalityName = context.SYS_Locality.Where(x => x.Id == a.Locality).FirstOrDefault().Name,
                              Country = a.Country,
                              CountryName = context.SYS_Country.Where(x => x.Id == a.Country).FirstOrDefault().Name,
                              PostalCode = a.PostalCode,
                              MCAVCA = a.MCAVCA,
                              MCAVCAName = context.SYS_MCAVCA.Where(x => x.Id == a.MCAVCA).FirstOrDefault().Name,
                              CompanyIcon = a.CompanyIcon,
                              CompanyIconFlag = a.CompanyIcon == null ? false : true,
                              Colour1 = a.Colour1,
                              Colour2 = a.Colour2,
                              Colour3 = a.Colour3,
                              Colour4 = a.Colour4,
                              Colour5 = a.Colour5,

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

        public BaseResponseDTO<CompanyDTO> GetById(long Id)
        {
            BaseResponseDTO<CompanyDTO> dto = new BaseResponseDTO<CompanyDTO>();
            CompanyDTO result = new CompanyDTO();
            try
            {
                result = (from a in context.SYS_Company
                          where a.DeleteStatus == false && a.Id == Id
                          select new CompanyDTO()
                          {
                              Id = a.Id,
                              Code = a.Code,
                              RegistrationNumber = a.RegistrationNumber,
                              NameofCompany = a.NameofCompany,
                              RegistrationDate = a.RegistrationDate.ToString(),
                              TelephoneNumber = a.TelephoneNumber,
                              MobileNumber = a.MobileNumber,
                              Email = a.Email,
                              Locality = a.Locality,
                              LocalityName = context.SYS_Locality.Where(x => x.Id == a.Locality).FirstOrDefault().Name,
                              Country = a.Country,
                              CountryName = context.SYS_Country.Where(x => x.Id == a.Country).FirstOrDefault().Name,
                              PostalCode = a.PostalCode,
                              MCAVCA = a.MCAVCA,
                              MCAVCAName = context.SYS_MCAVCA.Where(x => x.Id == a.MCAVCA).FirstOrDefault().Name,
                              CompanyIcon = a.CompanyIcon,
                              CompanyIconFlag = a.CompanyIcon == null ? false : true,
                              Colour1 = a.Colour1,
                              Colour2 = a.Colour2,
                              Colour3 = a.Colour3,
                              Colour4 = a.Colour4,
                              Colour5 = a.Colour5,
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
                dto.Data = new CompanyDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveAsync(CompanyDTO dataToSave)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            BaseResponseDTO<string> BaseDtoS = new BaseResponseDTO<string>();
            try
            {
                if (!context.SYS_Company.Any(x => x.NameofCompany.ToLower() == dataToSave.NameofCompany.ToLower() && x.DeleteStatus != true))
                {
                    SYS_Company DSS = new SYS_Company()
                    {
                        NameofCompany = dataToSave.NameofCompany,
                        CompanyIcon = dataToSave.CompanyIcon,
                        Code = dataToSave.Code,
                        RegistrationNumber = dataToSave.RegistrationNumber,
                        RegistrationDate = Convert.ToDateTime(dataToSave.RegistrationDate),
                        TelephoneNumber = dataToSave.TelephoneNumber == null ? "" : dataToSave.TelephoneNumber,
                        MobileNumber = dataToSave.MobileNumber,
                        Email = dataToSave.Email,
                        Locality = dataToSave.Locality,
                        Country = dataToSave.Country,
                        PostalCode = dataToSave.PostalCode,
                        MCAVCA = dataToSave.MCAVCA,
                        Colour1 = dataToSave.Colour1,
                        Colour2 = dataToSave.Colour2,
                        Colour3 = dataToSave.Colour3,
                        Colour4 = dataToSave.Colour4,
                        Colour5 = dataToSave.Colour5,

                    };
                    context.SYS_Company.Add(DSS);
                    context.SaveChanges();

                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Company save Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Company Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateAsync(CompanyDTO dataToUpdate)
        {
            BaseResponseDTO<bool> BaseDto = new BaseResponseDTO<bool>();
            try
            {
                if (!context.SYS_Company.Any(x => x.NameofCompany.ToLower() == dataToUpdate.NameofCompany.ToLower() && x.DeleteStatus != true
                && x.Id != dataToUpdate.Id))
                {
                    SYS_Company DSS = context.SYS_Company.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                    DSS.NameofCompany = dataToUpdate.NameofCompany;
                    DSS.CompanyIcon = dataToUpdate.CompanyIcon;
                    DSS.Code = dataToUpdate.Code;
                    DSS.RegistrationNumber = dataToUpdate.RegistrationNumber;
                    DSS.RegistrationDate = Convert.ToDateTime(dataToUpdate.RegistrationDate);
                    DSS.TelephoneNumber = dataToUpdate.TelephoneNumber == null ? "" : dataToUpdate.TelephoneNumber;
                    DSS.MobileNumber = dataToUpdate.MobileNumber;
                    DSS.Email = dataToUpdate.Email;
                    DSS.Locality = dataToUpdate.Locality;
                    DSS.Country = dataToUpdate.Country;
                    DSS.PostalCode = dataToUpdate.PostalCode;
                    DSS.MCAVCA = dataToUpdate.MCAVCA;
                    DSS.Colour1 = dataToUpdate.Colour1;
                    DSS.Colour2 = dataToUpdate.Colour2;
                    DSS.Colour3 = dataToUpdate.Colour3;
                    DSS.Colour4 = dataToUpdate.Colour4;
                    DSS.Colour5 = dataToUpdate.Colour5;

                    context.SYS_Company.Update(DSS);
                    context.SaveChanges();
                    BaseDto.Data = true;
                    BaseDto.ErrorMessage = "Company update Successfully";
                    BaseDto.QryResult = queryResult.SUCEEDED;
                }
                else
                {
                    BaseDto.Data = false;
                    BaseDto.ErrorMessage = "Company Already Exist";
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

        public BaseResponseDTO<List<DropDown>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDown>> dto = new BaseResponseDTO<List<DropDown>>();
            List<DropDown> Ddl = new List<DropDown>();
            DropDown Dd = new DropDown();
            string errorMsg = "No Data Found";
            try
            {

                //Locality
                Dd = new DropDown();
                Dd.title = "Locality";
                Dd.items = new List<DropDownItem>();
                List<SYS_Locality> Locality = context.SYS_Locality.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Locality lt in Locality)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //Country
                Dd = new DropDown();
                Dd.title = "Country";
                Dd.items = new List<DropDownItem>();
                List<SYS_Country> Country = context.SYS_Country.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_Country lt in Country)
                {
                    DropDownItem Ddi = new DropDownItem();
                    Ddi.Id = lt.Id;
                    Ddi.text = lt.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                //MCAVCA
                Dd = new DropDown();
                Dd.title = "MCAVCA";
                Dd.items = new List<DropDownItem>();
                List<SYS_MCAVCA> MCAVCA = context.SYS_MCAVCA.Where(x => x.DeleteStatus == false).ToList();
                foreach (SYS_MCAVCA lt in MCAVCA)
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


    }
}
