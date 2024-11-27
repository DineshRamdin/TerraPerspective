using BL.Constants;
using BL.Models;
using BL.Models.Administration;
using BL.Models.Common;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Administration
{
    public class LoginService
    {
        public static async Task<BaseResponseDTO<ApplicationUser>> Login(LoginDTO loginDto, SignInManager<ApplicationUser> _signInManager, UserManager<ApplicationUser> _userManager, RoleManager<ApplicationRole> _roleManager, string clientIp,string MacAddress)
        {
            
            ApplicationUser user = new ApplicationUser();
            QueryResult queryResult = new QueryResult();
            string errorMsg = "";
            string sQryResult = queryResult.SUCEEDED;
            try
            {
                var emailexists = await _userManager.FindByEmailAsync(loginDto.Email);
                if (emailexists != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, loginDto.RememberMe, true);
                    user = await _userManager.FindByNameAsync(loginDto.Email);
                    if (result.Succeeded)
                    {
                        if (user.FirstTimeLogin == false)
                        {
                            user.LoginDetail = new LoginLog()
                            {
                                LastKnownIPAddress = clientIp,
                                LastKnownMACAddress = MacAddress,
                                LastLoginDate = DateTime.Now,
                                Description = "Success Login",
                                userId = user.Id,
                            };
                        }
                        else
                        {
							user.LoginDetail = new LoginLog()
							{
								LastKnownIPAddress = clientIp,
								LastKnownMACAddress = MacAddress,
								LastLoginDate = DateTime.Now,
								Description = "Success First Time Login",
								userId = user.Id,
							};
						}

                        try
                        {
                            if (user.LastPasswordChangedDate.HasValue)
                            {
                                int passwordParamForExpiry = Convert.ToInt32(new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("PasswordEpxiry"));
                                DateTime dateTimeLastPasswordChangedDate = user.LastPasswordChangedDate.Value.AddDays(passwordParamForExpiry);
                                if (dateTimeLastPasswordChangedDate < DateTime.UtcNow)
                                {
                                    errorMsg = "Password Expired.Please change your Password";
                                    user.Status = UserStatus.PasswordExpired;

                                }
                            }
						}
                        catch (Exception ex)
                        {

                        }

                    }
                    else if (result.IsLockedOut)
                    {
                        errorMsg = "Account Locked.Please Contact Administrator";
                        user.Status = UserStatus.AccountLocked;
                        await _userManager.UpdateAsync(user);
                        user = new ApplicationUser();
                        sQryResult = queryResult.FAILED;

                    }
                    else
                    {
                        int AccountLockedAttempts = Convert.ToInt32(new ApplicationSettingsHandler().settings.Params.GetValueOrDefault("AccountLockedAttempts"));
                        errorMsg = "Invalid Credentials!! Please note that your account will be locked after " + AccountLockedAttempts + " fail login attempts.";
                        user = new ApplicationUser();
                        sQryResult = queryResult.FAILED;
                    }
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    errorMsg = "Login Failed. You don't have access on this system";
                    user = new ApplicationUser();
                    sQryResult = queryResult.FAILED;
                }

            }
            catch (Exception ex)
            {
                //message += "exception login service line 116 \r\n" + ex;
                errorMsg = ex.ToString();
                sQryResult = queryResult.FAILED;
            }
            Guid oneTimeToken = Guid.NewGuid();
            //message += "login service line 121 \r\n" + sQryResult + "\r\n" +user.Status;
            //file.WriteLine(message);
            user.OTT = oneTimeToken;
            await _userManager.UpdateAsync(user);
            return AccountService.castUser(user, oneTimeToken, _userManager, _roleManager, sQryResult, errorMsg);
        }
    }
}
