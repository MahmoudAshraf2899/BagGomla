using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BagGomla.Models;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using BagGomla.Helper;
using System.Net;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;
using BagGomla.ViewModel;
using System.Data.Entity;

namespace BagGomla.Controllers
{
    //[Authorize]
    public class HomeController : Lan
    {
        DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();
        public ActionResult ChangeLanguage(string lang, string returnUrl)
        {
            new LanguageMang().SetLanguage(lang);
            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (!returnUrl.Equals(""))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Home
        public void SetLocation()
        {
            string ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = Request.ServerVariables["REMOTE_ADDR"];
            }

            string APIKey = "8f75a1bc80f0cf4bd37a5d1ebdf95cae82746ffdf55fc5b3ef150958838a7e5e";
            string url = string.Format("http://api.ipinfodb.com/v3/ip-city/?key={0}&ip={1}&format=json", APIKey, ipAddress);
            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                LocationModel location = new JavaScriptSerializer().Deserialize<LocationModel>(json);
                List<LocationModel> locations = new List<LocationModel>();
                locations.Add(location);
            }

        }
        public ActionResult OLDIndex()
        {
            HomeModel model = new HomeModel();
            model.CategoryList = db.FWYCategory.Where(c => c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            //model.ProductList = db.FWYProduct.Where(c => c.IsDeleted == false && c.IsAvailable == true).ToList();
            ViewBag.FWYProductReview = db.FWYProductReview.GroupBy(t => t.ProductID).Select(t => new { ID = t.Key, Value = t.Sum(u => u.Rate) }).ToList().OrderByDescending(c => c.Value);
            model.ProductReviewList = new List<int>();
            model.NotificationList = db.FWYNotification.Where(c => c.IsDeleted == false && c.IsManual == true).ToList();
            foreach (var item1 in model.NotificationList)
            {
                item1.Image = helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
            }
            foreach (var item in ViewBag.FWYProductReview)
            {
                model.ProductReviewList.Add(item.ID);
            }
            //model.BlogList = db.FWYBlog.ToList();
            //model.ProductList = model.ProductList.Where(c => c.IsDeleted == false && c.IsAvailable == true).ToList();
            foreach (var item in model.CategoryList)
            {
                item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            }

            foreach (var item in model.ProductList)
            {
                foreach (var item1 in item.FWYProductImage)
                {
                    item1.Image = helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
                }
            }
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            HomeModel model = new HomeModel();
            model.CategoryList = db.FWYCategory.Where(c => c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            /*  model.ProductList = db.FWYProduct.Where(c => c.IsDeleted == false && c.IsAvailable == true).ToList();*/
            /*  ViewBag.FWYProductReview = db.FWYProductReview.GroupBy(t => t.ProductID).Select(t => new { ID = t.Key, Value = t.Sum(u => u.Rate) }).ToList().OrderByDescending(c => c.Value);*/
            /*   model.ProductReviewList = new List<int>();*/
            model.NotificationList = db.FWYNotification.Where(c => c.IsDeleted == false && c.IsManual == true).ToList();
            foreach (var item1 in model.NotificationList)
            {
                item1.Image = helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
            }
            /*foreach (var item in ViewBag.FWYProductReview)
            {
                model.ProductReviewList.Add(item.ID);
            }*/
            //model.BlogList = db.FWYBlog.ToList();
            /*  var Products = db.FWYProduct.Where(c => c.IsDeleted == false && c.IsAvailable == true && c.Show == true).ToList();*/

            /*  model.ProductList = Products.OrderBy(x => Guid.NewGuid()).Take(Products.Count).ToList();*/
            /*  foreach (var item in model.CategoryList)
              {
                  item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
              }*/

            /*  foreach (var item in model.ProductList)
              {
                  foreach (var item1 in item.FWYProductImage)
                  {
                      item1.Image = helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
                  }
              }*/
            return View(model);
        }
        public ActionResult GetData(int pageIndex, int pageSize)
        {
            var query = db.FWYProduct
                .Where(c => c.IsDeleted == false && c.IsAvailable == true && c.Show == true)
                .Include(x => x.FWYProductPriceRange).Include(x => x.FWYProductImage)
                .Include(x => x.FWYStoreProduct)
                .OrderByDescending(x => x.ID).Skip(pageIndex * pageSize).Take(pageSize).ToList();
            List<ProductVM> vm = new List<ProductVM>();
            var CurrentUserID = User.Identity.GetUserId();
            foreach (var item in query)
            {
                var product = new ProductVM()
                {
                    ID = item.ID,
                    LessQuantityGomla = item.LessQuantityGomla,
                    Name = item.Name
                };

                if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName.Contains("ar"))
                {
                    product.Name = item.ARName;
                }

                if (item.FWYStoreProduct != null && item.FWYStoreProduct.Count() > 0)
                {
                    if (item.FWYStoreProduct.First().FWYStore != null)
                    {
                        if (item.FWYStoreProduct.First().FWYStore.AspNetUsers != null)
                        {
                            var user = item.FWYStoreProduct.First().FWYStore.AspNetUsers;
                            var verified = " <i class='fa fa-check-circle text-secondary'></i>";
                            if (user.FWYSupplierCooperation != null && user.FWYSupplierCooperation.Count() > 0)
                            {
                                if (user.FWYSupplierCooperation.First().IsVerified)
                                {
                                    verified = " <i class='fa fa-check-circle text-success'></i>";
                                }
                            }
                            product.SupplierName = user.ARName + verified;
                            if (System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName.Contains("ar") && user.ARName != null)
                            {
                                product.SupplierName = user.ARName + verified;
                            }

                        }
                    }
                }

                if (item.FWYProductImage.Where(c => c.IsMain == true).ToList().Count > 0)
                {
                    product.Image = item.FWYProductImage.Where(c => c.IsMain == true).ToList().First(c => c.IsMain == true).Image;
                }
                if (item.FWYProductPriceRange.Count > 0)
                {
                    product.Min = String.Format("{0:0.00}", item.FWYProductPriceRange.Min(c => c.Price));
                    product.Max = String.Format("{0:0.00}", item.FWYProductPriceRange.Max(c => c.Price));
                }
                else
                {
                    product.Min = String.Format("{0:0.00}", item.FinalPrice);
                    product.Max = String.Format("{0:0.00}", item.FinalPrice);
                }

                if (CurrentUserID != null)
                {
                    if (db.FWYWishList.Where(c => c.UserID == CurrentUserID && c.ProductID == item.ID).Count() > 0)
                        product.isFavorite = true;
                }
                vm.Add(product);
            }
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCategoryList()
        {
            var MainCategories = new List<List<FWYCategory>>();
            var MainCategories1 = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            foreach (var item in MainCategories1)
            {
                var myList = new List<FWYCategory>();
                myList.Add(item);
                MainCategories.Add(myList);
            }
            return View(MainCategories);
        }

        public ActionResult GetBlog(int ID)
        {
            BlogModel model = new BlogModel();
            model.Blog = db.FWYBlog.SingleOrDefault(c => c.ID == ID);
            model.CategoryList = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            model.FeaturedProductList = db.FWYProduct.Where(c => c.IsFeatured == true).ToList();
            return View(model);
        }

        public ActionResult SeeNotify(int ID)
        {
            FWYNotification noti = db.FWYNotification.SingleOrDefault(c => c.Id == ID);
            if (noti.IsRead == false)
            {
                noti.IsRead = true;
                db.SaveChanges();
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSubCategories(int id)
        {
            var main = db.FWYCategory.SingleOrDefault(c => c.ID == id);
            var subs = main.FWYCategory1.Where(c => c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            return PartialView("_SubCategories", subs);
        }

        public ActionResult GetMainCategories(int id)
        {
            var main = db.FWYCategory.SingleOrDefault(c => c.ID == id);
            var AllMains = new List<FWYCategory>();
            if (main.CategoryID != null)
            {
                foreach (var item in main.FWYCategory2.FWYCategory1.Where(c => c.IsDeleted == false).ToList())
                {
                    AllMains.Add(item);
                }
            }
            else
            {
                AllMains = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            }
            return PartialView("_SubCategories", AllMains);
        }

        [Authorize]
        [HttpGet]
        public JsonResult IsUniqueEmail(string Email, string UserID)
        {
            var Result = 0;
            //var UserID = User.Identity.GetUserId();
            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
            if (Email != "" && Email != null)
            {
                var users = db.AspNetUsers.Where(c => c.Email.ToLower() == Email.ToLower()).ToList();
                var checker = 0;
                foreach (var item in users)
                {
                    if (item.Id != user.Id)
                    {
                        checker++;
                    }
                }
                if (checker > 0)
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

        [Authorize]
        [HttpGet]
        public ActionResult IsUniquePhone(string Phone, string UserID)
        {
            var Result = 0;
            //var UserID = User.Identity.GetUserId();
            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
            if (Phone != "" || Phone != null)
            {
                var users = db.AspNetUsers.Where(c => c.PhoneNumber.ToLower() == Phone.ToLower()).ToList();
                var checker = 0;
                foreach (var item in users)
                {
                    if (item.Id != user.Id)
                    {
                        checker++;
                    }
                }
                if (checker > 0)
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
    }
}