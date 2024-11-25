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
    public class UserService
    {
        private PerspectiveContext context;
        private QueryResult queryResult;

        public UserService()
        {
            context = new PerspectiveContext();
            queryResult = new QueryResult();
        }

        public BaseResponseDTO<List<UserDTO>> GetAll()
        {
            BaseResponseDTO<List<UserDTO>> dto = new BaseResponseDTO<List<UserDTO>>();
            UserDTO user = new UserDTO();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "No Data Found";

            try
            {

                string sQryResult = queryResult.FAILED;
                List<UserDTO> result = (from c in context.SYS_User
                                        join a in context.Users on c.AId equals a.Id
                                        join b in context.UserRoles on a.Id equals b.UserId
                                        join d in context.Roles on b.RoleId equals d.Id
                                        where c.DeleteStatus == false && c.Type == UserType.User
                                        select new UserDTO
                                        {
                                            Id = a.Id,
                                            Surname = a.Surname,
                                            Othername = a.Othername,
                                            Email = a.Email,
                                            Role = d.Id,
                                            RoleName = d.Name
                                        }).ToList();

                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

            }
            catch (Exception ex)
            {
                dto.Data = new List<UserDTO>();
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.SUCEEDED;
            }

            return dto;
        }

        public BaseResponseDTO<UserDTO> GetById(string Id)
        {
            BaseResponseDTO<UserDTO> dto = new BaseResponseDTO<UserDTO>();
            ApplicationUser result = new ApplicationUser();
            try
            {
                result = context.Users.Where(x => x.Id == Id).FirstOrDefault();
                UserDTO usr = new UserDTO()
                {
                    Id = Id,
                    Surname = result.Surname,
                    Othername = result.Othername,
                    Email = result.Email,
                    PhoneNumber = result.PhoneNumber,
                    Role = context.UserRoles.Where(x => x.UserId == Id).FirstOrDefault().RoleId,
                    ProfileImagebase64 = (from ur in context.SYS_User
                                          join u in context.SYS_UserDetails on ur.Id equals u.User.Id
                                          where ur.AId == Id
                                          select u.ProfileImagebase64).FirstOrDefault(),
                };

                if (result == null)
                {
                    dto.Data = usr;
                    dto.QryResult = queryResult.FAILED;
                }
                else
                {
                    dto.Data = usr;
                    dto.QryResult = queryResult.SUCEEDED;
                }
            }
            catch (Exception ex)
            {
                dto.Data = new UserDTO();
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }
        public BaseResponseDTO<List<DropDownLogin>> GetAllDropDownValues()
        {
            BaseResponseDTO<List<DropDownLogin>> dto = new BaseResponseDTO<List<DropDownLogin>>();
            List<DropDownLogin> Ddl = new List<DropDownLogin>();
            DropDownLogin Dd = new DropDownLogin();
            string errorMsg = "No Data Found";
            try
            {
                List<ApplicationRole> result = (from r in context.Roles
                                                where r.DeleteStatus == false
                                                select new ApplicationRole
                                                {
                                                    Id = r.Id,
                                                    Name = r.Name
                                                }
                                             ).GroupBy(p => p.Name).Select(g => g.First()).ToList();


                Dd.title = "Roles";
                Dd.items = new List<DropDownItemLogin>();
                foreach (ApplicationRole r in result)
                {
                    DropDownItemLogin Ddi = new DropDownItemLogin();
                    Ddi.id = r.Id;
                    Ddi.text = r.Name;
                    Dd.items.Add(Ddi);
                }
                Ddl.Add(Dd);

                dto.Data = Ddl;
                dto.QryResult = new QueryResult().SUCEEDED;
            }
            catch (Exception ex)
            {
                dto.Data = Ddl;
                dto.ErrorMessage = errorMsg;
                dto.QryResult = new QueryResult().FAILED;
            }
            return dto;
        }

        public async Task<BaseResponseDTO<bool>> SaveUser(UserDTO dataToSave, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            BaseResponseDTO<ApplicationUser> dt = new BaseResponseDTO<ApplicationUser>();
            string message = "";

            try
            {
                if (!userManager.Users.Any(x => x.Email.ToLower() == dataToSave.Email.ToLower() && x.DeleteStatus != true))
                {

                    ApplicationUser au = new ApplicationUser();
                    au.Id = Guid.NewGuid().ToString();
                    au.Surname = dataToSave.Surname;
                    au.Othername = dataToSave.Othername;
                    au.PhoneNumber = dataToSave.PhoneNumber;
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
                        var roleName = roleManager.FindByIdAsync(dataToSave.Role);
                        userManager.AddToRoleAsync(au, roleName.Result.Name).Wait();


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
                                Type = UserType.User,
                                IsLoginAllowed = true,
                            };
                            context.SYS_User.Add(usr);
                            context.SaveChanges();

                            SYS_UserDetails nusr = new SYS_UserDetails()
                            {
                                User = context.SYS_User.Where(x => x.AId == au.Id).FirstOrDefault(),
                                ProfileImagebase64 = dataToSave.ProfileImagebase64,

                            };
                            context.SYS_UserDetails.Add(nusr);
                            context.SaveChanges();


                            dto.Data = true;
                            dto.ErrorMessage = "User save Successfully";
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
                            dto.ErrorMessage = "User save Sucessfully but couldn't mail user its password";
                            dto.QryResult = new QueryResult().FAILED;
                        }
                        #endregion

                    }
                    else
                    {
                        dto.Data = false;
                        dto.ErrorMessage = "User save Fail";
                        dto.QryResult = new QueryResult().FAILED;
                    }

                }
                else
                {
                    dto.Data = false;
                    dto.ErrorMessage = "User Already Exist";
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

        public async Task<BaseResponseDTO<bool>> UpdateUser(UserDTO dataToUpdate, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            BaseResponseDTO<ApplicationUser> dt = new BaseResponseDTO<ApplicationUser>();
            string message = "";

            try
            {

                if (userManager.Users.Any(x => x.Email.ToLower() == dataToUpdate.Email.ToLower() && x.DeleteStatus != true && x.Id != dataToUpdate.Id))
                {
                    dto.Data = false;
                    dto.ErrorMessage = "User Already Exist";
                    dto.QryResult = queryResult.FAILED;
                    return dto;
                }

                ApplicationUser au = userManager.Users.Where(x => x.Id == dataToUpdate.Id).FirstOrDefault();

                au.Surname = dataToUpdate.Surname;
                au.Othername = dataToUpdate.Othername;
                au.PhoneNumber = dataToUpdate.PhoneNumber;
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

                    var roles = userManager.GetRolesAsync(au).Result;
                    if (roles.Count == 0)
                    {
                        var roleName = roleManager.FindByIdAsync(dataToUpdate.Role).Result;
                        userManager.AddToRoleAsync(au, roleName.Name).Wait();
                    }
                    else
                    {

                        var resultremoveroles = userManager.RemoveFromRolesAsync(au, roles).Result;
                        var roleName = roleManager.FindByIdAsync(dataToUpdate.Role).Result;
                        userManager.AddToRoleAsync(au, roleName.Name).Wait();
                    }


                    SYS_User usr = context.SYS_User.Where(x => x.AId == dataToUpdate.Id).FirstOrDefault();
                    usr.AId = dataToUpdate.Id;
                    usr.Type = UserType.User;
                    usr.IsLoginAllowed = true;
                    context.SYS_User.Update(usr);
                    context.SaveChanges();

                    SYS_UserDetails nusr = context.SYS_UserDetails.Where(x => x.User.Id == usr.Id).FirstOrDefault();
                    nusr.User = usr;
                    nusr.ProfileImagebase64 = dataToUpdate.ProfileImagebase64;

                    context.SYS_UserDetails.Update(nusr);
                    context.SaveChanges();

                    dto.Data = true;
                    dto.ErrorMessage = "User Update Successfully";
                    dto.QryResult = new QueryResult().SUCEEDED;

                }
                else
                {
                    dto.Data = false;
                    dto.ErrorMessage = "User save Fail";
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

    }
}
