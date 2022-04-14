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
using BagGomla.Helper;
using System.Data.Entity;
using static BagGomla.Models.Enum;
using System.IO;
using PagedList;
using BagGomla.Attributes;

namespace BagGomla.Controllers
{
    [CustomAuthorize(Role = UserRole.Admin)]
    public class AdminController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        private ApplicationUserManager _userManager;
        private TagHelper helper = new TagHelper();

        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        public ActionResult AdminProfile()
        {
            string userId = User.Identity.GetUserId();
            AspNetUsers user = db.AspNetUsers.FirstOrDefault(c => c.Id == userId);
            string userID = User.Identity.GetUserId();
            user.Image = helper.ConnvertToImageSrc(user.Image, user.ImageExtension);
            return View(user);
        }

        [HttpPost]
        public async Task<ActionResult> AdminProfile(AspNetUsers model, System.Web.HttpPostedFileBase Image)
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
                        myUser.Name = model.Name;
                        myUser.Email = model.Email;
                        myUser.Address = model.Address;
                        myUser.Latitude = model.Latitude;
                        myUser.Longitude = model.Longitude;
                        if (Image != null)
                        {
                            //var fileData = helper.ConvertFileToBase64(Image);
                            myUser.Image = FileHelper.UploadFile(Image, "/Images/Users");
                            //myUser.ImageExtension = fileData.FileExtension;
                        }
                        else
                            myUser.Image = myUser.Image;

