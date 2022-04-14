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
using BagGomla.Models;
using System.IO;
using BagGomla.Helper;
using static BagGomla.Models.Enum;
using System.Globalization;

namespace BagGomla.Controllers
{
    [Authorize]
    public class FWYStoresController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();

        public ActionResult Index()
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var stores = new List<FWYStore>();
                if (User.Identity.IsAuthenticated)
                {
                    var userID = User.Identity.GetUserId();
                    stores = db.FWYStore.Where(c => c.SupplierID == userID && c.IsDeleted == false).ToList();
                }
                return View(stores);
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        //    FWYStore fWYStore = db.FWYStore.Find(id);
        //    if (fWYStore == null)
        //        return HttpNotFound();

        //    ViewBag.StoreID = id;
        //    return View(fWYStore);
        //}

        public ActionResult EditCreate(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                StoreModel model = new StoreModel();
                model.fWYStore = new FWYStore();
                if (ID > 0)
                    model.fWYStore = db.FWYStore.Find(ID);

                model.fWYStore.SupplierID = User.Identity.GetUserId();
                return PartialView("_EditCreate", model);
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(StoreModel model)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if (model.fWYStore.ID > 0)
                    {
                        db.Entry(model.fWYStore).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.FWYStore.Add(model.fWYStore);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(model.fWYStore);
            }
            return RedirectToAction("AccessDenied", "Account");

        }


