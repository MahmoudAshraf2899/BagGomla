using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System.IO;
using static BagGomla.Models.Enum;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using BagGomla.Helper;
using PagedList;

namespace BagGomla.Controllers
{
    public class SupplierCorporationController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private TagHelper helper = new TagHelper();

        public SupplierCorporationController()
        {

        }
        public SupplierCorporationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public ActionResult GetProductList(int ID, string SupID)
        {
            var user = db.AspNetUsers.FirstOrDefault(c => c.Id == SupID);
            var ProductList = new List<FWYProduct>();

            foreach (var item in user.FWYStore)
            {
                foreach (var item1 in item.FWYStoreProduct)
                {
                    if (item1.FWYProduct.CategoryID == ID)
                    {
                        if (ProductList.Find(c => c.ID == item1.FWYProduct.ID) == null && item1.FWYProduct.IsDeleted == false)
                        {
                            ProductList.Add(item1.FWYProduct);
                        }
                    }
                }
            }
            return PartialView("_ProductList", ProductList);
        }


        // GET: SupplierCorporation
        public ActionResult Index()
        {
            var fWYSupplierCorporation = db.FWYSupplierCooperation.Include(f => f.AspNetUsers);
            return View(fWYSupplierCorporation.ToList());
        }

        [Authorize]
        public ActionResult EditSupProfile(string id)
        {
            var UserID = User.Identity.GetUserId();
            if (id != null)
            {
                UserID = id;
            }
            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
            var ID = 0;
            if (user.FWYSupplierCooperation.Count > 0)
            {
                ID = user.FWYSupplierCooperation.First().ID;
            }
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                if (user.FWYSupplierCooperation.Count > 0)
                {
                    var companyID = user.FWYSupplierCooperation.First().ID;
                    if (ID == companyID)
                    {
                        FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                        fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
                        fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
                        fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
                        fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);

                        var ProfilePicture = "";
                        if (fWYSupplierCorporation.ProfilePictureID != null)
                        {
                            var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                            ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
                        }
                        ViewBag.ProfilePicture = ProfilePicture;

                        var ProfilePictures = db.FWYGallery.ToList();
                        foreach (var item in ProfilePictures)
                        {
                            item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
                        }
                        ViewBag.ProfilePictures = ProfilePictures;

                        var categories = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).ToList();
                        foreach (var item in fWYSupplierCorporation.FWYSupplierCooperationCategory)
                        {
                            categories.Remove(item.FWYCategory);
                        }
                        ViewBag.CategoryList = categories;

                        ViewBag.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "ArName");
                        if (fWYSupplierCorporation.CityId != null)
                        {
                            var city = db.City.Find(fWYSupplierCorporation.CityId);
                            ViewBag.CityList = new SelectList(db.City.Where(c => c.CountryId == city.CountryId).ToList(), "Id", "ArName");
                        }
                        else
                        {
                            ViewBag.CityList = new SelectList(db.City.ToList(), "Id", "ArName");
                        }

