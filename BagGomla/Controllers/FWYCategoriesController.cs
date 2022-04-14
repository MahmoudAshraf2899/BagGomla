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
using BagGomla.Helper;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    [Authorize]
    public class FWYCategoriesController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();

        public ActionResult Index()
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var Category = new List<FWYCategory>();
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    Category = db.FWYCategory.Where(c => c.IsDeleted == false && c.CategoryID == null).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
                    //foreach (var item in Category)
                    //{
                    //    item.Image = "~/Images/Categories/" + item.Image;
                    //}
                }
                return View(Category);
            }
            return RedirectToAction("AccessDenied", "Account");      
        }

        public ActionResult HiddenCategories()
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var Category = new List<FWYCategory>();
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    Category = db.FWYCategory.Where(c => c.IsDeleted == true && c.CategoryID == null).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
                    //foreach (var item in Category)
                    //{
                    //    item.Image = "~/Images/Categories/" + item.Image;
                    //}
                }
                return View(Category);
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        [HttpGet]
        //public ActionResult EditCreate(int ID, int CateID)
        //{
        //    var Role = ((int)UserRole.Admin).ToString();
        //    var IsInRole = User.IsInRole(Role);
        //    if (IsInRole)
        //    {
        //        FWYCategory FWYCategory = new FWYCategory();
        //        //FWYCategory.SupplierID = User.Identity.GetUserId();

        //        if (CateID > 0)
        //        {
        //            FWYCategory.CategoryID = CateID;
        //            var parent = db.FWYCategory.SingleOrDefault(c => c.ID == CateID);
        //            if (parent != null)
        //            {
        //                if (parent.CategoryID == null)
        //                {
        //                    if (ID > 0)
        //                    {
        //                        FWYCategory = db.FWYCategory.Find(ID);
        //                        return View("_EditCreate", FWYCategory);
        //                    }
        //                }
        //            }
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    return RedirectToAction("AccessDenied", "Account");

        //}
        //[HttpGet]
        public ActionResult EditCreate(int ID, int CateID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                FWYCategory FWYCategory = new FWYCategory();
                if (CateID > 0)
                    FWYCategory.CategoryID = CateID;
                //FWYCategory.SupplierID = User.Identity.GetUserId();
                if (ID > 0)
                {
                    FWYCategory = db.FWYCategory.Find(ID);
                    return View("_EditCreate", FWYCategory);
                }
                else
                {
                    if(CateID == 0)
                    {
                        return View("_EditCreate", FWYCategory);
                    }
                    else
                    {
                        var parent = db.FWYCategory.SingleOrDefault(c => c.ID == CateID);
                        if (parent != null)
                        {
                            if (parent.CategoryID == null)
                                return View("_EditCreate", FWYCategory);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(FWYCategory FWYCategory , HttpPostedFileBase Image)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if(Image != null)
                    {
                        //var fileData = helper.ConvertFileToBase64(Image);
                        FWYCategory.Image = FileHelper.UploadFile(Image, "/Images/Categories");
                        //FWYCategory.ImageExtension = FWYCategory.ImageExtension; //fileData.FileExtension;
                    }
                    if (FWYCategory.ID > 0)
                    {
                        var categories = db.FWYCategory.Where(c => (c.Name == FWYCategory.Name || c.ARName == FWYCategory.ARName) && c.IsDeleted == false).ToList();
                        if (categories.Count == 1)
                        {
                            var mycate = categories.First();
                            if (mycate.ID == FWYCategory.ID)
                            {
                                mycate.Name = FWYCategory.Name;
                                mycate.ARName = FWYCategory.ARName;
                                mycate.Description = FWYCategory.Description;
                                mycate.ARDescription = FWYCategory.ARDescription;
                                mycate.Number = FWYCategory.Number;
                                mycate.Code = FWYCategory.Code;
                                if (Image != null)
                                {
                                    FWYCategory.Image = FileHelper.UploadFile(Image, "/Images/Categories");
                                    mycate.ImageExtension = FWYCategory.ImageExtension;
                                }
                                db.SaveChanges();
                            }
                            else if (categories.Count > 1)
                            {
                                ModelState.AddModelError("", "اسم هذه الفئة موجود بالفعل");
                                return View("_EditCreate", FWYCategory);
                            }
                        }
                    }
                    else
                    {
                        var categories = db.FWYCategory.Where(c => (c.Name == FWYCategory.Name || c.ARName == FWYCategory.ARName) && c.IsDeleted == false).ToList();
                        if(categories.Count == 0)
                        {
                            db.FWYCategory.Add(FWYCategory);
                            db.SaveChanges();
                        }
                        else
                        {
                            ModelState.AddModelError("", "اسم هذه الفئة موجود بالفعل");
                            return View("_EditCreate", FWYCategory);
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View("_EditCreate", FWYCategory);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult Delete(int Id)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYCategory.SingleOrDefault(a => a.ID == Id);
                List<FWYCategory> myList = new List<FWYCategory>();
                myList.Add(del);
                ProductsController p = new ProductsController();
                p.GetChilds(del, myList);
                if (del.FWYProduct.Where(c => c.IsDeleted == false).ToList().Count > 0 || 
                    myList.Where(c => c.FWYProduct.Where(x => x.IsDeleted == false).ToList().Count > 0).ToList().Count > 0)
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    del.IsDeleted = true;
                    db.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(-1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult HideCat(int Id)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYCategory.SingleOrDefault(a => a.ID == Id);
                del.IsDeleted = true;
                db.SaveChanges();
            }
            return Json(1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CategoryDetails(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var category = db.FWYCategory.SingleOrDefault(c => c.ID == ID);
                //category.Image = "~/Images/Categories/" + category.Image; //helper.ConnvertToImageSrc(category.Image, category.ImageExtension);
                return PartialView("_CategoryDetails", category);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult ReturnDeletedCategory(int id)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYCategory.SingleOrDefault(a => a.ID == id);
                del.IsDeleted = false;
                db.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
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