        public JsonResult Delete(int Id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYStore.FirstOrDefault(a => a.ID == Id);
                
                if (del.FWYStoreProduct.Where(c => c.FWYProduct.IsDeleted == false).ToList().Count == 0)
                {
                    del.IsDeleted = true;
                    db.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(2, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }


        /////////////////////////////Section Products/////////////////////////////

        public string GetCodeNumber(int StoreID)
        {
            string CODENumber = "";
            string Prefix = "PRO_";
            string StartNumber = "0001";
            var StoreSup = db.FWYStore.FirstOrDefault(c => c.ID == StoreID);
            if (StoreSup.FWYStoreProduct.Count() == 0)
            {
                CODENumber = Prefix + StartNumber;
            }
            if (StoreSup.FWYStoreProduct.Count() != 0)
            {
                var CodeNumber = "";
                if (StoreSup.FWYStoreProduct.Where(c => c.FWYProduct.Code != null).Last().FWYProduct.Code.Contains("_") == true)
                {
                    CodeNumber = StoreSup.FWYStoreProduct.Where(c => c.FWYProduct.Code != null).Last().FWYProduct.Code;
                    string[] LasCodeNumberChar = CodeNumber.Split('_');
                    int FinalInvoiceNumber = Int16.Parse(LasCodeNumberChar[1]) + 1;
                    CODENumber = LasCodeNumberChar[0] + "_" + FinalInvoiceNumber.ToString().PadLeft(4, '0');
                }
            }
            return CODENumber;
        }

        public ActionResult StoreProducts(int id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                ViewBag.StoreID = id;
                var store = db.FWYStore.SingleOrDefault(c => c.ID == id);
                List<FWYProduct> Products = new List<FWYProduct>();
                foreach (var item in store.FWYStoreProduct)
                {
                    if (Products.Where(c => c.ID == item.ProductID).ToList().Count == 0)
                    {
                        if (item.FWYProduct.IsDeleted == false)
                            Products.Add(item.FWYProduct);
                    }
                }
                return View(Products);
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [HttpGet]
        public ActionResult EditCreateProduct(int ID, int? StoreID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                ViewBag.MainCategory = null;
                var UserID = User.Identity.GetUserId();
                StoreProduct StoreProduct = new StoreProduct();
                var store = new FWYStore();
                if (StoreID != null)
                    store = db.FWYStore.SingleOrDefault(c => c.ID == StoreID);

                else
                    store = db.FWYStore.SingleOrDefault(c => c.SupplierID == UserID);

                if (store == null || store.ID == 0)
                {
                    db.FWYStore.Add(new FWYStore()
                    {
                        Name = "My Store",
                        ARName = "My store",
                        SupplierID = UserID
                    });
                    db.SaveChanges();
                }

                StoreProduct.StoreName = store.Name;
                StoreProduct.Product = new FWYProduct();               
                StoreProduct.Product.IsAvailable = true;
                StoreProduct.Product.IsFeatured = false;
                StoreProduct.StoreID = store.ID;
                StoreProduct.UnitList = new SelectList(db.FWYUnit.Where(c => c.IsDeleted== false).ToList(), "ID", "Name");
                StoreProduct.ProductTypeList = System.Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Select(v => new SelectListItem
                {
                    Value = ((int)v).ToString(),
                    Text = v.ToString()
                }).ToList();
                var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;

                var Categories = new List<FWYCategory>();
                Categories = db.FWYCategory.Where(c => c.IsDeleted == false && c.CategoryID == null).ToList();
                //ProductsController p = new ProductsController();
                //if (store.AspNetUsers.FWYSupplierCooperation.Count > 0)
                //{
                //    var company = store.AspNetUsers.FWYSupplierCooperation.First();
                   
                //    foreach (var i in company.FWYSupplierCooperationCategory)
                //    {
                //            if (i.FWYCategory.IsDeleted == false)
                //            {
                //                Categories.Add(i.FWYCategory);
                //            }
                //    }
                    if (lang == "ar-EG")
                        StoreProduct.CategoryList = new SelectList(Categories, "ID", "ARName");
                    else
                        StoreProduct.CategoryList = new SelectList(Categories, "ID", "Name");

                //}
                var Categories2 = new List<FWYCategory>();

                StoreProduct.CategoryList2 = new SelectList(Categories2, "ID", "Name");

                StoreProduct.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "Name");              
                if (lang == "ar-EG")
                {
                    StoreProduct.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false), "ID", "ARName");
                    StoreProduct.UnitList = new SelectList(db.FWYUnit.Where(c => c.IsDeleted == false).ToList(), "ID", "ARName");
                    var item1 = new SelectListItem { Value = "1", Text = "محلي" };
                    var item2 = new SelectListItem { Value = "2", Text = "مستورد" };
                    StoreProduct.ProductTypeList = new List<SelectListItem>();
                    StoreProduct.ProductTypeList.Add(item1);
                    StoreProduct.ProductTypeList.Add(item2);
                }
                if (ID > 0)
                {
                    StoreProduct.Product = db.FWYProduct.SingleOrDefault(c => c.ID == ID);
                    StoreProduct.CateID = (int)StoreProduct.Product.CategoryID;
                    var subCate = db.FWYCategory.Find(StoreProduct.CateID);
                    if(subCate != null)
                    {
                        if(lang == "ar-EG")
                            StoreProduct.CategoryList2 = new SelectList(db.FWYCategory.Where(c => c.CategoryID == subCate.CategoryID && c.IsDeleted == false), "ID", "ARName");
                        else
                            StoreProduct.CategoryList2 = new SelectList(db.FWYCategory.Where(c => c.CategoryID == subCate.CategoryID && c.IsDeleted == false), "ID", "Name");
                        ViewBag.MainCategory = subCate.CategoryID; 
                    }
                     
                }
                return View(StoreProduct);
            }
            return RedirectToAction("AccessDenied", "Account");
        }


        [HttpPost]
        public ActionResult EditCreateProduct(StoreProduct StoreProduct, List<System.Web.HttpPostedFileBase> Images, List<int> edPPIDs)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {

                StoreProduct.Product.Name = StoreProduct.Product.ARName;
                StoreProduct.Product.CategoryID = StoreProduct.CateID;
                StoreProduct.Product.FinalPrice = StoreProduct.Product.Price;
                
                var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;
                //
                if (StoreProduct.Product.ID == 0)
                {
                    StoreProduct.Product.CreatedDateTime = DateTime.Now;
                    db.FWYProduct.Add(StoreProduct.Product);
                    db.SaveChanges();
                    var sp = new FWYStoreProduct
                    {
                        StoreID = StoreProduct.StoreID,
                        ProductID = StoreProduct.Product.ID,
                        Quantity = StoreProduct.Product.TotalQuantity
                    };
                    db.FWYStoreProduct.Add(sp);
                    db.SaveChanges();
                    var range1 = new FWYProductPriceRange
                    {
                        ProductID = StoreProduct.Product.ID,
                        FromQuantity = 1,
                        ToQuantity = StoreProduct.Product.LessQuantityGomla - 1,
                        Price = StoreProduct.Product.Price
                    };
                    db.FWYProductPriceRange.Add(range1);
                    var range2 = new FWYProductPriceRange
                    {
                        ProductID = StoreProduct.Product.ID,
                        FromQuantity = StoreProduct.Product.LessQuantityGomla,
                        ToQuantity = 100000,
                        Price = StoreProduct.Product.WholesalePrice
                    };
                    db.FWYProductPriceRange.Add(range2);
                    db.SaveChanges();
                }
                else
                {
                    var Product = db.FWYProduct.SingleOrDefault(c => c.ID == StoreProduct.Product.ID);
                    Product.Name = StoreProduct.Product.ARName;
                    Product.ARName = StoreProduct.Product.ARName;
                    Product.Price = StoreProduct.Product.Price;
                    Product.CategoryID = StoreProduct.Product.CategoryID;
                    Product.UnitID = StoreProduct.Product.UnitID;
                    Product.Description = StoreProduct.Product.ARDescription;
                    Product.ARDescription = StoreProduct.Product.ARDescription;
                    Product.IsFeatured = StoreProduct.Product.IsFeatured;
                    Product.FinalPrice = StoreProduct.Product.FinalPrice;
                    Product.IsAvailable = StoreProduct.Product.IsAvailable;
                    Product.LessQuantityGomla = StoreProduct.Product.LessQuantityGomla;
                    Product.CountryID = StoreProduct.Product.CountryID;
                    Product.TypeID = StoreProduct.Product.TypeID;
                    Product.Show = StoreProduct.Product.Show;
                    db.SaveChanges();
                    var ProductRanges = db.FWYProductPriceRange.Where(c => c.ProductID == Product.ID).ToList();
                    if (ProductRanges.Count >= 2)
                    {
                        ProductRanges[0].FromQuantity = 1;
                        ProductRanges[0].ToQuantity = StoreProduct.Product.LessQuantityGomla - 1;
                        ProductRanges[0].Price = StoreProduct.Product.Price;
                        ProductRanges[1].FromQuantity = StoreProduct.Product.LessQuantityGomla;
                        ProductRanges[1].ToQuantity = 100000;
                        ProductRanges[1].Price = StoreProduct.Product.WholesalePrice;
                    }
                    else
                    {
                        if (ProductRanges.Count == 1)
                        {
                            ProductRanges[0].FromQuantity = 1;
                            ProductRanges[0].ToQuantity = StoreProduct.Product.LessQuantityGomla - 1;
                            ProductRanges[0].Price = StoreProduct.Product.Price;
                        }
                        else
                        {
                            var range1 = new FWYProductPriceRange
                            {
                                ProductID = StoreProduct.Product.ID,
                                FromQuantity = 1,
                                ToQuantity = StoreProduct.Product.LessQuantityGomla - 1,
                                Price = StoreProduct.Product.Price
                            };
                            db.FWYProductPriceRange.Add(range1);
                        }

                        var range2 = new FWYProductPriceRange
                        {
                            ProductID = StoreProduct.Product.ID,
                            FromQuantity = StoreProduct.Product.LessQuantityGomla,
                            ToQuantity = 100000,
                            Price = StoreProduct.Product.WholesalePrice
                        };
                        db.FWYProductPriceRange.Add(range2);
                    }

                    db.SaveChanges();
                }


                var x = 0;
                foreach (var item in Images)
                {
                    var pro = db.FWYProduct.SingleOrDefault(c => c.ID == StoreProduct.Product.ID);
                    if (item != null)
                    {
                        var ProductImage = new FWYProductImage()
                        {
                            ProductID = StoreProduct.Product.ID
                        };
                        //var fileData = helper.ConvertFileToBase64(item);
                        ProductImage.Image = FileHelper.UploadFile(item, "/Images/Products");
                        //ProductImage.ImageExtension = fileData.FileExtension;
                        if (x == 0)
                        {
                            ProductImage.IsMain = true;
                            foreach (var img in pro.FWYProductImage)
                            {
                                img.IsMain = false;
                            }
                        }
                        db.FWYProductImage.Add(ProductImage);
                        db.SaveChanges();
                    }
                    x++;
                }



                var user = db.FWYStore.SingleOrDefault(c => c.ID == StoreProduct.StoreID).AspNetUsers;
                var CompanyID = 0;
                if (user.FWYSupplierCooperation.Count > 0)
                {
                    CompanyID = user.FWYSupplierCooperation.First().ID;
                }

                var mainCategory = db.FWYCategory.Find((int)db.FWYCategory.Find(StoreProduct.CateID).CategoryID);
                if(db.FWYSupplierCooperationCategory.Where(c => c.CompanyID == CompanyID && c.CategoryID == mainCategory.ID).Count() == 0)
                {
                    db.FWYSupplierCooperationCategory.Add(new FWYSupplierCooperationCategory()
                    {
                        CompanyID = CompanyID,
                        CategoryID = mainCategory.ID
                    });
                    db.SaveChanges();
                }
                return RedirectToAction("CompanyProducts", "SupplierCorporation", new { ID = CompanyID });
            }
            return RedirectToAction("AccessDenied", "Account");

        }


        [AllowAnonymous]
        public ActionResult GetCatList(int CatID)
        {
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;
            var items = db.FWYCategory.Where(c => c.CategoryID == CatID).ToList();
            if (lang == "ar-EG")
            {
                var list = new SelectList(items, "ID", "ARName");
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var list = new SelectList(items, "ID", "Name");
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DeleteProduct(int id, int StoreID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var product = db.FWYProduct.SingleOrDefault(c => c.ID == id);
                if(product.FWYInvoiceOrderProduct.Where(c => c.FWYInvoiceOrder.OrderTypeID != (int)OrderType.Finished || c.FWYInvoiceOrder.OrderTypeID != (int)OrderType.Rejected).ToList().Count == 0)
                {
                    product.IsDeleted = true;
                    db.SaveChanges();
                    return Json(1, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            return Json(-1, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ProductDetails(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                var model = db.FWYProduct.SingleOrDefault(c => c.ID == ID);
                //foreach (var item in model.FWYProductImage)
                //{
                //    item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
                //}
                return View(model);
            }
            return RedirectToAction("AccessDenied", "Account");

        }


        public ActionResult RemoveProductImage(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                var ProductImage = db.FWYProductImage.SingleOrDefault(c => c.ID == ID);
                int ProductID = (int)ProductImage.ProductID;
                var pro = db.FWYProduct.SingleOrDefault(c => c.ID == ProductID);
                if (pro.FWYProductImage.Count > 1)
                {
                    db.FWYProductImage.Remove(ProductImage);
                    db.SaveChanges();
                    pro.FWYProductImage.First().IsMain = true;
                    db.SaveChanges();
                }
                return RedirectToAction("ProductDetails", new { ID = ProductID });
            }
            return RedirectToAction("AccessDenied", "Account");

        }


        [HttpGet]
        public ActionResult AddNewBrand()
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
                    var item = new FWYBrand() { SupplierID = userID };
                    return PartialView("AddNewBrand", item);
                }
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [HttpPost]
        public ActionResult AddNewBrand(FWYBrand model)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                if (User.Identity.IsAuthenticated)
                {
                    db.FWYBrand.Add(model);
                    db.SaveChanges();
                    var items = db.FWYBrand.Where(c => c.SupplierID == model.SupplierID).ToList();
                    var list = new SelectList(items, "ID", "Name");
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("AccessDenied", "Account");

        }


        [HttpGet]
        public ActionResult AddNewUnit()
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
                    var item = new FWYUnit() { SupplierID = userID };
                    return PartialView("AddNewUnit", item);
                }
                return RedirectToAction("Login", "Account");
            }
            return RedirectToAction("AccessDenied", "Account");

        }

        [HttpPost]
        public ActionResult AddNewUnit(FWYUnit model)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            var Role1 = ((int)UserRole.Admin).ToString();
            var IsInRole1 = User.IsInRole(Role1);
            if (IsInRole || IsInRole1)
            {
                if (User.Identity.IsAuthenticated)
                {
                    if(model.ARName == null || model.ARName == "")
                    {
                        model.ARName = model.Name;
                    }
                    db.FWYUnit.Add(model);
                    db.SaveChanges();
                    var items = db.FWYUnit.Where(c => c.SupplierID == model.SupplierID).ToList();
                    var list = new SelectList(items, "ID", "Name");
                    return Json(list, JsonRequestBehavior.AllowGet);
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
