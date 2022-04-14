using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BagGomla.Models;
using System.Collections.Generic;
using IdentityLibrary.DataModel;
using System.IO;
using IdentityLibrary;
using BagGomla.Helper;
using System.Web.Hosting;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    public class AccountController : Lan
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string EmailOrPhone)
        {
            bool Check = EmailOrPhone.Contains("@");
            model.RememberMe = true;
            AspNetUsers user = null;
            if (Check)
                user = db.AspNetUsers.Where(c => c.Email == EmailOrPhone).FirstOrDefault();
            if (Check == false)
            {
                user = db.AspNetUsers.Where(c => c.PhoneNumber == EmailOrPhone).FirstOrDefault();
            }
            bool isBlocked = false;
            if (user != null)
            {
                if (user.IsDeleted == true)
                {
                    isBlocked = true;
                }
            }
            if (isBlocked == true)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Check)
            {
                var result = await SignInManager.PasswordSignInAsync(EmailOrPhone, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        {
                            var obj = user;
                            return RedirectToLocal(returnUrl, obj.RoleID);
                        }
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            else
            {
                var obj = user;
                if (obj != null)
                {
                    var result = await SignInManager.PasswordSignInAsync(obj.PhoneNumber, model.Password, model.RememberMe, shouldLockout: false);
                    switch (result)
                    {
                        case SignInStatus.Success:
                            {
                                return RedirectToLocal(returnUrl, obj.RoleID);
                            }
                        case SignInStatus.LockedOut:
                            return View("Lockout");
                        case SignInStatus.RequiresVerification:
                            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                        case SignInStatus.Failure:
                        default:
                            ModelState.AddModelError("", "Invalid login attempt.");
                            return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                }
                return View(model);
            }


        }

        private ActionResult RedirectToLocal(string returnUrl, string RoleID)
        {
            //var userid = User.Identity.GetUserId();
            //var user = db.AspNetUsers.SingleOrDefault(c => c.Id == userid);
            //var CompanyID = 0;
            //if(user.FWYSupplierCooperation.Count > 0)
            //{
            //    CompanyID = user.FWYSupplierCooperation.First().ID;
            //}
            if (returnUrl != null && returnUrl != "")
                return Redirect(returnUrl);

            if (RoleID == "1")
                return RedirectToAction("Dashboard", "Admin");
            //if (RoleID == "2")
            //    return RedirectToAction("EditSupProfile", "SupplierCorporation");
            else
                return RedirectToAction("Index", "Home");
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl, bool IsSupplier = true)
        {
            RegisterViewModel model = new RegisterViewModel() { IsSupplier = IsSupplier };
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var RoleID = "2";
                model.Username = model.Phone;
                var user = new ApplicationUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.Phone, RoleID = RoleID };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var myUser = db.AspNetUsers.SingleOrDefault(c => c.Id == user.Id);
                    myUser.Address = model.Address;
                    myUser.Latitude = model.Latitude;
                    myUser.Longitude = model.Longitude;
                    myUser.IsSupplier = true;
                    myUser.Name = model.FullName;
                    myUser.CreatedDateTime = DateTime.Now;

                    FWYSupplierCooperation company = new FWYSupplierCooperation()
                    {
                        SupplierID = myUser.Id,
                        Phone = myUser.PhoneNumber,
                        Address = myUser.Address,
                        Name = model.FullName
                    };

                    if (model.ArFullName == "" || model.ArFullName == null)
                    {
                        myUser.ARName = model.FullName;
                        company.ArName = model.FullName;
                    }
                    else
                    {
                        myUser.ARName = model.ArFullName;
                        company.ArName = model.ArFullName;
                    }
                    myUser.Email = model.Email;
                    myUser.IsSupplier = true;
                    
                    db.FWYSupplierCooperation.Add(company);
                    db.SaveChanges();
                    //foreach(var item in db.AspNetUsers.Where(c => c.RoleID == "1"))
                    //{
                    //    var Notification = new FWYNotification()
                    //    {
                    //        Title = "New Request",
                    //        Details = myUser.Name + " send a request",
                    //        DetailsURL = "/SupplierRequest/SupplierCompany/" + company.ID,
                    //        DateTime = DateTime.Now,
                    //        SendFrom = myUser.Id,
                    //        SendTo = item.Id,
                    //        ArTitle = "طلب جديد",
                    //        ArDetails = "قام " + myUser.ARName + " بارسال طلب جديد"
                    //    };
                    //    db.FWYNotification.Add(Notification);

                    //    db.SaveChanges();
                    //}
                    SendActivationLink(myUser.Id);
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    //return RedirectToLocal(returnUrl);
                    return RedirectToLocal(returnUrl, myUser.RoleID);

                }
                AddErrors(result);
            }


            ViewBag.returnUrl = returnUrl;
            

            return View(model);
        }
       
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult ForgotPasswordUser()
        {
            return View();
        }
        [HttpPost]

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordUser(ForgotPasswordViewModel model)
        {
            string userid = "";
            string usertoken = "";
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    string To = model.Email, UserID, Password, SMTPPort, Host;
                    string token = UserManager.GeneratePasswordResetToken(user.Id);
                    usertoken = token;
                    if (token == null)
                    {
                        return View("Index");
                    }
                    else
                    {
                        var lnkHref = "<a href='" + Url.Action("ResetPasswordUser", "Account", new { code = token, email = model.Email }, "http") + "'>Reset Password</a>";
                        string subject = "Your changed password";
                        string body = "<b>Please find the Password Reset Link. </b><br/>" + lnkHref;
                        EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                        EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                        userid = UserID;
                    }
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(1);
                }

            }
            return View(model);
        }

        public void SendActivationLink(string Id)
        {
            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == Id);
            if(user.Email != null)
            {
                string To = user.Email, UserID, Password, SMTPPort, Host;
                var lnkHref = "<a href='" + Url.Action("ActivateAccount", "Account", new { MyUserId = user.Id }, "http") + "'>اضغط لتفعيل حسابك على موقع بيج جملة</a>";
                string subject = "فعل حسابك";
                string body = lnkHref;
                EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
            }
        }

        public ActionResult ActivateAccount(string MyUserId)
        {
            var MyUser = db.AspNetUsers.SingleOrDefault(c => c.Id == MyUserId);
            if(MyUser != null)
            {
                if(MyUser.FWYSupplierCooperation.Count > 0)
                {
                    MyUser.FWYSupplierCooperation.First().IsAccepted = true;
                    MyUser.FWYSupplierCooperation.First().IsRejected = false;
                    db.SaveChanges();
                }
            }
            
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("EditSupProfile", "SupplierCorporation");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        //ChangePass
        [AllowAnonymous]
        public ActionResult ChangePassword(string ID ,string returnUrl)
        {
            if (ID != null)
            {
                var user =db.AspNetUsers.Find(ID);
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                ViewBag.TestOldPassword = user.PasswordHash;
                ViewBag.returnUrl = returnUrl;
                model.Email = user.Email;
                model.Id = user.Id;
                return View(model);
            }
            return RedirectToAction("AccessDenied");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ResetPasswordViewModel model ,string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = db.AspNetUsers.FirstOrDefault(c => c.Id == model.Id);
            if (user == null)
            {
                return RedirectToAction("AccessDenied");
            }
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.Password);
            user.PasswordHash = hashedNewPassword;
            db.SaveChanges();
            return RedirectToLocal(returnUrl);
        }
        public ActionResult CheckPass(string password, string confirmPasswordHash)
        {
            confirmPasswordHash = confirmPasswordHash.Replace(" ", "+");
            var Check = false;
            var result = UserManager.PasswordHasher.VerifyHashedPassword(confirmPasswordHash, password);
            if (result == PasswordVerificationResult.Success)
            {
                Check = true;
            }
            return Json(Check, JsonRequestBehavior.AllowGet);        
        }
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPasswordUser(string code, string email)
        {
            if(code != null && email != null)
            {
                ResetPasswordViewModel model = new ResetPasswordViewModel();
                model.Code = code;
                return View(model);
            }
            return RedirectToAction("AccessDenied");
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        ///[ValidateAntiForgeryToken]
        public ActionResult ResetPasswordUser(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = db.AspNetUsers.FirstOrDefault(c => c.Email == model.Email);
            if (user == null)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            string hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.Password);
            user.PasswordHash = hashedNewPassword;
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        [HttpGet]
        public ActionResult DownloadFile(int id, int type)
        {
            var company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == id);
            string base64 = "";
            string extension = "";
            if(type == 0)
            {
                base64 = company.NationalCopyID;
                extension = company.NationalCopyIDExtension;
            }
            else if(type == 1)
            {
                base64 = company.CommericialRegister;
                extension = company.CommericialRegisterExtension;
            }
            byte[] bytes = System.Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(bytes);
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet,"gehad");
        }

        [AllowAnonymous]
        public ActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpGet]
        public JsonResult IsUniqueEmail(string Email)
        {
            var Result = 0;
            if(Email != "" && Email != null)
            {
                var users = db.AspNetUsers.Where(c => c.Email.ToLower() == Email.ToLower()).ToList();
                if (users.Count > 0)
                {
                    Result = -2;
                }
                else
                {
                    Result = 1;
                }
            }
            
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult IsUniquePhone(string Phone)
        {
            var Result = 0;
            if (Phone != "" || Phone != null)
            {
                var users = db.AspNetUsers.Where(c => c.PhoneNumber.ToLower() == Phone.ToLower()).ToList();
                if (users.Count > 0)
                {
                    Result = -2;
                }
                else
                {
                    Result = 1;
                }
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> IsUserExisting(string EmailOrPhone, string Password)
        {
            var result = false;
            bool Check = EmailOrPhone.Contains("@");
            var users = db.AspNetUsers.Where(c => c.Email == EmailOrPhone).ToList();
            if (Check == false)
            {
                users = db.AspNetUsers.Where(c => c.PhoneNumber == EmailOrPhone).ToList();
            }

            if(users.Count > 0)
            {
                if(users.First().IsDeleted == false)
                {
                    var x = await SignInManager.PasswordSignInAsync(EmailOrPhone, Password, false, shouldLockout: false);
                    if (x.ToString() == "Success")
                        result = true;
                    else
                        result = false;
                }
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public async Task<ActionResult> IsUserExisting1(string EmailOrPhone, string Password)
        {
            var result = 0;
            bool Check = EmailOrPhone.Contains("@");
            var users = db.AspNetUsers.Where(c => c.Email == EmailOrPhone).ToList();
            if (Check == false)
            {
                users = db.AspNetUsers.Where(c => c.PhoneNumber == EmailOrPhone).ToList();
            }

            if (users.Count > 0)
            {
                if (users.First().IsDeleted == false)
                {
                    var x = await SignInManager.PasswordSignInAsync(EmailOrPhone, Password, false, shouldLockout: false);
                    if (x.ToString() == "Success")
                    {
                        result = 1;
                    }
                    else
                    {
                        result = -1;
                    }
                       
                }
                else
                {
                    result = 0;
                }
            }
            else
            {
                result = 0;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}