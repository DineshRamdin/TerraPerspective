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

namespace BL.Services.Common
{
    public class SeedService
    {

        //Create Roles
        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            try
            {

                if (!roleManager.RoleExistsAsync("Administrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "Administrator";
                    role.NormalizedName = "ADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
                if (!roleManager.RoleExistsAsync("SuperAdministrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "SuperAdministrator";
                    role.NormalizedName = "SUPERADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
                if (!roleManager.RoleExistsAsync("Statndard").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "Statndard";
                    role.NormalizedName = "STATNDARD";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
                if (!roleManager.RoleExistsAsync("ReadOnly").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "ReadOnly";
                    role.NormalizedName = "READONLY";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


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

                        userManager.AddToRoleAsync(user, "SuperAdministrator").Wait();
                    }
                }
            }
            catch (Exception e)
            {

            }

        }

		public static void SeedTableCodeConfigurations(UserManager<ApplicationUser> userManager)
		{
			try
			{

				PerspectiveContext context = new PerspectiveContext();
				List<SYS_TableCodeConfigurations> LCconf = context.SYS_TableCodeConfigurations.Where(x => x.HasAddi == null).ToList();
				foreach (SYS_TableCodeConfigurations Cconf in LCconf)
				{
					Cconf.HasAddi = false;
					context.SYS_TableCodeConfigurations.Update(Cconf);
					context.SaveChanges();

				}
				string[] TblName = new string[] { "SYS_Projects-PRO" };
				Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault());
				SYS_CodeConfiguration cc = context.SYS_CodeConfiguration.Where(x => x.Name == "Default").FirstOrDefault();
				if (cc == null)
				{
					cc = new SYS_CodeConfiguration()
					{
						Name = "Default",
						Date = false,
						Month = false,
						Year = false,
						UsePrefix = true,
						PaddingNo = 0,
						DateFormat = string.Empty,
						YearFormat = string.Empty,
						MonthFormat = string.Empty,
						ResetConfig = string.Empty,
						Comment = "Default if no config Found",
						CreatedBy = createdBy,
						CreatedDate = DateTime.Now
					};
					context.SYS_CodeConfiguration.Add(cc);
					context.SaveChanges();
				}
				cc = new SYS_CodeConfiguration();
				cc = context.SYS_CodeConfiguration.Where(x => x.Name == "Laboratory").FirstOrDefault();
				if (cc == null)
				{
					cc = new SYS_CodeConfiguration()
					{
						Name = "Laboratory",
						Date = false,
						Month = false,
						Year = true,
						UsePrefix = true,
						PaddingNo = 10,
						DateFormat = string.Empty,
						YearFormat = "yyyy",
						MonthFormat = string.Empty,
						ResetConfig = "Year",
						Comment = "Default if no config Found",
						CreatedBy = createdBy,
						CreatedDate = DateTime.Now
					};
					context.SYS_CodeConfiguration.Add(cc);
					context.SaveChanges();
				}
				cc = new SYS_CodeConfiguration();
				cc = context.SYS_CodeConfiguration.Where(x => x.Name == "Schemes").FirstOrDefault();
				if (cc == null)
				{
					cc = new SYS_CodeConfiguration()
					{
						Name = "Schemes",
						Date = false,
						Month = false,
						Year = true,
						UsePrefix = true,
						PaddingNo = 10,
						DateFormat = string.Empty,
						YearFormat = "yyyy",
						MonthFormat = string.Empty,
						ResetConfig = "Year",
						Comment = "Default if no config Found",
						CreatedBy = createdBy,
						CreatedDate = DateTime.Now
					};
					context.SYS_CodeConfiguration.Add(cc);
					context.SaveChanges();
				}
				SYS_Company cl = context.SYS_Company.Where(x => x.NameofCompany == "Default").FirstOrDefault();
				if (cl == null)
				{
					cl = new SYS_Company()
					{
						NameofCompany = "Default",
						RegistrationNumber ="123",
						Code ="00",
						RegistrationDate = DateTime.Now,
						TelephoneNumber ="",
						MobileNumber = "",
						CreatedBy = createdBy,
						CreatedDate = DateTime.Now

					};
					context.SYS_Company.Add(cl);
					context.SaveChanges();
				}
				foreach (string name in TblName)
				{
					string[] splt = name.Split('-');
					SYS_TableCodeConfigurations tcc = context.SYS_TableCodeConfigurations.Where(x => x.TableName == splt[0] && x.CompanyId == cl.Id).FirstOrDefault();
					if (tcc == null)
					{
						tcc = new SYS_TableCodeConfigurations()
						{
							TableName = splt[0],
							Prefix = splt[1],
							CompanyId = Convert.ToInt32(cl.Id),
							ConfigurationId = cc.Id,
							Comment = "Default Table Config",
							CreatedBy = createdBy,
							CreatedDate = DateTime.Now,
							HasAddi = true
						};
						context.SYS_TableCodeConfigurations.Add(tcc);
						context.SaveChanges();
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

				if (!context.SYS_GlobalParam.Any(x => x.Name == "UniqueCodeGenerator"))
				{
					SYS_GlobalParam GP = new SYS_GlobalParam();
					GP.Name = "UniqueCodeGenerator";
					GP.Value = "0";
					context.SYS_GlobalParam.Add(GP);
					context.SaveChanges();
				}
			}
			catch (Exception e)
			{

            }

        }

        public static void SeedAcessRights(RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                PerspectiveContext context = new PerspectiveContext();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault());
                if (!roleManager.RoleExistsAsync("Administrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "Administrator";
                    role.NormalizedName = "ADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }
                ApplicationRole AdmRole = context.Roles.Where(x => x.Name == "Administrator").FirstOrDefault();
                context.Roles.Update(AdmRole);
                List<SYS_Modules> mdl = context.SYS_Modules.Where(x => x.DeleteStatus == false).ToList();
                if (mdl.Count > 0)
                {
                    foreach (SYS_Modules md in mdl)
                    {
                        if (!context.SYS_AccessRights.Any(x => x.Role.Id == AdmRole.Id && x.Module.Id == md.Id))
                        {
                            context.SYS_Modules.Update(md);
                            SYS_AccessRights ar = new SYS_AccessRights()
                            {
                                hasRight = true,
                                Role = AdmRole,
                                Module = md,
                                CreatedBy = createdBy,
                                CreatedDate = DateTime.Now,
                                DeleteStatus = false

                            };
                            context.SYS_AccessRights.Add(ar);
                            context.SaveChanges();

                        }

                    }
                }


            }
            catch (Exception e)
            {

            }

        }
        public static void SeedAcessRightsN(RoleManager<ApplicationRole> roleManager)
        {
            try
            {
                PerspectiveContext context = new PerspectiveContext();
                Guid createdBy = Guid.Parse(context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").Select(x => x.Id).FirstOrDefault());
                if (!roleManager.RoleExistsAsync("SuperAdministrator").Result)
                {
                    ApplicationRole role = new ApplicationRole();
                    role.Name = "SuperAdministrator";
                    role.NormalizedName = "SUPERADMIN";
                    IdentityResult roleResult = roleManager.
                    CreateAsync(role).Result;


                }

                ApplicationRole AdmRole = context.Roles.Where(x => x.Name == "SuperAdministrator").FirstOrDefault();
                context.Roles.Update(AdmRole);
                List<SYS_Modules> mdl = context.SYS_Modules.Where(x => x.DeleteStatus == false).ToList();
                if (mdl.Count > 0)
                {
                    foreach (SYS_Modules md in mdl)
                    {
                        if (!context.SYS_AccessRights.Any(x => x.Role.Id == AdmRole.Id && x.Module.Id == md.Id))
                        {
                            try
                            {
                                context.SYS_Modules.Update(md);
                                SYS_AccessRights ar = new SYS_AccessRights()
                                {
                                    hasRight = true,
                                    Role = AdmRole,
                                    Module = md,
                                    CreatedBy = createdBy,
                                    CreatedDate = DateTime.Now,
                                    DeleteStatus = false

                                };
                                context.SYS_AccessRights.Add(ar);
                                context.SaveChanges();

                                foreach (AccessOperationType g in Enum.GetValues(typeof(AccessOperationType)))
                                {
                                    SYS_AccessRightsDetails ARD = new SYS_AccessRightsDetails()
                                    {
                                        AccessRights = ar,
                                        AOT = g,
                                        Permission = true
                                    };
                                    context.SYS_AccessRightsDetails.Add(ARD);
                                    context.SaveChanges();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }
                }
                List<SYS_AccessRights> ACCL = context.SYS_AccessRights.Where(x => x.DeleteStatus == false).ToList();
                if (ACCL.Count > 0)
                {
                    foreach (SYS_AccessRights ACC in ACCL)
                    {
                        if (!context.SYS_AccessRightsDetails.Any(x => x.AccessRights.id == ACC.id && x.DeleteStatus == false && x.AOT == AccessOperationType.Export))
                        {
                            SYS_AccessRightsDetails ARD = new SYS_AccessRightsDetails()
                            {
                                AccessRights = ACC,
                                AOT = AccessOperationType.Export,
                                Permission = false
                            };
                            context.SYS_AccessRightsDetails.Add(ARD);
                            context.SaveChanges();
                        }
                        if (!context.SYS_AccessRightsDetails.Any(x => x.AccessRights.id == ACC.id && x.DeleteStatus == false && x.AOT == AccessOperationType.Report))
                        {
                            SYS_AccessRightsDetails ARD = new SYS_AccessRightsDetails()
                            {
                                AccessRights = ACC,
                                AOT = AccessOperationType.Report,
                                Permission = true
                            };
                            context.SYS_AccessRightsDetails.Add(ARD);
                            context.SaveChanges();
                        }
                    }
                }


            }
            catch (Exception e)
            {

            }

        }

        public static void SeedModules(UserManager<ApplicationUser> userManager)
        {
            PerspectiveContext context = new PerspectiveContext();
            var user = userManager.FindByNameAsync("admin@gmail.com").Result;

            #region Dashboard
            if (!context.SYS_Modules.Any(x => x.Name == "Dashboard"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Dashboard",
                    Url = "Dashboard/Index",
                    Order = 0,
                    Icon = "fas fa-th",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

			#region System Icon
			if (!context.SYS_Modules.Any(x => x.Name == "Projects"))
			{
				context.SYS_Modules.Add(new SYS_Modules()
				{
					Name = "Projects",
					Url = "Projects/Index",
					Order = 0,
					Icon = "fas fa-chart-pie",
					CreatedBy = Guid.Parse(user.Id),
					CreatedDate = DateTime.Now


				});
				context.SaveChanges();
			}
			#endregion

			#region User
			if (!context.SYS_Modules.Any(x => x.Name == "User"))
			{
				context.SYS_Modules.Add(new SYS_Modules()
				{
					Name = "User",
					Url = "User/Index",
					Order = 0,
					Icon = "fas fa-user",
					CreatedBy = Guid.Parse(user.Id),
					CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Poster
            if (!context.SYS_Modules.Any(x => x.Name == "Poster"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Poster",
                    Url = "Poster/Index",
                    Order = 0,
                    Icon = "fas fa-images",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Carousel
            if (!context.SYS_Modules.Any(x => x.Name == "Carousel"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Carousel",
                    Url = "Carousel/Index",
                    Order = 0,
                    Icon = "fas fa-images",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Device
            if (!context.SYS_Modules.Any(x => x.Name == "Device"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Device",
                    Url = "Device/Index",
                    Order = 0,
                    Icon = "fas fa-desktop",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region LookUpType
            if (!context.SYS_Modules.Any(x => x.Name == "LookUpType"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "LookUpType",
                    Url = "LookUpType/Index",
                    Order = 0,
                    Icon = "fas fa-cog",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region LookUpValue
            if (!context.SYS_Modules.Any(x => x.Name == "LookUpValue"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "LookUpValue",
                    Url = "LookUpValue/Index",
                    Order = 0,
                    Icon = "fas fa-cog",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Log and sub menus

            if (!context.SYS_Modules.Any(x => x.Name == "Log"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Log",
                    Url = "",
                    Order = 2,
                    Icon = "fas fa-history",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }

            if (!context.SYS_Modules.Any(x => x.Name == "Room"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Room",
                    Url = "Room/index",
                    Order = 3,
                    Icon = "far fa-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Log").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            if (!context.SYS_Modules.Any(x => x.Name == "Resource"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Resource",
                    Url = "Resource/index",
                    Order = 3,
                    Icon = "far fa-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Log").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }


            if (!context.SYS_Modules.Any(x => x.Name == "Check In/Out"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Check In/Out",
                    Url = "AccessLog/CheckInOut",
                    Order = 3,
                    Icon = "far fa-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Log").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }

            if (!context.SYS_Modules.Any(x => x.Name == "Access Log"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Access Log",
                    Url = "AccessLog/index",
                    Order = 3,
                    Icon = "far fa-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Log").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }


            #endregion

            #region Zone Management
            if (!context.SYS_Modules.Any(x => x.Name == "Zone Management"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Zone Management",
                    Url = "ZoneManagement/index",
                    Order = 0,
                    Icon = "fas fa-cog",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now
                });
                context.SaveChanges();
            }
            #endregion

            #region GlobalParam
            if (!context.SYS_Modules.Any(x => x.Name == "Global Param"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Global Param",
                    Url = "GlobalParam/index",
                    Order = 0,
                    Icon = "far fa-dot-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Testing
            if (!context.SYS_Modules.Any(x => x.Name == "Testing"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Testing",
                    Url = "Testing/index",
                    Order = 0,
                    Icon = "fas fa-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Administration And Submenu

            if (!context.SYS_Modules.Any(x => x.Name == "Administration"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Administration",
                    Url = "",
                    Order = 2,
                    Icon = "fas fa-user-cog",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }


            #region Access and Submenu
            if (!context.SYS_Modules.Any(x => x.Name == "Access"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Access",
                    Url = "",
                    Order = 3,
                    Icon = "fas fa-user-cog",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Administration").Select(x => x.Id).FirstOrDefault()
                });
                context.SaveChanges();
            }
            #endregion

            #region Access Rights
            if (!context.SYS_Modules.Any(x => x.Name == "Access Rights"))
            {

                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Access Rights",
                    Url = "AccessRights/index",
                    Order = 4,
                    Icon = "fa fa-circle-user",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            #endregion

            if (!context.SYS_Modules.Any(x => x.Name == "Matrix"))
            {

                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Matrix",
                    Url = "Matrix/Index",
                    Order = 24,
                    Icon = "fa fa-folder-tree",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            if (!context.SYS_Modules.Any(x => x.Name == "Menu"))
            {

                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Menu",
                    Url = "Menu/index",
                    Order = 24,
                    Icon = "fa-regular fa-square-caret-down",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now,
                    ParentId = context.SYS_Modules.Where(x => x.Name == "Access").Select(x => x.Id).FirstOrDefault()

                });
                context.SaveChanges();
            }
            #endregion

            #region Reports
            if (!context.SYS_Modules.Any(x => x.Name == "Report"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Report",
                    Url = "Reports/index",
                    Order = 0,
                    Icon = "fas fa-chart-pie",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region System Icon
            if (!context.SYS_Modules.Any(x => x.Name == "System Icon"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "System Icon",
                    Url = "SystemIcon/index",
                    Order = 0,
                    Icon = "fas fa-chart-pie",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region All System Icon
            if (!context.SYS_Modules.Any(x => x.Name == "All System Icon"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "All System Icon",
                    Url = "SystemIcon/AllSystemIcon",
                    Order = 0,
                    Icon = "fas fa-chart-pie",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Company
            if (!context.SYS_Modules.Any(x => x.Name == "Company"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Company",
                    Url = "Company/index",
                    Order = 0,
                    Icon = "fas fa-chart-pie",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Locality
            if (!context.SYS_Modules.Any(x => x.Name == "Locality"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Locality",
                    Url = "Locality/index",
                    Order = 0,
                    Icon = "far fa-dot-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Country
            if (!context.SYS_Modules.Any(x => x.Name == "Country"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "Country",
                    Url = "Country/index",
                    Order = 0,
                    Icon = "far fa-dot-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            #region Country
            if (!context.SYS_Modules.Any(x => x.Name == "MCA/VCA"))
            {
                context.SYS_Modules.Add(new SYS_Modules()
                {
                    Name = "MCA/VCA",
                    Url = "MCAVCA/index",
                    Order = 0,
                    Icon = "far fa-dot-circle",
                    CreatedBy = Guid.Parse(user.Id),
                    CreatedDate = DateTime.Now


                });
                context.SaveChanges();
            }
            #endregion

            List<SYS_Modules> dtl = context.SYS_Modules.Where(x => x.DisplayName == null).ToList();
            if (dtl.Count > 0)
            {
                foreach (SYS_Modules dt in dtl)
                {
                    dt.DisplayName = dt.Name;
                    context.SYS_Modules.Update(dt);
                    context.SaveChanges();
                }

            }
        }

        public static void SeedGroupMatrix(UserManager<ApplicationUser> userManager)
        {
            try
            {
                PerspectiveContext context = new PerspectiveContext();
                SYS_GroupMatrix RGM = new SYS_GroupMatrix();
                var Usr = context.Users.Where(x => x.Email.ToLower() == "admin@gmail.com").FirstOrDefault();
                var SysUsr = context.SYS_User.Where(x => x.AId == Usr.Id).FirstOrDefault();
                if (!context.SYS_GroupMatrix.Any(x => x.GMDescription == "ROOT"))
                {
                    RGM = new SYS_GroupMatrix();
                    RGM.ParentGMID = null;
                    RGM.GMDescription = "ROOT";
                    RGM.Remarks = "ROOT";
                    RGM.CompanyCode = "COMP1";
                    RGM.IsCompany = false;
                    RGM.LegalName = "ROOT";
                    RGM.CreatedBy = Guid.Parse(Usr.Id);
                    RGM.CreatedDate = DateTime.Now;
                    context.SYS_GroupMatrix.Add(RGM);
                    context.SaveChanges();
                }
                List<SYS_GroupMatrix> mlt = context.SYS_GroupMatrix.Where(x => x.ParentGMID == null && x.GMID != RGM.GMID).ToList();
                foreach (SYS_GroupMatrix m in mlt)
                {
                    m.ParentGMID = RGM.GMID;
                    context.SYS_GroupMatrix.Update(m);
                    context.SaveChanges();
                }
                if (!context.SYS_GroupMatrix.Any(x => x.GMDescription == "FAREI"))
                {
                    SYS_GroupMatrix GM = new SYS_GroupMatrix();
                    GM.ParentGMID = RGM.GMID;
                    GM.GMDescription = "FAREI";
                    GM.Remarks = "FAREI";
                    GM.CompanyCode = "COMP1";
                    GM.IsCompany = false;
                    GM.LegalName = "FAREI";
                    GM.CreatedBy = Guid.Parse(Usr.Id);
                    GM.CreatedDate = DateTime.Now;
                    context.SYS_GroupMatrix.Add(GM);
                    context.SaveChanges();
                }

                if (!context.SYS_GroupMatrixUser.Any(x => x.IID == SysUsr.Id))
                {
                    SYS_GroupMatrixUser GMU = new SYS_GroupMatrixUser();
                    GMU.IID = SysUsr.Id;
                    GMU.GMID = RGM.GMID;
                    GMU.CreatedBy = Guid.Parse(Usr.Id);
                    GMU.CreatedDate = DateTime.Now;
                    context.SYS_GroupMatrixUser.Add(GMU);
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
            SeedModules(userManager);
            SeedAcessRights(roleManager);
			SeedAcessRightsN(roleManager);
			SeedTableCodeConfigurations(userManager);
			SeedGroupMatrix(userManager);

        }
    }
}
