﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using MudHook.UI.Areas.Admin.Models;
using MudHook.Core;
using System.Text;

namespace MudHook.UI.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {

        public MudHookMembershipProvider MembershipService { get; set; }
        public MudHookRoleProvider AuthorizationService { get; set; }
        private MudHookRepository repo = new MudHookRepository();
   
        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null)
                MembershipService = new MudHookMembershipProvider();
            if (AuthorizationService == null)
                AuthorizationService = new MudHookRoleProvider();

            base.Initialize(requestContext);
        }    


        public ActionResult Index()
        {
            return View(repo.GetAllUsers());
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(User user)
        {
            user.Role = repo.GetRole(user.RoleId);
            if (ModelState.IsValid)
            {
                try
                {
                    MembershipService.CreateUser(user.Username, user.RealName, user.Password, user.Password, user.Role.Name);

                    MudHookNotifications.Set(new Notification("success", "User was successfully added."));

                    return RedirectToAction("Index", "Users");
                }
                catch (ArgumentException ae)
                {
                    MudHookNotifications.Set(new Notification("error", ae.Message));
                }
            }

            return View(user);
        }

        public ActionResult Edit(int id)
        {
            return View(repo.GetUser(id));
        }

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = repo.GetUser(user.Id).Password;
                }
                else
                {
                    user.Password =  Convert.ToBase64String(
                        MudHookSecurity.GenerateSaltedHash(Encoding.UTF8.GetBytes(user.Password), Encoding.UTF8.GetBytes(user.Username.ToLower())));
                }
                repo.EditUser(user);
                MudHookNotifications.Set(new Notification("success", "User has been updated"));
                RedirectToAction("Index");
            }
            else
            {
                var allErrors = ModelState.Values.SelectMany(v => v.Errors);
                string message = "";
                foreach (var e in allErrors)
                {
                    message += e.ErrorMessage + ",";
                }
                MudHookNotifications.Set(new Notification("error", message.TrimEnd()));
            }
            return View(user);
        }

        public ActionResult LogOn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Posts");
                    }
                }
                else
                {
                    MudHookNotifications.Set(new Notification("error", "The user name or password provided is incorrect."));                    
                }
            }
            
            return View(model);
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                
                try
                {
                    MembershipService.CreateUser(model.UserName, model.RealName, model.Password, model.Email, "Member");

                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                catch (ArgumentException ae)
                {
                    ModelState.AddModelError("", ae.Message);
                }
            }
            
            return View(model);
        }
       

        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [RequireHttps]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (MembershipService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword))
                    return RedirectToAction("ChangePasswordSuccess");
                else
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }
            
            return View(model);
        }        

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            repo.DeleteUser(id);
            MudHookNotifications.Set(new Notification("success", "User has been deleted"));
            return RedirectToAction("Index");
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
