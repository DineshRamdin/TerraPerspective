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
                                               RegistrationDate = a.RegistrationDate,
                                               TelephoneNumber = a.TelephoneNumber,
                                               MobileNumber = a.MobileNumber,
                                               Email = a.Email,
                                               Locality = a.Locality,
                                               Country = a.Country,
                                               PostalCode = a.PostalCode,
                                               MCAVCA = a.MCAVCA,
                                               CompanyIcon = a.CompanyIcon,
                                               CompanyIconFlag = a.CompanyIcon == null ? false : true,
                                               Follow1 = a.Follow1,
                                               Follow2 = a.Follow2,
                                               Follow3 = a.Follow3,
                                               Follow4 = a.Follow4,
                                               Follow5 = a.Follow5,

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
                              RegistrationDate = a.RegistrationDate,
                              TelephoneNumber = a.TelephoneNumber,
                              MobileNumber = a.MobileNumber,
                              Email = a.Email,
                              Locality = a.Locality,
                              Country = a.Country,
                              PostalCode = a.PostalCode,
                              MCAVCA = a.MCAVCA,
                              CompanyIcon = a.CompanyIcon,
                              CompanyIconFlag = a.CompanyIcon == null ? false : true,
                              Follow1 = a.Follow1,
                              Follow2 = a.Follow2,
                              Follow3 = a.Follow3,
                              Follow4 = a.Follow4,
                              Follow5 = a.Follow5,

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
