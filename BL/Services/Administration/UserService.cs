using BL.Constants;
using BL.Models.Administration;
using BL.Models.Common;
using BL.Services.Common;
using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
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
                                            RoleName = d.Name,
                                            ProfileImagebase64 = (from u in context.SYS_UserDetails
                                                                  where c.Id == u.User.Id
                                                                  select u.ProfileImagebase64).FirstOrDefault(),
                                            IsImage = (from u in context.SYS_UserDetails
                                                       where c.Id == u.User.Id
                                                       select u.ProfileImagebase64).FirstOrDefault() == null ? false : true,
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
                    au.FirstTimeLogin = true;
                    au.Status = UserStatus.ChangePassword;
                    //au.CreatedBy = Guid.Parse(dataToSave.userToken);
                    IdentityResult result = userManager.CreateAsync(au).Result;


                    if (result.Succeeded)
                    {
                        var roleName = roleManager.FindByIdAsync(dataToSave.Role);
                        userManager.AddToRoleAsync(au, roleName.Result.Name).Wait();


                        #region generate randdom password and sent to user by mail 
                        try
                        {
                            //var randPassword = "Admin@123.";//ConversionService.GenerateRandomPassword();
                            var randPassword = ConversionService.GenerateRandomPassword();
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

                            try
                            {

                                SYS_GlobalParam Gp = context.SYS_GlobalParam.Where(x => x.Name == "APIKey").FirstOrDefault();
                                SYS_GlobalParam GpFromEail = context.SYS_GlobalParam.Where(x => x.Name == "SMTPUsername").FirstOrDefault();
                                SYS_GlobalParam GpSMPTServer = context.SYS_GlobalParam.Where(x => x.Name == "SMTPServer").FirstOrDefault();
                                SYS_GlobalParam GpSMPTPassword = context.SYS_GlobalParam.Where(x => x.Name == "SMTPPassword").FirstOrDefault();
                                string APIKey = Gp.Value;
                                string fromEmail = GpFromEail.Value;
                                string url = GpSMPTServer.Value;
                                string pas = GpSMPTPassword.Value;
                                string contents = "Successfully create the Account.";
                                string emailBody = $"<!DOCTYPE html><html><head><style>body {{font-family: Arial, sans-serif; background-color: #f4f4f9; margin: 0; padding: 0;}} .email-container {{max-width: 600px; margin: 20px auto; background: #ffffff; padding: 20px; border: 1px solid #ddd; border-radius: 5px;}} .email-header {{font-size: 18px; font-weight: bold; color: #333333; margin-bottom: 10px;}} .email-body {{font-size: 14px; line-height: 1.6; color: #555555;}} .email-footer {{margin-top: 20px; font-size: 12px; color: #999999;}}</style></head><body><div class='email-container'><div class='email-header'>Dear Valued Customer,</div><div class='email-body'><p>Please note that you have been added to the Terra Perspective Application.</p><p>This is your login credentials:</p><p><strong>Email:</strong> {user.Email}</p><p><strong>Password:</strong> {randPassword}</p></div><div class='email-footer'><p>Thank you for using the Terra Perspective Application.</p><p>-- The Terra Perspective Team</p></div></div></body></html>";
                                BaseResponseDTO<bool> Emailbto = new BaseResponseDTO<bool>();
                                Emailbto = SendEmailService.SendEmailAsync(APIKey, fromEmail, user.Email, "Reset Password", emailBody, contents, pas, url);

                            }
                            catch (Exception ex)
                            {
                                dto.Data = false;
                                dto.ErrorMessage = "User save Sucessfully but couldn't mail user its password";
                                dto.QryResult = new QueryResult().FAILED;
                            }


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

        public async Task<BaseResponseDTO<bool>> ResetUserPassword(string Id, UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            string message = "";

            try
            {

                ApplicationUser au = context.Users.Where(x => x.Id == Id).FirstOrDefault();

                if (au != null)
                {

                    var randPassword = ConversionService.GenerateRandomPassword();
                    //var randPassword = ConversionService.GenerateRandomPassword();
                    var user = await userManager.FindByEmailAsync(au.Email);
                    var token = userManager.GeneratePasswordResetTokenAsync(user).Result; //generate password reset token
                    IdentityResult resultResetPasword = userManager.ResetPasswordAsync(user, token, randPassword).Result; //reset password using token
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    user.LastPasswordChangedDate = DateTime.UtcNow;
                    user.FirstTimeLogin = true;
                    user.Status = UserStatus.ChangePassword;
                    await userManager.UpdateAsync(user);
                    try
                    {

                        SYS_GlobalParam Gp = context.SYS_GlobalParam.Where(x => x.Name == "APIKey").FirstOrDefault();
                        SYS_GlobalParam GpFromEail = context.SYS_GlobalParam.Where(x => x.Name == "SMTPUsername").FirstOrDefault();
                        SYS_GlobalParam GpSMPTServer = context.SYS_GlobalParam.Where(x => x.Name == "SMTPServer").FirstOrDefault();
                        SYS_GlobalParam GpSMPTPassword = context.SYS_GlobalParam.Where(x => x.Name == "SMTPPassword").FirstOrDefault();
                        string APIKey = Gp.Value;
                        string fromEmail = GpFromEail.Value;
                        string url = GpSMPTServer.Value;
                        string pass = GpSMPTPassword.Value;
                        string contents = "Reset Password";
                        string emailBody = $"<!DOCTYPE html><html><head><style>body {{font-family: Arial, sans-serif; background-color: #f4f4f9; margin: 0; padding: 0;}} .email-container {{max-width: 600px; margin: 20px auto; background: #ffffff; padding: 20px; border: 1px solid #ddd; border-radius: 5px;}} .email-header {{font-size: 18px; font-weight: bold; color: #333333; margin-bottom: 10px;}} .email-body {{font-size: 14px; line-height: 1.6; color: #555555;}} .email-footer {{margin-top: 20px; font-size: 12px; color: #999999;}}</style></head><body><div class='email-container'><div class='email-header'>Dear Valued Customer,</div><div class='email-body'><p>Please note that we have reset your password for the Terra Perspective Application.</p><p>This is your login credentials after resetting the password:</p><p><strong>Email:</strong> {user.Email}</p><p><strong>Password:</strong> {randPassword}</p></div><div class='email-footer'><p>Thank you for using the Terra Perspective Application.</p><p>-- The Terra Perspective Team</p></div></div></body></html>";
                        BaseResponseDTO<bool> Emailbto = new BaseResponseDTO<bool>();
                        Emailbto = SendEmailService.SendEmailAsync(APIKey, fromEmail, user.Email, "Reset Password", emailBody, contents, pass, url);

                    }
                    catch (Exception ex)
                    {
                        dto.Data = false;
                        dto.ErrorMessage = "Reset User Password Sucessfully but couldn't mail user its password";
                        dto.QryResult = new QueryResult().FAILED;
                    }

                    dto.Data = true;
                    dto.ErrorMessage = "Reset User Password Successfully";
                    dto.QryResult = new QueryResult().SUCEEDED;

                }
                else
                {
                    dto.Data = false;
                    dto.ErrorMessage = "Reset User Password Fail";
                    dto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.ErrorMessage = "Reset User Password Fail";
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public async Task<BaseResponseDTO<bool>> ChangeUserPassword(ChangePasswordDTO changePasswordDTO, string Id, UserManager<ApplicationUser> userManager)
        {
            BaseResponseDTO<bool> dto = new BaseResponseDTO<bool>();
            string message = "";

            try
            {

                ApplicationUser au = context.Users.Where(x => x.Id == Id).FirstOrDefault();

                if (au != null)
                {

                    var randPassword = changePasswordDTO.NewPassword;
                    var user = await userManager.FindByEmailAsync(au.Email);
                    var token = userManager.GeneratePasswordResetTokenAsync(user).Result; //generate password reset token
                    IdentityResult resultResetPasword = userManager.ResetPasswordAsync(user, token, randPassword).Result; //reset password using token
                    user.LockoutEnd = null;
                    user.FirstTimeLogin = false;
                    user.AccessFailedCount = 0;
                    user.LastPasswordChangedDate = DateTime.UtcNow;
                    user.Status = UserStatus.Active;
                    await userManager.UpdateAsync(user);
                    dto.Data = true;
                    dto.ErrorMessage = "Change User Password Successfully";
                    dto.QryResult = new QueryResult().SUCEEDED;

                }
                else
                {
                    dto.Data = false;
                    dto.ErrorMessage = "Change User Password Fail";
                    dto.QryResult = new QueryResult().FAILED;
                }

            }
            catch (Exception ex)
            {
                dto.Data = false;
                dto.ErrorMessage = "Change User Password Fail";
                dto.QryResult = new QueryResult().FAILED;
            }

            return dto;
        }

        public string UserProfileImage(string Id)
        {
            string Profile = (from ur in context.SYS_User
                              join u in context.SYS_UserDetails on ur.Id equals u.User.Id
                              where ur.AId == Id
                              select u.ProfileImagebase64).FirstOrDefault();
            return Profile;
        }

        public BaseResponseDTO<List<UserDTO>> GetPreviewById(string id)
        {
            BaseResponseDTO<List<UserDTO>> dto = new BaseResponseDTO<List<UserDTO>>();
            List<UserDTO> result = new List<UserDTO>();
            string errorMsg = "No Data Found";
            try
            {

                result = (from a in context.Users
                          where a.Id == id && a.DeleteStatus == false
                          select new UserDTO()
                          {
                              Id = a.Id.ToString(),
                              Surname = a.Surname,
                              Othername = a.Othername,
                              Email = a.Email,
                              PhoneNumber = a.PhoneNumber,
                              Role = context.UserRoles.Where(x => x.UserId == id).FirstOrDefault().RoleId,
                              ProfileImagebase64 = (from ur in context.SYS_User
                                                    join u in context.SYS_UserDetails on ur.Id equals u.User.Id
                                                    where ur.AId == id
                                                    select u.ProfileImagebase64).FirstOrDefault(),
                              IsImage = (from ur in context.SYS_User
                                         join u in context.SYS_UserDetails on ur.Id equals u.User.Id
                                         where ur.AId == id
                                         select u.ProfileImagebase64).FirstOrDefault() == null ? false : true,

                          }).ToList();
                dto.Data = result;
                dto.QryResult = queryResult.SUCEEDED;

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
                dto.Data = result;
                dto.ErrorMessage = errorMsg;
                dto.QryResult = queryResult.FAILED;
            }

            return dto;
        }
    }
}
