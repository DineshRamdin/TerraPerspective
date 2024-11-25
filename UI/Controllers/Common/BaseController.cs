using DAL.Context;
using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;

namespace UI.Controllers.Common
{
    public class BaseController : Controller
    {
        private readonly PerspectiveContext _Dbcontext;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;
        public readonly RoleManager<ApplicationRole> _roleManager;

        public BaseController(UserManager<ApplicationUser> userManager,
                     SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> rolemanager, PerspectiveContext Dbcontext)
        {
            _Dbcontext = Dbcontext;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = rolemanager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                var httpContext = context.HttpContext;
                bool auth = httpContext.User.Identity.IsAuthenticated;
                var User = httpContext.User.Identity.Name;
                ISession session = HttpContext.Session;
                string OTT = session.GetString("OTT");
                string Username = session.GetString("Username");
                ApplicationUser usr = _Dbcontext.Users.Where(x => x.UserName == User).FirstOrDefault();
                if (usr != null)
                {
                    if (usr.OTT.ToString() != OTT)
                    {
                        auth = false;
                    }
                    else
                    {
                        //TODO: ADD PopUP Loggin from another browser
                    }
                }
                else
                {
                    auth = false;
                }
                if (!auth)
                {
                    if (httpContext.Request.Path.Value != "/" && !httpContext.Request.Path.Value.ToLower().Contains("login") && !httpContext.Request.Path.Value.ToLower().Contains("preview"))
                    {
                        context.Result = RedirectToRoute(new { controller = "Login", action = "index" });
                        return;
                    }
                }
                else
                {
                    if (httpContext.Request.Path.Value == "/" || (httpContext.Request.Path.Value.ToLower().Contains("login") && !httpContext.Request.QueryString.HasValue))
                    {
                        {
                            context.Result = RedirectToRoute(new { controller = "Dashboard", action = "Index" });
                            return;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText("c:\\errors.txt", ex.ToString());
            }
        }
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync(); // Signs out the user
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login"); // Redirect to home page or login page
        }
    }
}
