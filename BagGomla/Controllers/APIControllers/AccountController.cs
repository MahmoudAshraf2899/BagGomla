using BagGomla.Business;
using BagGomla.Enums;
using BagGomla.Helper;
using BagGomla.Models;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        #region Properties
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        private IAuthenticationManager Authentication
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }
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
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        #endregion
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public HttpResponseMessage LoginUser(LoginUserBindingModel model)
        {

            DatabaseContext db = new DatabaseContext();

            bool isEmail = model.Email.Contains("@");
            AspNetUsers user = null;
            if (isEmail)
                user = db.AspNetUsers.Where(a => a.Email.Equals(model.Email)).FirstOrDefault();
            else
                user = db.AspNetUsers.Where(c => c.PhoneNumber == model.Email).FirstOrDefault();
            if (user == null)
            {
                if (isEmail)
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { code = HttpStatusCode.NotFound, message = "هذا البريد غير موجود" });
                else
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { code = HttpStatusCode.NotFound, message = "رقم الهاتف غير موجود" });
            }
            //if (userbyemail.EmailConfirmed != true)
            //{
            //    return Request.CreateResponse(HttpStatusCode.NotFound, new { messgage = ResponseCode.UserNotConfirmed });
            //}
            PasswordVerificationResult result = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Success)
            {
                UserVM uservm = new UserVM();
                uservm.UserId = user.Id;
                uservm.Email = user.Email;
                uservm.Name = user.Name;
                uservm.ImageUrl = user.Image;
                uservm.Phone = user.PhoneNumber;
                #region Generate Token
                // Ugly hack: I use a server-side HTTP POST because I cannot directly invoke the service (it is deeply hidden in the OAuthAuthorizationServerHandler class)
                try
                {
                    WebClient webClient = new WebClient();
                    webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                    JavaScriptSerializer selialize = new JavaScriptSerializer();
                    var baseUrl = Request.RequestUri.AbsoluteUri.Split(new[] { Request.RequestUri.AbsolutePath }, StringSplitOptions.None)[0];
                    var toke = webClient.UploadString($"{baseUrl}/Token", "POST", $"grant_type=password&username={user.Email}&password={model.Password}");
                    var tokenMatchingObject = selialize.Deserialize<TokenAccess>(toke);
                    uservm.Token = tokenMatchingObject.Access_Token;
                }
                catch (Exception e)
                {
                }

                #endregion
                var message = Request.CreateResponse(HttpStatusCode.OK, uservm);
                message.Headers.Add("Access-Token", user.Id);
                return Request.CreateResponse(HttpStatusCode.OK, uservm);
            }
            else
            {
                //logging.Message = "Invalid UserName OR password";
                //HttpError error = new HttpError("UserName or Password Are Invalid") { { "Code", ResponseCode.InvalidUserOrPassword } };
                return Request.CreateResponse(HttpStatusCode.NotFound, new { code = HttpStatusCode.NotFound, message = "كلمة المرور غير صحيحة" });
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, new { message = "كلمة المرور غير صحيحة" });
            }
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<HttpResponseMessage> Register(APIRegisterViewModel model)
        {
            Response<UserVM> ResponseVM = new Response<UserVM>();
            try
            {
                DatabaseContext db = new DatabaseContext();
                string Message = "";
                if (ModelState.IsValid)
                {
                    var RoleID = "2";
                    model.Username = model.Email;
                    ApplicationUser user = new ApplicationUser { UserName = model.Username, Email = model.Email, PhoneNumber = model.Phone, RoleID = RoleID };
                    IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        AspNetUsers myUser = db.AspNetUsers.SingleOrDefault(c => c.Id == user.Id);
                        myUser.Address = model.Address;
                        myUser.Latitude = model.Latitude;
                        myUser.Longitude = model.Longitude;
                        myUser.IsSupplier = true;
                        myUser.Name = model.FullName;
                        myUser.ARName = model.FullName;
                        myUser.Email = model.Email;
                        myUser.IsSupplier = true;
                        FWYSupplierCooperation company = new FWYSupplierCooperation()
                        {
                            SupplierID = myUser.Id,
                            Phone = myUser.PhoneNumber,
                            Address = myUser.Address
                        };
                        db.FWYSupplierCooperation.Add(company);
                        db.SaveChanges();
                        SendActivationLink(myUser.Id);
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // generate token
                        WebClient webClient = new WebClient();
                        webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                        string uri = new Uri(Request.RequestUri.ToString()).ToString();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var baseUrl = Request.RequestUri.AbsoluteUri.Split(new[] { Request.RequestUri.AbsolutePath }, StringSplitOptions.None)[0];
                        var toke = webClient.UploadString($"{baseUrl}/Token", "POST", $"grant_type=password&username={model.Email}&password={model.Password}");
                        var tokenMatchingObject = js.Deserialize<TokenAccess>(toke);
                        UserVM myusr = new UserVM
                        {
                            Email = myUser.Email,
                            Name = myUser.Name,
                            UserId = myUser.Id,
                            Token = tokenMatchingObject.Access_Token
                        };
                        ResponseVM.DataResult = myusr;
                        ResponseVM.Code = ResponseCode.Success;
                        return Request.CreateResponse(HttpStatusCode.Created, myusr);
                    }
                    else
                    {
                        ResponseVM.Message = result.Errors.FirstOrDefault();
                        ResponseVM.Code = ResponseVM.Message == "is already taken" ? ResponseCode.UserAlreadyExist : ResponseCode.Error;
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
                }
            }
            catch
            {

            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
        }
        [HttpGet]
        [Route("Logout")]
        public HttpResponseMessage Logout()
        {
            Response ResponseVM = new Response();
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            ResponseVM.Code = ResponseCode.Success;
            return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
        }
        [HttpGet]
        [Route("ActivateAccount/{MyUserId}")]
        public HttpResponseMessage ActivateAccount(string MyUserId)
        {
            DatabaseContext db = new DatabaseContext();

            Response ResponseVM = new Response
            {
                Code = ResponseCode.AccountIsNotActive
            };
            var MyUser = db.AspNetUsers.SingleOrDefault(c => c.Id == MyUserId);
            if (MyUser != null)
            {
                if (MyUser.FWYSupplierCooperation.Count > 0)
                {
                    foreach (var item in MyUser.FWYSupplierCooperation)
                    {
                        item.IsAccepted = true;
                        item.IsRejected = false;
                    }
                }
                MyUser.EmailConfirmed = true;
            }

            if (db.SaveChanges() > 0)
            {
                ResponseVM.Code = ResponseCode.Success;
                return Request.CreateResponse(HttpStatusCode.Created, ResponseVM);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public HttpResponseMessage ForgotPasswordUser(ForgotPasswordViewModel model)
        {
            string userid = "";
            string usertoken = "";
            Response ResponseVM = new Response();
            if (ModelState.IsValid)
            {
                DatabaseContext db = new DatabaseContext();
                var user = db.AspNetUsers.FirstOrDefault(c => c.Email == model.Email);// await UserManager.FindByNameAsync(model.Email);
                if (user != null)
                {
                    string To = model.Email, UserID, Password, SMTPPort, Host;
                    string token = UserManager.GeneratePasswordResetToken(user.Id);
                    usertoken = token;
                    if (token == null)
                    {
                        ResponseVM.Code = ResponseCode.TryAgain;
                    }
                    else
                    {

                        //var lnkHref = "<a href='" + Url.Action("ResetPasswordUser", "Account", new { code = token, email = model.Email }, "http") + "'>Reset Password</a>";
                        string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                                         HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
                        var lnkHref = baseUrl + "/Account/ResetPasswordUser?code=" + token + "&&email=" + model.Email;
                        string subject = "Reset Your Password";
                        string body = "Please find the Password Reset Link. : " + lnkHref;
                        EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                        EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                        userid = UserID;
                    }
                    ResponseVM.Code = ResponseCode.Success;
                }
                else
                {
                    ResponseVM.Code = ResponseCode.UserNotFound;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Forgetpassword")]
        public HttpResponseMessage ForgetPassword(ForgotPasswordViewModel model)
        {
            Response ResponseVM = new Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    ResponseVM.Code = ResponseCode.EmailIsNoSent;
                    return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
                }
                DatabaseContext db = new DatabaseContext();

                AspNetUsers user = db.AspNetUsers.FirstOrDefault(p => p.Email == model.Email);
                if (user == null)
                {
                    ResponseVM.Code = ResponseCode.UserNotFound;
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
                }
                var forgotPasswordCode = (new Random().Next(100000, 999999)).ToString();
                user.ForgotPasswordCode = forgotPasswordCode;
                if (db.SaveChanges() > 0)
                {
                    string To = model.Email;
                    string subject = "Fogot your password ?";
                    string body = $"Reset The Password With The Code: {forgotPasswordCode} ";
                    EmailManager.AppSettings(out string UserID, out string Password, out string SMTPPort, out string Host);
                    EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                    ResponseVM.Code = ResponseCode.EmailHasSentToUser;
                }
                else
                {
                    ResponseVM.Code = ResponseCode.TryAgain;
                }
            }
            catch (Exception ex)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("ValidateForgetPasswordCode")]
        public HttpResponseMessage ValidateForgetPasswordCode(ForgotPasswordCodeValidatorViewModel model)
        {
            Response ResponseVM = new Response();
            try
            {
                if (!ModelState.IsValid)
                {
                    ResponseVM.Code = ResponseCode.ForgetPasswordCodeIsInvalid;
                }
                DatabaseContext db = new DatabaseContext();

                AspNetUsers user = db.AspNetUsers.FirstOrDefault(p => p.Email == model.Email && p.ForgotPasswordCode == model.Code && p.IsDeleted == false);
                if (user == null)
                {
                    ResponseVM.Code = ResponseCode.InvalidForgetPasswordCode;
                    return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
                }
                ResponseVM.Code = ResponseCode.Success;
            }
            catch (Exception ex)
            {

            }
            return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
        }
        [HttpPost]
        [Route("ResetPasswordWithForgetPasswordCode")]
        public async Task<HttpResponseMessage> ResetPasswordWithForgetPasswordCode(ResetPasswordWithForgetPasswordCodeApiViewModel model)
        {
            Response ResponseVM = new Response();
            if (!ModelState.IsValid)
            {
                ResponseVM.Code = ResponseCode.EmailOrPasswordAreInvalid;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
            }
            DatabaseContext db = new DatabaseContext();

            AspNetUsers user = db.AspNetUsers.FirstOrDefault(u => u.Email == model.Email && u.IsDeleted == false);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ResponseVM.Code = ResponseCode.InvalidForgetPasswordCode;
                return Request.CreateResponse(HttpStatusCode.NotFound, ResponseVM);
            }
            if (user.ForgotPasswordCode != model.Code)
            {
                ResponseVM.Code = ResponseCode.InvalidUserOrPassword;
                return Request.CreateResponse(HttpStatusCode.NotFound, ResponseVM);
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            if (code == null)
            {
                ResponseVM.Code = ResponseCode.CannotResetPassword;
                return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
            }
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (code != null & result.Succeeded)
            {
                string To = model.Email;
                string subject = "Password Is Reset";
                string body = $"You Have Reset Password Successfully";
                EmailManager.AppSettings(out string UserID, out string Password, out string SMTPPort, out string Host);
                EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
                ResponseVM.Code = ResponseCode.Success;
                return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, ResponseVM);
        }
        [HttpPost]
        [Authorize]
        public async Task<HttpResponseMessage> ResetPassword(ResetPasswordApiViewModel model)
        {
            Response ResponseVM = new Response();
            if (!ModelState.IsValid)
            {
                ResponseVM.Code = ResponseCode.CannotResetPassword;
                return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
            }
            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ResponseVM.Code = ResponseCode.UserNotFound;
                return Request.CreateResponse(HttpStatusCode.NotFound, ResponseVM);
            }
            SignInStatus result1 = await SignInManager.PasswordSignInAsync(user.UserName, model.OldPassword, isPersistent: false, shouldLockout: false);
            if (result1 != 0)
            {
                ResponseVM.Code = ResponseCode.InvalidUserOrPassword;
                return Request.CreateResponse(HttpStatusCode.NotFound, ResponseVM);
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            if (code == null)
            {
                ResponseVM.Code = ResponseCode.CannotResetPassword;
                return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
            }
            IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (code != null & result.Succeeded)
            {
                ResponseVM.Code = ResponseCode.Success;
                return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound, ResponseVM);
        }
        //[Authorize]
        //[HttpGet]
        //[Route("Profile")]
        //public HttpResponseMessage Profile()
        //{
        //    Response ResponseVM = new Response();
        //    ProfileAppService appService = new ProfileAppService();
        //    ResponseVM = appService.GetMyProfileInfo();
        //    if(ResponseVM.IsSccuessCode)
        //        return Request.CreateResponse(HttpStatusCode.OK, ResponseVM);
        //    return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
        //}
        public HttpResponseMessage AccessDenied()
        {
            Response ResponseVM = new Response();
            ResponseVM.Code = ResponseCode.AccessDenied;
            return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
        }
        #region Private Methods
        public void SendActivationLink(string Id)
        {
            DatabaseContext db = new DatabaseContext();
            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == Id);
            if (user.Email != null)
            {
                string To = user.Email, UserID, Password, SMTPPort, Host;
                var lnkHref = "<a href='" + Url.Route("DefaultApi", new { controller = "Account", action = "ActivateAccount", MyUserId = Id }) + "'>اضغط لتفعيل حسابك على موقع بيج جملة</a>";
                string subject = "فعل حسابك";
                string body = lnkHref;
                EmailManager.AppSettings(out UserID, out Password, out SMTPPort, out Host);
                EmailManager.SendEmail(UserID, subject, body, To, UserID, Password, SMTPPort, Host);
            }
        }
        #endregion

    }
}
