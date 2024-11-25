using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class ResourceService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public ResourceService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<ResourceDTO>> GetAll()
        {
            BaseResponseDTO<List<ResourceDTO>> dto = new BaseResponseDTO<List<ResourceDTO>>();
            ResourceDTO user = new ResourceDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<ResourceDTO> result = (from c in context.SYS_User
                                            join a in context.Users on c.AId equals a.Id
                                            join b in context.SYS_Resource on c.Id equals b.User.Id
                                            where c.DeleteStatus == false && c.Type == UserType.Resource
                                            select new ResourceDTO
                                            {
                                                Id = b.Id,
                                                Code = b.Code,
                                                Surname = b.Surname,
                                                Othername = b.Othername,
                                                Email = b.Email,
                                                Type = b.Type,
                                                MobileNo = b.MobileNo
                                            }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<ResourceDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<ResourceDTO> GetById(long Id)
        {
            BaseResponseDTO<ResourceDTO> dto = new BaseResponseDTO<ResourceDTO>();
            try
            {
                //result = context.Users.Where(x => x.Id == Id).FirstOrDefault();
                ResourceDTO result = (from c in context.SYS_User
                                      join a in context.Users on c.AId equals a.Id
                                      join b in context.SYS_Resource on c.Id equals b.User.Id
                                      where b.Id == Id && c.DeleteStatus == false && c.Type == UserType.Resource
                                      select new ResourceDTO
                                      {
                                          Id = b.Id,
                                          Code = b.Code,
                                          AId = c.AId,
                                          Surname = b.Surname,
                                          Othername = b.Othername,
                                          Email = b.Email,
                                          Type = b.Type,
                                          MobileNo = b.MobileNo
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
                dto.Data = new ResourceDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveUser(ResourceDTO dataToSave, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            BaseResponseDTO<ApplicationUser> dt = new BaseResponseDTO<ApplicationUser>();
            string message = "";

            try
            {
                if (!context.SYS_Resource.Any(x => x.Code.ToLower() == dataToSave.Code.ToLower() && x.DeleteStatus != true))
                {
                    if (!userManager.Users.Any(x => x.Email.ToLower() == dataToSave.Email.ToLower() && x.DeleteStatus != true))
                    {

                        ApplicationUser au = new ApplicationUser();
                        au.Id = Guid.NewGuid().ToString();
                        au.Surname = dataToSave.Surname;
                        au.Othername = dataToSave.Othername;
                        au.PhoneNumber = dataToSave.MobileNo;
                        au.UserToken = Guid.Parse(au.Id);
                        au.UserName = dataToSave.Email;
                        //au.PasswordHash = dataToSave.Data.PasswordHash;
                        au.Email = dataToSave.Email;
                        au.NormalizedUserName = dataToSave.Email.ToUpper();
                        au.NormalizedEmail = dataToSave.Email.ToUpper();
                        au.Status = UserStatus.Active;
                        au.CreatedDate = DateTime.Now;
                        //au.CreatedBy = Guid.Parse(dataToSave.userToken);
                        IdentityResult result = userManager.CreateAsync(au).Result;


                        if (result.Succeeded)
                        {

                            #region generate randdom password and sent to user by mail 
                            try
                            {
                                var randPassword = "Admin@123.";//ConversionService.GenerateRandomPassword();
                                var user = await userManager.FindByEmailAsync(au.Email);
                                var token = userManager.GeneratePasswordResetTokenAsync(user).Result; //generate password reset token
                                IdentityResult resultResetPasword = userManager.ResetPasswordAsync(user, token, randPassword).Result; //reset password using token
                                au.PasswordHash = randPassword;
                                dt.Data = au;

                                SYS_User usr = new SYS_User()
                                {
                                    AId = au.Id,
                                    Type = UserType.Resource,
                                    IsLoginAllowed = false,
                                };
                                context.SYS_User.Add(usr);
                                context.SaveChanges();

                                SYS_Resource nusr = new SYS_Resource()
                                {
                                    User = context.SYS_User.Where(x => x.AId == au.Id).FirstOrDefault(),
                                    Code = dataToSave.Code,
                                    Surname = dataToSave.Surname,
                                    Othername = dataToSave.Othername,
                                    Type = dataToSave.Type,
                                    MobileNo = dataToSave.MobileNo,
                                    Email = dataToSave.Email,

                                };
                                context.SYS_Resource.Add(nusr);
                                context.SaveChanges();


                                dto.Data = true;
                                dto.ErrorMessage = "Resource save Successfully";
                                dto.QryResult = new QueryResult().SUCEEDED;

                                //try
                                //{
                                //    BaseRequestDTO<TunnelApplicationUser> dtt = new BaseRequestDTO<TunnelApplicationUser>();
                                //    TunnelApplicationUser Tasur = new TunnelApplicationUser()
                                //    {
                                //        Title = au.Title,
                                //        Email = au.Email,
                                //        NormEmail = au.NormalizedEmail,
                                //        NormUsername = au.NormalizedUserName,
                                //        Username = au.UserName,
                                //        FirstName = au.FirstName,
                                //        LastName = au.LastName,
                                //        FkID = au.Id,
                                //    };
                                //    dtt.Data = Tasur;

                                //    NPF_SYS_GlobalParam Gp = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ExternalURL").FirstOrDefault();
                                //    string url = Gp.Value;
                                //    var x = new HttpRequestService().Call("post", url + "/Users/CreateTunnel", dtt);
                                //}
                                //catch (Exception ex)
                                //{

                                //}

                                ////Prepare email model to sent to user with one time password
                                //NPF_SYS_GlobalParam Gpa = context.NPF_SYS_GlobalParam.Where(x => x.Name == "ApplicationURL").FirstOrDefault();
                                //string URL = Gpa.Value;
                                //EmailSenderModel<dynamic> esm = new EmailSenderModel<dynamic>();
                                //esm.To = new List<string>() { user.Email };
                                //esm.Subject = "User Creation";
                                //esm.From = "noreply@naveo.mu";
                                //esm.Body = new List<string>() { "Dear Valued Customer", "Please note that you have added to the Naveo Platform Application. ", "This is your login credentials ; ", "Email : " + user.Email + " ", "Password : " + randPassword + "", "URL : " + URL + "" };
                            }
                            catch (Exception ex)
                            {
                                dto.Data = false;
                                dto.ErrorMessage = "Resource save Sucessfully but couldn't mail user its password";
                                dto.QryResult = new QueryResult().FAILED;
                            }
                            #endregion

                        }
                        else
                        {
                            dto.Data = false;
                            dto.ErrorMessage = "Resource save Fail";
                            dto.QryResult = new QueryResult().FAILED;
                        }

                    }
                    else
                    {
                        dto.Data = false;
                        dto.ErrorMessage = "Resource Already Exist";
                        dto.QryResult = new QueryResult().FAILED;
                    }
                }
                else
                {
                    dto.Data = false;
                    dto.ErrorMessage = "Resource Code Already Exist";
                    dto.QryResult = new QueryResult().FAILED;
                }
            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.ErrorMessage = "User save Fail";
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<bool>> UpdateUser(ResourceDTO dataToUpdate, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            BaseResponseDTO<ApplicationUser> dt = new BaseResponseDTO<ApplicationUser>();
            string message = "";

            try
            {

                if (!context.SYS_Resource.Any(x => x.Code.ToLower() == dataToUpdate.Code.ToLower() && x.DeleteStatus != true
              && x.Id != dataToUpdate.Id))
                {
                    dto.Data = false;
                    dto.ErrorMessage = "Resource Code Already Exist";
                    dto.QryResult = queryResult.FAILED;
                    return dto;
                }

                if (userManager.Users.Any(x => x.Email.ToLower() == dataToUpdate.Email.ToLower() && x.DeleteStatus != true && x.Id != dataToUpdate.AId))
                {
                    dto.Data = false;
                    dto.ErrorMessage = "User Already Exist";
                    dto.QryResult = queryResult.FAILED;
                    return dto;
                }

                ApplicationUser au = userManager.Users.Where(x => x.Id == dataToUpdate.AId).FirstOrDefault();

                au.Surname = dataToUpdate.Surname;
                au.Othername = dataToUpdate.Othername;
                au.PhoneNumber = dataToUpdate.MobileNo;
                au.UserName = dataToUpdate.Email;
                //au.PasswordHash = dataToSave.Data.PasswordHash;
                au.Email = dataToUpdate.Email;
                au.NormalizedUserName = dataToUpdate.Email.ToUpper();
                au.NormalizedEmail = dataToUpdate.Email.ToUpper();
                au.Status = UserStatus.Active;
                au.UpdatedDate = DateTime.Now;
                //au.UpdatedBy = Guid.Parse(dataToSave.userToken);
                IdentityResult result = userManager.UpdateAsync(au).Result;


                if (result.Succeeded)
                {
                    //var roleName = roleManager.FindByIdAsync(dataToUpdate.Role);
                    ////userManager.RemoveFromRoleAsync(au, roleName.Result.Name).Wait();
                    //userManager.AddToRoleAsync(au, roleName.Result.Name).Wait();

                    //var roles = userManager.GetRolesAsync(au).Result;
                    //if (roles.Count == 0)
                    //{
                    //    var roleName = roleManager.FindByIdAsync(dataToUpdate.Role).Result;
                    //    userManager.AddToRoleAsync(au, roleName.Name).Wait();
                    //}
                    //else
                    //{

                    //    var resultremoveroles = userManager.RemoveFromRolesAsync(au, roles).Result;
                    //    var roleName = roleManager.FindByIdAsync(dataToUpdate.Role).Result;
                    //    userManager.AddToRoleAsync(au, roleName.Name).Wait();
                    //}


                    SYS_User usr = context.SYS_User.Where(x => x.AId == dataToUpdate.AId).FirstOrDefault();
                    usr.AId = dataToUpdate.AId;
                    usr.Type = UserType.Resource;
                    usr.IsLoginAllowed = true;
                    context.SYS_User.Update(usr);
                    context.SaveChanges();

                    SYS_Resource nusr = context.SYS_Resource.Where(x => x.User.Id == usr.Id).FirstOrDefault();
                    nusr.User = usr;
                    nusr.Code = dataToUpdate.Code;
                    nusr.Surname = dataToUpdate.Surname;
                    nusr.Othername = dataToUpdate.Othername;
                    nusr.Type = dataToUpdate.Type;
                    nusr.MobileNo = dataToUpdate.MobileNo;
                    nusr.Email = dataToUpdate.Email;

                    context.SYS_Resource.Update(nusr);
                    context.SaveChanges();

                    dto.Data = true;
                    dto.ErrorMessage = "Resource Update Successfully";
                    dto.QryResult = new QueryResult().SUCEEDED;

                }
                else
                {
                    dto.Data = false;
                    dto.ErrorMessage = "Resource save Fail";
                    dto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.ErrorMessage = "Resource save Fail";
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

    }
}
