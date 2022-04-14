using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using static BagGomla.Models.Enum;
using BagGomla.Helper;
using BagGomla.Models;
using System.Collections.Generic;

namespace BagGomla.Controllers
{
    [Authorize]
    public class CustomerController : Lan
    {
        private DatabaseContext db = new DatabaseContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private TagHelper helper = new TagHelper();

        public CustomerController()
        {

        }
        public CustomerController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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



        [HttpGet]
        public ActionResult EditCustomerProfile(string id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    if(id != null)
                    {
                        userID = id;
                    }
                    var user = db.AspNetUsers.FirstOrDefault(c => c.Id == userID);
                    //user.Image = helper.ConnvertToImageSrc(user.Image, user.ImageExtension);
                    //foreach (var item1 in user.FWYInvoiceOrder)
                    //{
                    //    foreach (var item2 in item1.FWYInvoiceOrderProduct)
                    //    {
                    //        if (item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().Count > 0)
                    //        {
                    //            item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().First().Image =
                    //                helper.ConnvertToImageSrc(item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().First().Image,
                    //                item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().First().ImageExtension);
                    //        }
                    //    }
                    //}
                    return View(user);
                }
                else
                {
                    return RedirectToAction("Login", "Home", new { returnUrl = "/Customer/GetOrders" });
                }
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpPost]
        public async Task<ActionResult> EditCustomerProfile(AspNetUsers model, System.Web.HttpPostedFileBase Image)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                if (ModelState.IsValid)
                {
                    var myUser = db.AspNetUsers.FirstOrDefault(c => c.Id == model.Id);
                    var user = await UserManager.FindByIdAsync(model.Id);

                    var Check = db.AspNetUsers.Where(c => c.Email == model.Email || c.UserName == model.UserName || c.PhoneNumber == model.PhoneNumber).ToList();
                    if (Check.Count == 1)
                    {
                        if (myUser.Id == Check[0].Id)
                        {
                            //String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.PasswordHash);                      
                            myUser.Name = model.Name;
                            //myUser.UserName = model.UserName;
                            myUser.Email = model.Email;
                            myUser.Address = model.Address;
                            myUser.Latitude = model.Latitude;
                            myUser.Longitude = model.Longitude;
                            //myUser.PasswordHash = hashedNewPassword;
                            myUser.ARName = model.ARName;
                            if (Image != null)
                            {
                                //var fileData = helper.ConvertFileToBase64(Image);
                                myUser.Image = FileHelper.UploadFile(Image, "~/Images/Users");
                                //myUser.ImageExtension = fileData.FileExtension;
                            }
                            else
                                myUser.Image = myUser.Image;

                            db.SaveChanges();
                        }
                    }
                    if (Check.Count == 0)
                    {
                        //String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.PasswordHash);
                        myUser.Name = model.Name;
                        myUser.UserName = model.UserName;
                        myUser.Email = model.Email;
                        myUser.Address = model.Address;
                        myUser.Latitude = model.Latitude;
                        myUser.Longitude = model.Longitude;
                        //myUser.PasswordHash = hashedNewPassword;
                        if (Image != null)
                        {
                            //var fileData = helper.ConvertFileToBase64(Image);
                            myUser.Image = FileHelper.UploadFile(Image, "~/Images/Users");
                            //myUser.ImageExtension = fileData.FileExtension;
                        }
                        else
                            myUser.Image = myUser.Image;

                        db.SaveChanges();
                    }
                    return RedirectToAction("EditCustomerProfile");
                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var userID = User.Identity.GetUserId();
                        var user = db.AspNetUsers.FirstOrDefault(c => c.Id == userID);
                        user.Image = helper.ConnvertToImageSrc(user.Image, user.ImageExtension);
                        foreach (var item1 in user.FWYInvoiceOrder)
                        {
                            foreach (var item2 in item1.FWYInvoiceOrderProduct)
                            {
                                if (item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().Count > 0)
                                {
                                    item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().First().Image =
                                        helper.ConnvertToImageSrc(item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().First().Image,
                                        item2.FWYProduct.FWYProductImage.Where(c => c.IsMain == true).ToList().First().ImageExtension);
                                }
                            }
                        }
                        return View(user);
                    }
                    else
                    {
                        return RedirectToAction("Login", "Home", new { returnUrl = "/Customer/GetOrders" });
                    }
                }
            }
            return RedirectToAction("AccessDenied", "Account");
           
        }



        public ActionResult GetOrders()
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    var orders = db.FWYInvoiceOrder.Where(c => c.UserID == userID).ToList();
                    foreach (var item in orders)
                    {
                        if (item.AspNetUsers1.FWYSupplierCooperation.Count > 0)
                        {
                            item.AspNetUsers1.FWYSupplierCooperation.First().Logo = helper.ConnvertToImageSrc(item.AspNetUsers1.FWYSupplierCooperation.First().Logo, item.AspNetUsers1.FWYSupplierCooperation.First().LogoExtension);
                        }
                    }
                    return View(orders);
                }
                else
                {
                    return RedirectToAction("Login", "Home", new { returnUrl = "/Customer/GetOrders" });
                }
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult CancelCustomerOrder(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var UserID = User.Identity.GetUserId();
                var MyUser = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                var InvoiceOrder = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == ID);
                if(InvoiceOrder.UserID == UserID)
                {
                    if (InvoiceOrder.OrderTypeID == (int)OrderType.Pending)
                    {
                        InvoiceOrder.OrderTypeID = (int)OrderType.Canceled;
                        db.SaveChanges();
                        var Notification = new FWYNotification()
                        {
                            Type = (int)OrderType.Canceled,
                            Title = "Order Canceled",
                            Details = MyUser.Name + " canceled a request order",
                            DetailsURL = "/SupplierCorporation/CustomerOrder/" + InvoiceOrder.ID,
                            DateTime = DateTime.Now,
                            SendFrom = UserID,
                            SendTo = InvoiceOrder.SupplierID,
                            ArTitle = "تم الغاء الطلب",
                            ArDetails = "تم الغاء الطلب المرسل من " + MyUser.Name
                        };
                        db.FWYNotification.Add(Notification);
                        db.SaveChanges();
                        return RedirectToAction("GetOrders", "Customer");
                    }
                    else
                    {
                        return RedirectToAction("CustomerOrder", "SupplierCorporation", new { ID = InvoiceOrder.ID });
                    }
                }
               
            }
            return RedirectToAction("AccessDenied", "Account");
            

        }


    
        public ActionResult ConvertToSupplier()
        {
            string Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var userID = User.Identity.GetUserId();
                var myUser = db.AspNetUsers.SingleOrDefault(c => c.Id == userID);
                RegisterViewModel model = new RegisterViewModel();
                model.FullName = myUser.Name;
                model.ArFullName = myUser.ARName;
                model.Phone = myUser.PhoneNumber;
                model.Email = myUser.Email;
                model.Username = myUser.Email;
                model.IsSupplier = true;
                model.ID = myUser.Id;
                model.Address = myUser.Address;
                model.Longitude = (decimal)myUser.Longitude;
                model.Latitude = (decimal)myUser.Latitude;
                model.HowYouKnowUsID = myUser.HowYouKnowUsID;
                var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;
                var Categories = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted==false).ToList();
                model.CategoryList = new SelectList(Categories, "ID", "Name");
                model.HowYouKnowUsList = System.Enum.GetValues(typeof(HowYouKnowUs)).Cast<HowYouKnowUs>().Select(v => new SelectListItem
                {
                    Value = ((int)v).ToString(),
                    Text = v.ToString()
                }).ToList();

                model.CompanyTypeList = System.Enum.GetValues(typeof(CompanyType)).Cast<CompanyType>().Select(v => new SelectListItem
                {
                    Value = ((int)v).ToString(),
                    Text = v.ToString()
                }).ToList();

                if (lang == "ar-EG")
                {
                    model.CategoryList = new SelectList(Categories, "ID", "ARName");
                    model.HowYouKnowUsList = new List<SelectListItem>();
                    SelectListItem item1 = new SelectListItem { Value = "1", Text = "فيسبوك" };
                    SelectListItem item2 = new SelectListItem { Value = "2", Text = "جوجل" };
                    SelectListItem item3 = new SelectListItem { Value = "3", Text = "يوتيوب" };
                    SelectListItem item4 = new SelectListItem { Value = "4", Text = "صديق" };
                    SelectListItem item5 = new SelectListItem { Value = "5", Text = "اخر" };
                    model.HowYouKnowUsList.Add(item1);
                    model.HowYouKnowUsList.Add(item2);
                    model.HowYouKnowUsList.Add(item3);
                    model.HowYouKnowUsList.Add(item4);
                    model.HowYouKnowUsList.Add(item5);

                    model.CompanyTypeList = new List<SelectListItem>();
                    SelectListItem item6 = new SelectListItem { Value = "1", Text = "مصنع" };
                    SelectListItem item7 = new SelectListItem { Value = "2", Text = "مستورد" };
                    SelectListItem item8 = new SelectListItem { Value = "3", Text = "تاجر جملة" };
                    model.CompanyTypeList.Add(item6);
                    model.CompanyTypeList.Add(item7);
                    model.CompanyTypeList.Add(item8);
                }
                var ProfilePictures = db.FWYGallery.ToList();
                foreach (var item in ProfilePictures)
                {
                    item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
                }
                ViewBag.ProfilePictures = ProfilePictures;
                return View(model);
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpPost]
        public ActionResult ConvertToSupplier(RegisterViewModel model, System.Web.HttpPostedFileBase Image, string returnUrl
        , System.Web.HttpPostedFileBase Logo, System.Web.HttpPostedFileBase CommericialRegister,
            System.Web.HttpPostedFileBase NationalCopyID, List<int> CategoryIDs, int CategoryID)
        {
            string Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var myUser = db.AspNetUsers.SingleOrDefault(c => c.Id == model.ID);
                myUser.Name = model.FullName;
                myUser.ARName = model.ArFullName;
                myUser.PhoneNumber = model.Phone;
                myUser.Email = model.Email;
                myUser.UserName = model.Username;
                myUser.IsSupplier = model.IsSupplier;
                myUser.Address = model.Address;
                myUser.Longitude = (decimal)model.Longitude;
                myUser.Latitude = (decimal)model.Latitude;
                myUser.HowYouKnowUsID = myUser.HowYouKnowUsID;
                myUser.RoleID = "2";

                if (Image != null)
                {
                    //var fileData = helper.ConvertFileToBase64(Image);
                    myUser.Image = FileHelper.UploadFile(Image, "/Images/Users");
                    //myUser.ImageExtension = fileData.FileExtension;
                }

                FWYStore store = new FWYStore()
                {
                    Name = "My Store",
                    ARName = "المخزن",
                    SupplierID = myUser.Id
                };
                db.FWYStore.Add(store);

                if (model.IsSupplier == true)
                {
                    model.CompanyModel.SupplierID = myUser.Id;
                    model.CompanyModel.Address = myUser.Address;
                    model.CompanyModel.Phone = myUser.PhoneNumber;

                    if (model.CompanyModel.ArName == "" || model.CompanyModel.ArName == null)
                    {
                        model.CompanyModel.ArName = model.CompanyModel.Name;
                    }
                    if (Logo != null)
                    {
                        var fileData = helper.ConvertFileToBase64(Logo);
                        model.CompanyModel.Logo = fileData.FileBase64;
                        model.CompanyModel.LogoExtension = fileData.FileExtension;
                    }
                    if (CommericialRegister != null)
                    {
                        var fileData = helper.ConvertFileToBase64(CommericialRegister);
                        model.CompanyModel.CommericialRegister = fileData.FileBase64;
                        model.CompanyModel.CommericialRegisterExtension = fileData.FileExtension;
                    }
                    if (NationalCopyID != null)
                    {
                        var fileData = helper.ConvertFileToBase64(NationalCopyID);
                        model.CompanyModel.NationalCopyID = fileData.FileBase64;
                        model.CompanyModel.NationalCopyIDExtension = fileData.FileExtension;
                    }

                    db.FWYSupplierCooperation.Add(model.CompanyModel);

                    db.SaveChanges();

                    if (CategoryIDs != null)
                    {
                        foreach (var item in CategoryIDs)
                        {
                            var SupCategory = new FWYSupplierCooperationCategory
                            {
                                CategoryID = item,
                                CompanyID = model.CompanyModel.ID
                            };
                            db.FWYSupplierCooperationCategory.Add(SupCategory);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var SupCategory = new FWYSupplierCooperationCategory
                        {
                            CategoryID = CategoryID,
                            CompanyID = model.CompanyModel.ID
                        };
                        db.FWYSupplierCooperationCategory.Add(SupCategory);
                        db.SaveChanges();
                    }

                    var Notification = new FWYNotification()
                    {
                        Title = "New Request",
                        Details = myUser.Name + " send a request",
                        DetailsURL = "/SupplierRequest/SupplierCompany/" + model.CompanyModel.ID,
                        DateTime = DateTime.Now,
                        SendFrom = myUser.Id,
                        SendTo = "1cc2ede9-5bc0-41e2-be52-801a844f1819",
                        ArTitle = "طلب جديد",
                        ArDetails = "قام " + myUser.ARName + " بارسال طلب جديد"
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();
                }
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