                        db.SaveChanges();
                    }
                }
                if (Check.Count == 0)
                {
                    myUser.Name = model.Name;
                    myUser.UserName = model.UserName;
                    myUser.Email = model.Email;
                    myUser.Address = model.Address;
                    myUser.Latitude = model.Latitude;
                    myUser.Longitude = model.Longitude;
                    if (Image != null)
                    {
                        //var fileData = helper.ConvertFileToBase64(Image);
                        myUser.Image = FileHelper.UploadFile(Image, "/Images/Users");
                        //myUser.ImageExtension = fileData.FileExtension;
                    }
                    else
                        myUser.Image = myUser.Image;

                    db.SaveChanges();
                }
            }
            return RedirectToAction("AdminProfile");
        }


        public ActionResult AboutIndex()
        {
            var about = db.FWYAbout.Take(5).ToList();
            foreach (var item in about)
            {
                item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            }
            return View(about);
        }

        public ActionResult About(int ID)
        {
            FWYAbout FWYAbout = new FWYAbout();

            if (ID > 0)
            {
                FWYAbout.Image = helper.ConnvertToImageSrc(FWYAbout.Image, FWYAbout.ImageExtension);
            }
            FWYAbout = db.FWYAbout.Find(ID);

            return View(FWYAbout);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult About(FWYAbout FWYAbout, System.Web.HttpPostedFileBase Images)
        {
            if (ModelState.IsValid)
            {
                if (FWYAbout.ID > 0)
                {
                    if (Images != null)
                    {
                        var fileData = helper.ConvertFileToBase64(Images);
                        FWYAbout.Image = fileData.FileBase64;
                        FWYAbout.ImageExtension = fileData.FileExtension;
                    }

                    db.Entry(FWYAbout).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {

                    if (Images != null)
                    {
                        var fileData = helper.ConvertFileToBase64(Images);
                        FWYAbout.Image = fileData.FileBase64;
                        FWYAbout.ImageExtension = fileData.FileExtension;
                    }
                    db.FWYAbout.Add(FWYAbout);
                    db.SaveChanges();
                }
                return RedirectToAction("AboutIndex");
            }
            return View(FWYAbout);
        }


        public ActionResult contect()
        {
            FWYContect FWYContect = new FWYContect();

            if (db.FWYContect.Count() != 0)
                FWYContect = db.FWYContect.FirstOrDefault();

            return View(FWYContect);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult contect(FWYContect FWYContect)
        {
            if (FWYContect.ID > 0)
            {
                db.Entry(FWYContect).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.FWYContect.Add(FWYContect);
                db.SaveChanges();
            }
            return RedirectToAction("contect", "Admin");
        }


        [HttpGet]
        public ActionResult Complaints(int page = 1, string searchStr = "")
        {
            var model = db.FWYComplaintOrsuggestion.OrderByDescending(c => c.ID).ToList();
            if(searchStr != null && searchStr != "")
            {
                searchStr = searchStr.ToLower();
                model = model.Where(c => (c.EmailFrom != null && c.EmailFrom.ToLower().Contains(searchStr))
                || (c.ComplaintOrsuggestion != null && c.ComplaintOrsuggestion.ToLower().Contains(searchStr))).ToList();
            }
            ViewBag.searchStr = searchStr;
            return View(model.ToPagedList(page, 20));
        }
        [HttpGet]
        public ActionResult Gallery()
        {
            var images = db.FWYGallery.OrderByDescending(c => c.ID).ToList();
            foreach (var item in images)
            {
                item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            }
            return View(images);
        }
        [HttpPost]
        public ActionResult Gallery(List<System.Web.HttpPostedFileBase> Images)
        {
            foreach (var item in Images)
            {
                if (item != null)
                {
                    FWYGallery gallery = new FWYGallery();
                    var fileData = helper.ConvertFileToBase64(item);
                    gallery.Image = fileData.FileBase64;
                    gallery.ImageExtension = fileData.FileExtension;
                    gallery.ImageName = Path.GetFileName(item.FileName);
                    db.FWYGallery.Add(gallery);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Gallery", "Admin");
        }
        public ActionResult DeleteGallery(int id)
        {
            var image = db.FWYGallery.SingleOrDefault(c => c.ID == id);
            db.FWYGallery.Remove(image);
            db.SaveChanges();
            return RedirectToAction("Gallery", "Admin");
        }
        [Authorize]
        public ActionResult Dashboard()
        {
            AdminDashboard model = new AdminDashboard();
            //suppliers or sellers
            var sql = " ";
            sql += " select * from FWYSupplierCooperation";
            sql += " where SupplierID in";
            sql += " (select distinct(AspNetUsers.id) from aspnetusers";
            sql += " join fwystore on aspnetusers.id = fwystore.supplierid";
            sql += " join FWYStoreProduct on fwystore.id = fwystoreproduct.storeid";
            sql += " join fwyproduct on fwystoreproduct.productid = fwyproduct.id";
            sql += " where fwyproduct.IsDeleted = 0";
            sql += " and aspnetusers.IsDeleted =0";
            sql += " )and isdeleted = 0";
            model.SellerList = db.FWYSupplierCooperation.SqlQuery(sql).ToList();//.ToList();
            //customers
            sql = "";
            sql += " select * from aspnetusers";
            sql += " where id not in ";
            sql += " (select distinct(AspNetUsers.id) from aspnetusers";
            sql += " join fwystore on aspnetusers.id = fwystore.supplierid";
            sql += " join FWYStoreProduct on fwystore.id = fwystoreproduct.storeid";
            sql += " join fwyproduct on fwystoreproduct.productid = fwyproduct.id";
            sql += " where fwyproduct.IsDeleted = 0)";
            sql += " and isdeleted = 0";
            sql += " and roleid <> '1'";
            model.CustomerList = db.AspNetUsers.SqlQuery(sql).ToList();//Where(c => c.RoleID == ((int)UserRole.Customer).ToString() && c.IsDeleted == false).ToList();
            model.ProductList = db.FWYProduct.Where(c => c.IsDeleted == false).ToList();
            model.InvoiceOrderList = db.FWYInvoiceOrder.Where(c => c.IsDeleted == false).ToList();
            return View(model);
        }
        public ActionResult Customers(int page = 1, int userState = 1,string searchStr = "")
        {
            //userState --> 1 not deleted, 2 deleted , 3 all
            ViewBag.userState = userState;
            var model = new List<AspNetUsers>();
            var sql = "select * from aspnetusers ";
            sql += " where id not in ";
            sql += " (select distinct(AspNetUsers.id) from aspnetusers";
            sql += " join fwystore on aspnetusers.id = fwystore.supplierid";
            sql += " join FWYStoreProduct on fwystore.id = fwystoreproduct.storeid";
            sql += " join fwyproduct on fwystoreproduct.productid = fwyproduct.id";
            sql += " where fwyproduct.IsDeleted = 0)";

            if (userState == 1)
            {
                sql += " and isdeleted = 0";
                //model = db.AspNetUsers.Where(c => c.IsDeleted == false &&  c.FWYStore != null && c.FWYStore.Count() > 0 
                //&& c.FWYStore.First().FWYStoreProduct.Where(x => x.FWYProduct != null && x.FWYProduct.IsDeleted == false).Count() == 0).ToList();
            }

            if (userState == 2)
            {
                sql += " and isdeleted = 1";
                //model = db.AspNetUsers.Where(c => c.IsDeleted == true && c.FWYStore != null && c.FWYStore.Count() > 0
                //&& c.FWYStore.First().FWYStoreProduct.Where(x => x.FWYProduct != null && x.FWYProduct.IsDeleted == false).Count() == 0).ToList();
            }

            //if (userState == 3)
            //{
            //    //model = db.AspNetUsers.Where(c => c.FWYStore != null && c.FWYStore.Count() > 0 
            //    //&& c.FWYStore.First().FWYStoreProduct.Where(x => x.FWYProduct != null && x.FWYProduct.IsDeleted == false).Count() == 0).ToList();
            //}
            sql += " and roleid <> '1'";
            model = db.AspNetUsers.SqlQuery(sql).ToList();
            
            if(searchStr != null && searchStr != "")
            {
                searchStr = searchStr.ToLower();
                model = model.Where(c => (c.ARName != null && c.ARName.ToLower().Contains(searchStr))
                || (c.Name != null && c.Name.ToLower().Contains(searchStr))
                || (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(searchStr))
                || (c.CreatedDateTime != null && c.CreatedDateTime.ToString().Contains(searchStr))).ToList();
            }

            model = model.OrderByDescending(c => c.Id).OrderByDescending(c => c.CreatedDateTime).ToList();
            ViewBag.searchStr = searchStr;
            return View(model.ToPagedList(page, 20));
        }

        public ActionResult Sellers(int page = 1, int filterSellerType = 3, string searchStr = "")
        {
            var model = new List<FWYSupplierCooperation>();
            ViewBag.filterSellerType = filterSellerType;
            var sql = " ";
            sql += " select * from FWYSupplierCooperation";
            sql += " where SupplierID in";
            sql += " (select distinct(AspNetUsers.id) from aspnetusers";
            sql += " join fwystore on aspnetusers.id = fwystore.supplierid";
            sql += " join FWYStoreProduct on fwystore.id = fwystoreproduct.storeid";
            sql += " join fwyproduct on fwystoreproduct.productid = fwyproduct.id";
            sql += " where fwyproduct.IsDeleted = 0";
            
            if (filterSellerType == 1)
            {
                sql += " )";
                //model = db.FWYSupplierCooperation.ToList();
                ViewBag.SellerType = "الكل";
            }
            if (filterSellerType == 2)
            {
                sql += " and aspnetusers.IsDeleted = 1";
                sql += " )and isdeleted = 1";
                //model = db.FWYSupplierCooperation.Where(c => c.AspNetUsers.IsDeleted == true).ToList();
                ViewBag.SellerType = "المحذوفين";
            }

            if (filterSellerType == 3)
            {
                sql += " and aspnetusers.IsDeleted =0";
                sql += " )and isdeleted = 0";
                //model = db.FWYSupplierCooperation.Where(c => c.AspNetUsers.IsDeleted == false).ToList();
                ViewBag.SellerType = "الغير محذوفين";
            }
            int pageSize = 10;
            model = db.FWYSupplierCooperation.SqlQuery(sql).ToList();
           

            if (searchStr != null && searchStr != "")
            {
                searchStr = searchStr.ToLower();
                model = model.Where(c => (c.AspNetUsers.Name != null && c.AspNetUsers.Name.ToLower().Contains(searchStr))
                || (c.Name!= null && c.Name.ToLower().Contains(searchStr))
                || (c.AspNetUsers.CreatedDateTime != null && c.AspNetUsers.CreatedDateTime.ToString().Contains(searchStr))).ToList();
            }
            model = model.OrderByDescending(c => c.ID).OrderByDescending(c => c.AspNetUsers.CreatedDateTime).ToList();
            ViewBag.searchStr = searchStr;

            return View(model.ToPagedList(page, pageSize));
        }

        public ActionResult Products(bool? IsFeatured, string SellerID, bool? IsShow, int page = 1,string searchStr = "")
        {
            ViewBag.IsFeatured = IsFeatured;
            ViewBag.SellerID = SellerID;
            ViewBag.IsShow = IsShow;
            var model = new List<FWYProduct>();

            if (IsFeatured != null)
            {
                if (IsFeatured == true)
                {
                    model = db.FWYProduct.Where(c => c.IsDeleted == false && c.IsFeatured == true).ToList();
                }
            }

            if (SellerID != null && SellerID != "")
            {
                model = db.FWYProduct.Where(c => c.IsDeleted == false && c.FWYStoreProduct.Where(x => x.FWYStore.SupplierID == SellerID).Count() > 0).ToList();
                //var model1 = new List<FWYProduct>();
                //foreach (var item in model)
                //{
                //    if (item.FWYStoreProduct.Count > 0)
                //    {
                //        if (item.FWYStoreProduct.First().FWYStore.SupplierID == SellerID)
                //        {
                //            model1.Add(item);
                //        }
                //    }
                //}
                //model = new List<FWYProduct>();
                //model = model1;
            }
            if (IsShow != null)
            {
                model = db.FWYProduct.Where(c => c.IsDeleted == false && c.Show == true).ToList();
            }

            if (IsFeatured == null && IsShow == null && (SellerID == null || SellerID == ""))
            {
                model = db.FWYProduct.Where(c => c.IsDeleted == false).ToList();
            }

            model = model.OrderByDescending(c => c.ID).ToList();

            if(searchStr != null && searchStr != "")
            {
                searchStr = searchStr.ToLower();
                model = model.Where(c => (c.ARName != null && c.ARName.ToLower().Contains(searchStr))
                || (c.FinalPrice != null && c.FinalPrice.ToString().Contains(searchStr))
                || (c.TotalQuantity != null && c.TotalQuantity.ToString().Contains(searchStr))
                || (c.CreatedDateTime != null && c.CreatedDateTime.ToString().Contains(searchStr))
                || c.SalesNum.ToString().Contains(searchStr)
                || (c.FWYCategory != null && c.FWYCategory.ARName.ToLower().Contains(searchStr))).ToList();
            }
            ViewBag.searchStr = searchStr;

            return View(model.ToPagedList(page, 20));
        }

        public ActionResult Orders(int? OrderType, string SellerID, string CustomerID, int? page)
        {
            var model = db.FWYInvoiceOrder.ToList();
            if (OrderType != null)
            {

                model = model.Where(c => c.OrderTypeID == OrderType).ToList();
            }

            if (SellerID != null)
            {
                model = model.Where(c => c.SupplierID == SellerID).ToList();
            }

            if (CustomerID != null)
            {
                model = model.Where(c => c.UserID == CustomerID).ToList();
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            model = model.OrderByDescending(c => c.ID).ToList();
            return View(model.ToPagedList(pageNumber, pageSize));
        }


        public ActionResult AcceptProduct(int ID, int TypeID)
        {
            var pro = db.FWYProduct.FirstOrDefault(c => c.ID == ID);
            if (TypeID == 0)
            {
                pro.IsAvailable = false;
                db.SaveChanges();
                var Notification = new FWYNotification()
                {
                    Title = "Request Rejected",
                    Details = "Request Rejected",
                    DetailsURL = "/Products/Index",
                    DateTime = DateTime.Now,
                    SendFrom = User.Identity.GetUserId(),
                    SendTo = pro.FWYStoreProduct.FirstOrDefault().FWYStore.SupplierID,
                    ArTitle = "تم رفض طلبك",
                    ArDetails = "تم رفض طلبك",
                };
                db.FWYNotification.Add(Notification);
                db.SaveChanges();
            }

            else
            {
                pro.IsAvailable = true;
                db.SaveChanges();
                var Notification = new FWYNotification()
                {
                    Title = "Request Accepted",
                    Details = "Request Accepted",
                    DetailsURL = "/Products/Index",
                    DateTime = DateTime.Now,
                    SendFrom = User.Identity.GetUserId(),
                    SendTo = pro.FWYStoreProduct.FirstOrDefault().FWYStore.SupplierID,
                    ArTitle = "تم قبول طلبك",
                    ArDetails = "تم قبول طلبك",
                };
                db.FWYNotification.Add(Notification);
                db.SaveChanges();
            }
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UntrustedSupplier(string SellerID, int ID)
        {
            var User = db.AspNetUsers.FirstOrDefault(c => c.Id == SellerID);
            if (ID == 0)
                User.FWYSupplierCooperation.First().IsVerified = false;

            else
                User.FWYSupplierCooperation.First().IsVerified = true;

            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        public ActionResult BlockUser(string ID)
        {
            var User = db.AspNetUsers.FirstOrDefault(c => c.Id == ID);
            User.IsDeleted = true;
            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UnBlockUser(string ID)
        {
            var User = db.AspNetUsers.FirstOrDefault(c => c.Id == ID);
            User.IsDeleted = false;
            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProduct(int ID)
        {
            var pro = db.FWYProduct.FirstOrDefault(c => c.ID == ID);
            pro.IsDeleted = true;
            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInHomePage(int ID)
        {
            var PRO = db.FWYProduct.FirstOrDefault(c => c.ID == ID);
            PRO.Show = true;
            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemoveFromHomePage(int ID)
        {
            var PRO = db.FWYProduct.FirstOrDefault(c => c.ID == ID);
            PRO.Show = false;
            db.SaveChanges();
            return Json("1", JsonRequestBehavior.AllowGet);
        }


        #region Country
        [HttpGet]
        public ActionResult Countries()
        {
            var Countries = db.FWYCountry.Where(c => c.IsDeleted == false).OrderByDescending(c => c.ID).ToList();
            return View(Countries);
        }

        [HttpGet]
        public ActionResult EditCreateCountry(int ID)
        {
            var Country = new FWYCountry();
            if (ID > 0)
                Country = db.FWYCountry.Find(ID);

            return View(Country);
        }

        [HttpPost]
        public ActionResult EditCreateCountry(FWYCountry Country)
        {
            if (ModelState.IsValid)
            {
                if (Country.Name == "" || Country.Name == null)
                    Country.Name = Country.ArName;

                if (Country.ID > 0)
                {
                    var item = db.FWYCountry.SingleOrDefault(c => c.ID == Country.ID);
                    item.Name = Country.Name;
                    item.ArName = Country.ArName;
                }
                else
                    db.FWYCountry.Add(Country);

                db.SaveChanges();
            }
            return RedirectToAction("Countries");
        }

        [HttpGet]
        public ActionResult DeleteCountry(int id)
        {
            ConfirmDeleteModel model = new ConfirmDeleteModel()
            {
                ControllerName = "Admin",
                ActionName = "DeleteCountry",
                IntID = id
            };
            return PartialView("_DeleteView", model);
        }

        [HttpPost]
        [ActionName("DeleteCountry")]
        public ActionResult ConfirmDeleteCountry(int ID)
        {
            var Del = db.FWYCountry.Find(ID);

            if (Del.FWYProduct.Where(c => c.IsDeleted == false).Count() == 0 && (Del.Cities == null || Del.Cities.Count() == 0))
            {
                db.FWYCountry.Remove(Del);
                db.SaveChanges();
                return helper.SweetAlert("تم", "تم حذف البلد بنجاح", (int)SweetAlertType.success, "/Admin/Countries");
            }
            else if (Del.FWYProduct.Where(c => c.IsDeleted == true).Count() > 0 && (Del.Cities == null || Del.Cities.Count() == 0))
            {
                Del.IsDeleted = true;
                db.SaveChanges();
                return helper.SweetAlert("تم", "تم حذف البلد بنجاح", (int)SweetAlertType.success, "/Admin/Countries");
            }
            else
            {
                return helper.SweetAlert("لا يمكن حذف هذه الدولة", "يوجد منتجات او مدن مربوطة  بها", (int)SweetAlertType.error, "/Admin/Countries");
            }

        }
        #endregion


        public ActionResult ConverImages()
        {

            var users = db.AspNetUsers.Where(c => c.Image != null && !c.Image.Contains(".jpg"));
            if (users != null && users.Count() > 0)
            {
                foreach (var item in users)
                {
                    item.Image = item.Image = FileHelper.ConvertFromBase64ToJpg(item.Image, "/Images/Users");
                }
                db.SaveChanges();
            }

            var categories = db.FWYCategory.Where(c => c.Image != null && !c.Image.Contains(".jpg"));
            if (categories != null && categories.Count() > 0)
            {
                foreach (var item in categories)
                {
                    item.Image = FileHelper.ConvertFromBase64ToJpg(item.Image, "/Images/Categories/10-12-2021");
                }
                db.SaveChanges();
            }

            var products = db.FWYProductImage.Where(c => c.Image != null && !c.Image.Contains(".jpg"));
            if (products != null && products.Count() > 0)
            {
                foreach (var item in products)
                {
                    item.Image = FileHelper.ConvertFromBase64ToJpg(item.Image, "/Images/Products/10-12-2021");
                }
                db.SaveChanges();
            }

            var ads = db.FWYAds.Where(c => c.Image != null && !c.Image.Contains(".jpg"));
            if (ads != null && ads.Count() > 0)
            {
                foreach (var item in ads)
                {
                    item.Image = FileHelper.ConvertFromBase64ToJpg(item.Image, "/Images/SliderAds");
                }
                db.SaveChanges();
            }

            return View(nameof(ConverImages), "");
        }

    }
}