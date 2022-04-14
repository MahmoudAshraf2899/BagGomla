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
    public class UserController : Lan
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();
        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        // GET: User
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
        [ValidateAntiForgeryToken]
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
                    if (model.ArFullName == "" || model.ArFullName == null)
                    {
                        myUser.ARName = model.FullName;
                    }
                    else
                    {
                        myUser.ARName = model.ArFullName;
                    }
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

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    //return RedirectToLocal(returnUrl);
                    return RedirectToLocal(returnUrl, myUser.RoleID);

                }
                AddErrors(result);
            }


            ViewBag.returnUrl = returnUrl;


            return View(model);
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
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            if (RoleID == "1")
                return RedirectToAction("Dashboard", "Admin");
            if (RoleID == "2")
                return RedirectToAction("EditSupProfile", "SupplierCorporation");

            else
                return RedirectToAction("Index", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public ActionResult HomePage()
        {
            HomeModel model = new HomeModel();
            model.CategoryList = db.FWYCategory.Where(c => c.IsDeleted == false).ToList();
            model.NotificationList = db.FWYNotification.Where(c => c.IsDeleted == false && c.IsManual == true).ToList();
            //foreach (var item1 in model.NotificationList)
            //{
            //    item1.Image = helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
            //}  
            //foreach(var item in model.CategoryList)
            //{
            //    item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            //}
            return View(model);
        }

    }
}