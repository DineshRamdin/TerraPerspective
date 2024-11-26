﻿using DAL.Common;
using DAL.Context;
using DAL.Models;
using DAL.Models.Administration;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class SeedService
    {

        //Create Roles
        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            try
            {

                string[] roleNames = { "Administrator", "Statndard", "ReadOnly" };

                foreach (var roleName in roleNames)
                {
                    if (!roleManager.RoleExistsAsync(roleName).Result)
                    {
                        ApplicationRole role = new ApplicationRole();
                        role.Name = roleName;
                        role.NormalizedName = roleName;
                        IdentityResult roleResult = roleManager.
                        CreateAsync(role).Result;
                    }
                }

            }
            catch (Exception e)
            {

            }

        }

        //Create Users and Assign with Specific roles
        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {
            try
            {
                PerspectiveContext context = new PerspectiveContext();
                if (userManager.FindByNameAsync("admin@gmail.com").Result == null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.UserName = "admin@gmail.com";
                    user.Email = "admin@gmail.com";
                    user.NormalizedUserName = "admin@gmail.com";
                    user.NormalizedEmail = "admin@gmail.com";
                    user.Surname = "admin";
                    user.Othername = "admin";
                    user.PhoneNumber = string.Empty;
                    user.UserToken = Guid.Parse(user.Id);
                    user.Status = UserStatus.Active;
                    user.LoginDetail = new LoginLog()
                    {
                        //  LastKnownIPAddress = clientIp + " ~ " + Request.UserHostAddress.ToString(),
                        LastKnownIPAddress = "192.168.0.1",
                        LastLoginDate = DateTime.Now,
                        Description = "Success First Time Login",
                        userId = user.Id,
                        LastKnownMACAddress = string.Empty,
                    };


                    IdentityResult result = userManager.CreateAsync
                    (user, "Admin@123.").Result;


                    if (result.Succeeded)
                    {
                        SYS_User dt = new SYS_User()
                        {
                            AId = user.Id,
                            IsLoginAllowed = true,
                            Type = UserType.User,
                        };
                        context.SYS_User.Add(dt);
                        context.SaveChanges();

                        SYS_UserDetails nusr = new SYS_UserDetails()
                        {
                            User = context.SYS_User.Where(x => x.AId == user.Id).FirstOrDefault(),
                        };
                        context.SYS_UserDetails.Add(nusr);
                        context.SaveChanges();

                        userManager.AddToRoleAsync(user, "Administrator").Wait();
                    }
                }
            }
            catch (Exception e)
            {

            }

        }

		public static void SeedGlobalParam()
		{
			try
			{
				PerspectiveContext context = new PerspectiveContext();
				
				if (!context.SYS_GlobalParam.Any(x => x.Name == "SMTPServer"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
                    GP.Name = "SMTPServer";
                    GP.Value = "https://api.turbo-smtp.com/api/v2/mail/send";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}

				if (!context.SYS_GlobalParam.Any(x => x.Name == "SMTPAuthentication"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
					GP.Name = "SMTPAuthentication";
					GP.Value = "Enabled";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}

				if (!context.SYS_GlobalParam.Any(x => x.Name == "SMTPUsername"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
					GP.Name = "SMTPUsername";
					GP.Value = "dinu22@gmail.com";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}

				if (!context.SYS_GlobalParam.Any(x => x.Name == "SMTPPassword"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
					GP.Name = "SMTPPassword";
					GP.Value = "Terra@Test12345.";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}
				

				if (!context.SYS_GlobalParam.Any(x => x.Name == "SMTPPorts"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
					GP.Name = "SMTPPorts";
					GP.Value = "465";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}

				if (!context.SYS_GlobalParam.Any(x => x.Name == "APIKey"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
					GP.Name = "APIKey";
					GP.Value = "33d576a46b99e58fb454d4270e1b05b3";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}
			}
			catch (Exception e)
			{

			}

		}

		public static void SeedData(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedGlobalParam();

		}
    }
}