                        return View(fWYSupplierCorporation);
                    }
                }

            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> EditSupProfile(FWYSupplierCooperation model, HttpPostedFileBase Image, int? ImageProfileID, List<int> CategoryIDs)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);

            if (IsInRole || IsInRole1)
            {
                var UserID = model.AspNetUsers.Id;
                var CurrentUser = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                if (CurrentUser.FWYSupplierCooperation.Count > 0)
                {
                    var companyID = CurrentUser.FWYSupplierCooperation.First().ID;
                    if (model.ID == companyID)
                    {
                        if (ModelState.IsValid)
                        {
                            var myUser = db.AspNetUsers.FirstOrDefault(c => c.Id == model.SupplierID);
                            var user = await UserManager.FindByIdAsync(model.SupplierID);
                            var Check = 0;
                            if (model.AspNetUsers.Email != null && model.AspNetUsers.Email != "")
                            {
                                var CheckMail = db.AspNetUsers.Where(c => c.Email == model.AspNetUsers.Email).ToList();
                                Check = CheckMail.Count;
                            }

                            var Sup = db.FWYSupplierCooperation.FirstOrDefault(c => c.ID == model.ID);

                            if (Check == 1)
                            {
                                var cu = db.AspNetUsers.Where(c => c.Email == model.AspNetUsers.Email).ToList().First();
                                if (myUser.Id == cu.Id)
                                {
                                    myUser.Name = model.AspNetUsers.Name;
                                    myUser.Email = model.AspNetUsers.Email;
                                    myUser.ARName = model.AspNetUsers.ARName;
                                    myUser.PhoneNumber = model.Phone;
                                    myUser.Address = model.Address;

                                    Sup.About = model.About;
                                    Sup.Address = model.Address;
                                    Sup.Phone = model.Phone;
                                    Sup.Instagram = model.Instagram;
                                    Sup.Facebook = model.Facebook;
                                    Sup.Linkedin = model.Linkedin;
                                    Sup.WebsiteUrl = model.WebsiteUrl;
                                    Sup.ArName = model.ArName;
                                    Sup.ArAbout = model.ArAbout;
                                    Sup.Name = model.Name;
                                    Sup.CityId = model.CityId;
                                    if (Image != null)
                                    {
                                        var fileData = helper.ConvertFileToBase64(Image);
                                        Sup.Logo = fileData.FileBase64;
                                        Sup.LogoExtension = fileData.FileExtension;
                                    }
                                    else
                                        Sup.Logo = Sup.Logo;

                                    if (ImageProfileID != 0 && ImageProfileID != null)
                                        Sup.ProfilePictureID = ImageProfileID;
                                    else
                                        Sup.ProfilePictureID = Sup.ProfilePictureID;

                                    db.SaveChanges();
                                }
                            }
                            if (Check == 0)
                            {
                                //String hashedNewPassword = UserManager.PasswordHasher.HashPassword(model.PasswordHash);
                                myUser.Name = model.AspNetUsers.Name;
                                myUser.Email = model.AspNetUsers.Email;
                                myUser.ARName = model.AspNetUsers.ARName;
                                myUser.PhoneNumber = model.Phone;
                                myUser.Address = model.Address;

                                Sup.About = model.About;
                                Sup.Address = model.Address;
                                Sup.Phone = model.Phone;
                                Sup.Instagram = model.Instagram;
                                Sup.Facebook = model.Facebook;
                                Sup.Linkedin = model.Linkedin;
                                Sup.WebsiteUrl = model.WebsiteUrl;
                                Sup.ArName = model.ArName;
                                Sup.ArAbout = model.ArAbout;
                                Sup.Name = model.Name;
                                Sup.CityId = model.CityId;
                                if (Image != null)
                                {
                                    var fileData = helper.ConvertFileToBase64(Image);
                                    Sup.Logo = fileData.FileBase64;
                                    Sup.LogoExtension = fileData.FileExtension;
                                }
                                else
                                    Sup.Logo = Sup.Logo;

                                if (ImageProfileID != 0)
                                    Sup.ProfilePictureID = ImageProfileID;
                                else
                                    Sup.ProfilePictureID = Sup.ProfilePictureID;

                                db.SaveChanges();
                            }

                            var ProfilePictures1 = db.FWYGallery.ToList();
                            foreach (var item in ProfilePictures1)
                            {
                                item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
                            }
                            ViewBag.ProfilePictures = ProfilePictures1;


                            if (CategoryIDs != null)
                            {
                                foreach (var item in CategoryIDs)
                                {
                                    FWYSupplierCooperationCategory cate = new FWYSupplierCooperationCategory()
                                    {
                                        CategoryID = item,
                                        CompanyID = model.ID
                                    };
                                    db.FWYSupplierCooperationCategory.Add(cate);
                                    db.SaveChanges();
                                }

                            }
                            return RedirectToAction("EditSupProfile");
                        }
                        model.Logo = helper.ConnvertToImageSrc(model.Logo, model.LogoExtension);
                        model.NationalCopyID = helper.ConnvertToImageSrc(model.NationalCopyID, model.NationalCopyIDExtension);
                        model.CommericialRegister = helper.ConnvertToImageSrc(model.CommericialRegister, model.CommericialRegister);
                        model.AspNetUsers.Image = helper.ConnvertToImageSrc(model.AspNetUsers.Image, model.AspNetUsers.ImageExtension);
                        var ProfilePicture = "";
                        if (model.ProfilePictureID != null)
                        {
                            var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == model.ProfilePictureID);
                            ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
                        }
                        ViewBag.ProfilePicture = ProfilePicture;

                        var categories = db.FWYCategory.Where(c => c.CategoryID == null).ToList();
                        foreach (var item in model.FWYSupplierCooperationCategory)
                        {
                            categories.Remove(item.FWYCategory);
                        }
                        ViewBag.CategoryList = categories;

                        ViewBag.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "ArName");
                        if (model.CityId != null)
                        {
                            var city = db.City.Find(model.CityId);
                            ViewBag.CityList = new SelectList(db.City.Where(c => c.CountryId == city.CountryId).ToList(), "Id", "ArName");
                        }
                        else
                        {
                            ViewBag.CityList = new SelectList(db.City.ToList(), "Id", "ArName");
                        }
                        return View(model);
                    }
                }
            }

            return RedirectToAction("AccessDenied", "Account");

        }


        public ActionResult GetCountryCities(int id)
        {
            var cities = new SelectList(db.City.Where(c => c.CountryId == id).ToList(), "Id", "ArName");
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CompanyHome(int ID)
        {
            FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
            fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
            fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
            fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
            fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);
            var ProfilePicture = "";
            if (fWYSupplierCorporation.ProfilePictureID != null)
            {
                var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
            }
            ViewBag.ProfilePicture = ProfilePicture;
            return View(fWYSupplierCorporation);
        }

        public ActionResult CompanyProfile(int ID)
        {
            FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
            fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
            fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
            fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
            fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);
            var ProfilePicture = "";
            if (fWYSupplierCorporation.ProfilePictureID != null)
            {
                var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
            }
            ViewBag.ProfilePicture = ProfilePicture;
            return View(fWYSupplierCorporation);
        }

        public ActionResult CompanyProducts(int ID)
        {
            FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
            fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
            fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
            fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
            fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);
            var ProfilePicture = "";
            if (fWYSupplierCorporation.ProfilePictureID != null)
            {
                var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
            }
            ViewBag.ProfilePicture = ProfilePicture;
            return View(fWYSupplierCorporation);
        }

        public ActionResult CompanyContact(int ID)
        {
            FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
            fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
            fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
            fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
            fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);
            var ProfilePicture = "";
            if (fWYSupplierCorporation.ProfilePictureID != null)
            {
                var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
            }
            ViewBag.ProfilePicture = ProfilePicture;
            return View(fWYSupplierCorporation);
        }

        [Authorize]
        public ActionResult CompanyVerify(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var UserID = User.Identity.GetUserId();
                var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                if (user.FWYSupplierCooperation.Count > 0)
                {
                    var companyID = user.FWYSupplierCooperation.First().ID;
                    if (ID == companyID)
                    {
                        var fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                        fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
                        fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
                        fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
                        fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);
                        var ProfilePicture = "";
                        if (fWYSupplierCorporation.ProfilePictureID != null)
                        {
                            var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                            ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
                        }
                        ViewBag.ProfilePicture = ProfilePicture;
                        return View(fWYSupplierCorporation);
                    }
                }
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        [HttpPost]
        public ActionResult CompanyVerify(FWYSupplierCooperation model,
            HttpPostedFileBase CommericialRegister, HttpPostedFileBase NationalCopyID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var UserID = User.Identity.GetUserId();
                var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                if (user.FWYSupplierCooperation.Count > 0)
                {
                    var companyID = user.FWYSupplierCooperation.First().ID;
                    if (model.ID == companyID)
                    {
                        if (CommericialRegister != null)
                        {
                            var fileData = helper.ConvertFileToBase64(CommericialRegister);
                            model.CommericialRegister = fileData.FileBase64;
                            model.CommericialRegisterExtension = fileData.FileExtension;
                        }
                        if (NationalCopyID != null)
                        {
                            var fileData = helper.ConvertFileToBase64(NationalCopyID);
                            model.NationalCopyID = fileData.FileBase64;
                            model.NationalCopyIDExtension = fileData.FileExtension;
                        }
                        var item = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == model.ID);
                        item.Phone = model.Phone;
                        item.NationalCopyID = model.NationalCopyID;
                        item.CommericialRegister = model.CommericialRegister;
                        db.SaveChanges();
                        return RedirectToAction("CompanyHome/" + model.ID);
                    }
                }
            }
            return RedirectToAction("AccessDenied", "Account");

        }


        [Authorize]
        public ActionResult CompanyOrders(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var UserID = User.Identity.GetUserId();
                var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                if (user.FWYSupplierCooperation.Count > 0)
                {
                    var companyID = user.FWYSupplierCooperation.First().ID;
                    if (ID == companyID)
                    {
                        FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                        fWYSupplierCorporation.Logo = helper.ConnvertToImageSrc(fWYSupplierCorporation.Logo, fWYSupplierCorporation.LogoExtension);
                        fWYSupplierCorporation.NationalCopyID = helper.ConnvertToImageSrc(fWYSupplierCorporation.NationalCopyID, fWYSupplierCorporation.NationalCopyIDExtension);
                        fWYSupplierCorporation.CommericialRegister = helper.ConnvertToImageSrc(fWYSupplierCorporation.CommericialRegister, fWYSupplierCorporation.CommericialRegister);
                        fWYSupplierCorporation.AspNetUsers.Image = helper.ConnvertToImageSrc(fWYSupplierCorporation.AspNetUsers.Image, fWYSupplierCorporation.AspNetUsers.ImageExtension);
                        if (User.Identity.IsAuthenticated)
                        {
                            if (User.Identity.GetUserId() == fWYSupplierCorporation.SupplierID)
                            {
                                return View(fWYSupplierCorporation);
                            }
                        }
                        var ProfilePicture = "";
                        if (fWYSupplierCorporation.ProfilePictureID != null)
                        {
                            var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == fWYSupplierCorporation.ProfilePictureID);
                            ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
                        }
                        ViewBag.ProfilePicture = ProfilePicture;
                        return RedirectToAction("CompanyHome/" + ID);
                    }
                }

            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        public ActionResult CustomerOrder(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Supplier).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            var Role2 = ((int)UserRole.Admin).ToString();
            var IsInRole2 = User.IsInRole(Role2);
            if (IsInRole || IsInRole1 || IsInRole2)
            {
                var UserID = User.Identity.GetUserId();
                var order = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == ID);

                foreach (var item in order.FWYInvoiceOrderProduct)
                {
                    foreach (var item1 in item.FWYProduct.FWYProductImage)
                    {
                        item1.Image = helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
                    }
                }
                return View(order);


            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        public ActionResult GetInvoiceOrderProductDetails(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Supplier).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            var Role2 = ((int)UserRole.Admin).ToString();
            var IsInRole2 = User.IsInRole(Role2);
            if (IsInRole || IsInRole1 || IsInRole2)
            {
                var UserID = User.Identity.GetUserId();
                var model = db.FWYInvoiceOrderProduct.SingleOrDefault(c => c.ID == ID);
                var order = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == model.InvoiceOrderID);
                foreach (var item in model.FWYProduct.FWYProductImage)
                {
                    item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
                }
                return PartialView("_InvoiceOrderProductDetails", model);
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        public ActionResult AcceptOrder(int OrderID, string ShippingCompany, int TrackNumber, DateTime DeliverDate,
            decimal DeliveryServicePrice, List<int> OrderProductIDs, List<int> StoreProductIDs)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                var UserID = User.Identity.GetUserId();
                var MyUser = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                var order = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == OrderID);
                if (order.SupplierID == UserID || IsInRole1)
                {
                    decimal totalPrice = (decimal)order.TotalPrice;
                    totalPrice += DeliveryServicePrice;

                    order.ShippingCompany = ShippingCompany;
                    order.TrackNumber = TrackNumber;
                    order.DeliveryDate = DeliverDate;
                    order.DeliveryServicePrice = DeliveryServicePrice;
                    order.TotalPrice = totalPrice;
                    order.OrderTypeID = 2;
                    db.SaveChanges();

                    var i = 0;
                    foreach (var item in OrderProductIDs)
                    {
                        var orderProduct = db.FWYInvoiceOrderProduct.SingleOrDefault(c => c.ID == item);
                        orderProduct.StoreProductID = StoreProductIDs[i];
                        db.SaveChanges();
                        i++;
                    }

                    var Notification = new FWYNotification()
                    {
                        Type = (int)OrderType.Accepted,
                        Title = "Order Accepted",
                        Details = MyUser.Name + " accepted your request order",
                        DetailsURL = "/SupplierCorporation/CustomerOrder/" + order.ID,
                        DateTime = DateTime.Now,
                        SendFrom = order.SupplierID,
                        SendTo = order.UserID,
                        ArTitle = "تم الموافقة على طلبك",
                        ArDetails = "قام" + MyUser.ARName + " بالموافقة على طلبك"
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();

                    return RedirectToAction("CustomerOrder/" + OrderID);
                }

            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        public ActionResult ChangeOrderType(int ID, int TypeID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                var UserID = User.Identity.GetUserId();
                var MyUser = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);

                var type = db.FWYOrderType.SingleOrDefault(c => c.ID == TypeID);

                var order = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == ID);
                if (order.SupplierID == UserID || IsInRole1)
                {
                    order.OrderTypeID = TypeID;

                    if (TypeID == (int)OrderType.Finished)
                    {
                        foreach (var item in order.FWYInvoiceOrderProduct)
                        {
                            var FWYStoreProduct = db.FWYStoreProduct.SingleOrDefault(c => c.ID == item.StoreProductID);
                            FWYStoreProduct.SalesNum += (int)item.Quantity;
                            item.FWYProduct.SalesNum += (int)item.Quantity;
                        }

                    }

                    db.SaveChanges();

                    var Notification = new FWYNotification()
                    {
                        Type = TypeID,
                        Title = "Order " + type.Name,
                        Details = MyUser.Name + " " + type.Name + " your request order",
                        DetailsURL = "/SupplierCorporation/CustomerOrder/" + order.ID,
                        DateTime = DateTime.Now,
                        SendFrom = order.SupplierID,
                        SendTo = order.UserID,
                        ArTitle = "طلبك " + type.ARName,
                        ArDetails = "طلبك " + type.ARName
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();

                    return RedirectToAction("CustomerOrder/" + ID);
                }

            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        public ActionResult RemoveOrderProduct(int id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var orderProduct = db.FWYInvoiceOrderProduct.SingleOrDefault(c => c.ID == id);
                var ProductID = orderProduct.ProductID;
                int orderID = (int)orderProduct.InvoiceOrderID;
                var order = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == orderID);
                var UserID = User.Identity.GetUserId();
                if (order.SupplierID == UserID)
                {
                    if (orderProduct.FWYInvoiceOrder.OrderTypeID == (int)OrderType.Pending || orderProduct.FWYInvoiceOrder.OrderTypeID == (int)OrderType.Accepted)
                    {
                        db.FWYInvoiceOrderProduct.Remove(orderProduct);
                        db.SaveChanges();
                        var product = db.FWYProduct.SingleOrDefault(c => c.ID == ProductID);
                        if (product.FWYInvoiceOrderProduct.Count > 0)
                        {
                            decimal subTotal = 0;
                            foreach (var item in order.FWYInvoiceOrderProduct)
                            {
                                subTotal += (decimal)(item.UnitPrice * item.Quantity);
                            }
                            order.SubTotal = subTotal;
                            order.TotalPrice = subTotal - (subTotal * order.Discount);
                            if (order.DeliveryServicePrice != null)
                            {
                                order.TotalPrice += order.DeliveryServicePrice;
                            }
                            db.SaveChanges();
                            return Json(0, JsonRequestBehavior.AllowGet);
                            //return RedirectToAction("CustomerOrder", new { ID = orderID });
                        }
                        else
                        {
                            db.FWYInvoiceOrder.Remove(order);
                            db.SaveChanges();
                            return Json(1, JsonRequestBehavior.AllowGet);
                            //return RedirectToAction("GetOrders", "Customer");
                        }
                    }
                    else
                    {
                        return Json(-1, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("CustomerOrder", new { ID = orderID });
                    }
                }

            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        [HttpPost]
        public ActionResult EditOrderProduct(int orderProductID, int orderProductQuantity, int? orderProductColor, int? orderProductSize)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var orderProduct = db.FWYInvoiceOrderProduct.SingleOrDefault(c => c.ID == orderProductID);
                var UserID = User.Identity.GetUserId();
                if (orderProduct.FWYInvoiceOrder.UserID == UserID)
                {
                    orderProduct.Quantity = orderProductQuantity;
                    if (orderProductColor != null)
                    {
                        orderProduct.ProductColorID = orderProductColor;
                    }
                    if (orderProductSize != null)
                    {
                        orderProduct.ProductSizeID = orderProductSize;
                    }
                    db.SaveChanges();
                    var order = db.FWYInvoiceOrder.SingleOrDefault(c => c.ID == orderProduct.InvoiceOrderID);
                    decimal subTotal = 0;
                    foreach (var item in order.FWYInvoiceOrderProduct)
                    {
                        subTotal += (decimal)(item.UnitPrice * item.Quantity);
                    }
                    order.SubTotal = subTotal;
                    order.TotalPrice = subTotal - (subTotal * order.Discount);
                    if (order.DeliveryServicePrice != null)
                    {
                        order.TotalPrice += order.DeliveryServicePrice;
                    }
                    db.SaveChanges();
                    return RedirectToAction("CustomerOrder", new { ID = order.ID });
                }

            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [Authorize]
        public ActionResult LookUpTable(int page = 1, string searchStr = "")
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var Role2 = ((int)UserRole.Admin).ToString();

            var IsInRole = User.IsInRole(Role);
            var IsInRole2 = User.IsInRole(Role2);
            var LookUpList = new List<FWYLockupTable>();
            if(IsInRole || IsInRole2)
            {
                if (IsInRole2)
                {
                    LookUpList = db.FWYLockupTable.ToList();
                }
                if (IsInRole)
                {
                    var UserID = User.Identity.GetUserId();
                    var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                    var Company = new FWYSupplierCooperation();

                    if (user.FWYSupplierCooperation.Count > 0)
                    {
                        Company = user.FWYSupplierCooperation.First();
                        foreach (var item in Company.FWYSupplierCooperationCategory)
                        {
                            ProductsController p = new ProductsController();
                            List<FWYCategory> categories = new List<FWYCategory>();
                            p.GetChilds(item.FWYCategory, categories);
                            foreach (var item1 in categories)
                            {
                                foreach (var item2 in item1.FWYLockupTable)
                                {
                                    if (LookUpList.Where(c => c.ID == item2.ID).ToList().Count == 0)
                                    {
                                        LookUpList.Add(item2);
                                    }

                                }
                            }
                        }
                    }
                }

                if (searchStr != "" && searchStr != null)
                {
                    searchStr = searchStr.ToLower();
                    LookUpList = LookUpList.Where(c => (c.UserName != null && c.UserName.ToLower().Contains(searchStr))
                    || (c.SearchString != null && c.SearchString.ToLower().Contains(searchStr))
                    || (c.FWYCategory != null && c.FWYCategory.Name.ToLower().Contains(searchStr))
                    || (c.Longitude != null && c.Longitude.ToString().Contains(searchStr))
                    || (c.Latitude != null && c.Latitude.ToString().Contains(searchStr))
                    || (c.Location != null && c.Location.ToLower().Contains(searchStr))
                    || (c.SearchDateTime != null && c.SearchDateTime.ToString().Contains(searchStr))).ToList();
                }
                LookUpList = LookUpList.OrderByDescending(c => c.ID).ToList();
                ViewBag.searchStr = searchStr;
                return View(LookUpList.ToPagedList(page, 10));
            }
            
            return RedirectToAction("AccessDenied", "Account");
        }


        public ActionResult RemoveSupplierCategory(int id)
        {
            var result = true;
            var supmainCate = db.FWYSupplierCooperationCategory.SingleOrDefault(c => c.ID == id);
            if (supmainCate != null && supmainCate.ID > 0)
            {
                var supCategories = db.FWYCategory.Where(c => c.CategoryID == supmainCate.CategoryID).ToList();
                if (supCategories != null && supCategories.Count > 0)
                {
                    foreach (var item in supCategories)
                    {
                        if (item.FWYProduct != null)
                        {
                            if (item.FWYProduct.Where(c => c.IsDeleted == false && c.FWYStoreProduct != null && c.FWYStoreProduct.Count() > 0).Count() > 0)
                            {
                                if (item.FWYProduct.Where(c => c.IsDeleted == false && c.FWYStoreProduct.Where(x => x.FWYStore != null).Count() > 0).Count() > 0)
                                {
                                    if (item.FWYProduct.Where(c =>  c.IsDeleted == false && c.FWYStoreProduct.Where(x => x.FWYStore.SupplierID == supmainCate.FWYSupplierCooperation.SupplierID).Count() > 0).Count() > 0)
                                    {
                                        result = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    result = true;
                }
            }

            if (result == true)
            {
                db.FWYSupplierCooperationCategory.Remove(supmainCate);
                db.SaveChanges();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
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
