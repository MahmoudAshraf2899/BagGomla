using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static BagGomla.Models.Enum;

namespace BagGomla.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public UserRole Role { set; get; }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!HttpContext.Current.Request.IsAuthenticated)
            {
                HandleUnauthorizedRequest(filterContext);
                return;
            }
            string userId = HttpContext.Current.User.Identity.GetUserId();
            if (Role != default && !UserManager.IsInRole(userId, ((int)Role).ToString()))
            {
                HandleUnauthorizedRequest(filterContext);
                return;
            }
            base.OnAuthorization(filterContext);
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("/Account/AccessDenied");
        }
    }
}