using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using BagGomla.Models;
using static BagGomla.Models.Enum;
using BagGomla.Helper;
using System.Collections.Specialized;
using Microsoft.AspNet.Identity;
using System.Device.Location;
using PagedList;
using BagGomla.ViewModel;

namespace BagGomla.Controllers
{
    public class ProductsController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper Helper = new TagHelper();

        //////////////////Filter Product//////////////////////////
        
        private List<FWYProduct> FilterProductsResult(List<FWYProduct> lst, int? SortBy, int? Price , string SupplierID)
        {
            if(SortBy != null)
            {
                if (SortBy == 2)
                {
                    lst = lst.OrderByDescending(c => c.SalesNum).ToList();
                }
                else if (SortBy == 3)
                {
                    lst = lst.OrderBy(c => c.FinalPrice).ToList();
                }
                else if (SortBy == 4)
                {
                    lst = lst.OrderByDescending(c => c.FinalPrice).ToList();
                }
            }
            
            decimal FromPrice = 0;
            decimal ToPrice = 0;
            if (Price != null && Price > 1)
            {
                if (Price == 2)
                {
                    ToPrice = 50;
                }
                else if (Price == 3)
                {
                    FromPrice = 50;
                    ToPrice = 100;
                }
                else if (Price == 4)
                {
                    FromPrice = 100;
                    ToPrice = 150;
                }
                else
                {
                    FromPrice = 200;
                    ToPrice = db.FWYProduct.Max(c => (int)c.FinalPrice);
                }
            }
            else
            {
                FromPrice = lst.Min(c => (int)c.FinalPrice);
                ToPrice = lst.Max(c => (int)c.FinalPrice);
            }

            lst = lst.Where(c => c.FinalPrice >= FromPrice && c.FinalPrice <= ToPrice).ToList();


            if (SupplierID != null && SupplierID != "0")
            {
                lst = lst.Where(c => c.FWYStoreProduct != null && c.FWYStoreProduct.Count>0 && c.FWYStoreProduct.First().FWYStore.SupplierID == SupplierID).ToList();
            }
            return lst;
        }

        public ActionResult FilterProducts(int SortBy, int Price, int? Tag, string SupplierID)
        {
            ViewBag.SortBy = SortBy;
            ViewBag.Price = Price;
            ViewBag.Tag = Tag;
            ViewBag.SupplierID = SupplierID;
            ProductModel model = new ProductModel();
            model.ProList = new Dictionary<int, FWYProduct>();
            model.Dis = new Dictionary<int, double>();
            model.Suppliers = db.AspNetUsers.Where(c => c.RoleID == "2").ToList();
            if (SortBy == 1)
            {
                model.ProductList = db.FWYProduct.ToList();
            }
            else if (SortBy == 2)
            {
                model.ProductList = db.FWYProduct.OrderByDescending(c => c.SalesNum).ToList();
            }
            else if (SortBy == 3)
            {
                model.ProductList = db.FWYProduct.OrderBy(c => c.FinalPrice).ToList();
            }
            else if (SortBy == 4)
            {
                model.ProductList = db.FWYProduct.OrderByDescending(c => c.FinalPrice).ToList();
            }
            else
            {
                model.ProductList = db.FWYProduct.ToList();
            }
            decimal FromPrice = 0;
            decimal ToPrice = 0;
            if (Price > 1)
            {
                if (Price == 2)
                {
                    ToPrice = 50;
                }
                else if (Price == 3)
                {
                    FromPrice = 50;
                    ToPrice = 100;
                }
                else if (Price == 4)
                {
                    FromPrice = 100;
                    ToPrice = 150;
                }
                else
                {
                    FromPrice = 200;
                    ToPrice = db.FWYProduct.Max(c => (int)c.FinalPrice);
                }
            }
            else
            {
                FromPrice = db.FWYProduct.Min(c => (int)c.FinalPrice);
                ToPrice = db.FWYProduct.Max(c => (int)c.FinalPrice);
            }

            model.ProductList = model.ProductList.Where(c => c.FinalPrice >= FromPrice && c.FinalPrice <= ToPrice).ToList();

            if (Tag != null && Tag > 0)
            {
                model.ProductList = model.ProductList.Where(c => c.CategoryID == Tag).ToList();
                model.CategoryID = Tag;

            }
            if (SupplierID != "0")
            {
                model.ProductList = model.ProductList.Where(c => c.FWYStoreProduct.First().FWYStore.SupplierID == SupplierID).ToList();
            }

            model.ProductList = model.ProductList.Where(c => c.IsDeleted == false).ToList();

            model.ProductList1 = model.ProductList.ToPagedList(1, 20);

            float pagesF = (float)model.ProductList.Count / 20;
            int pagesI = model.ProductList.Count / 20;
            int pages = pagesI;
            if (pagesF > (float)pagesI)
            {
                pages += 1;
            }
            model.PageCount = pages;
            model.PageNumber = 1;

            return PartialView("_ProductList", model);
        }

        public void GetChilds(FWYCategory f, List<FWYCategory> myList)
        {
            if (f != null)
            {
                var Childs = db.FWYCategory.Where(c => c.CategoryID == f.ID && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();

                if (Childs.Count == 0)
                {
                    if (myList.Count > 0)
                    {
                        if (myList.First().ID != f.ID)
                        {
                            if (GetParent(f) != null)
                            {
                                GetChilds(GetParent(f), myList);
                            }
                        }
                    }
                }
                else
                {
                    var x = 0;
                    foreach (var item in Childs)
                    {
                        var i = myList.Find(c => c.ID == item.ID);
                        if (i != null)
                        {
                            x++;
                        }
                    }
                    if (x == Childs.Count)
                    {
                        if (myList.First().ID != f.ID)
                        {
                            GetChilds(GetParent(f), myList);
                        }
                    }
                    else
                    {
                        foreach (var item in Childs)
                        {
                            var check = myList.Find(c => c.ID == item.ID);
                            if (check == null)
                            {
                                myList.Add(item);
                                GetChilds(item, myList);
                            }
                        }
                    }
                }
            }
        }

        public FWYCategory GetParent(FWYCategory child)
        {
            var parent = child.FWYCategory2;
            return parent;
        }

        public ActionResult GetSubCategories(int CategoryID)
        {
            ProductModel model = new ProductModel();
            model.CategoryList = new List<FWYCategory>();
            model.ProList = new Dictionary<int, FWYProduct>();
            model.Dis = new Dictionary<int, double>();
            model.Suppliers = db.AspNetUsers.Where(c => c.RoleID == "2").ToList();
            var Category = db.FWYCategory.SingleOrDefault(c => c.ID == CategoryID);
            //var CategoryName = Category.Name;
            //var CategoryArName = Category.ARName;
            //var Categories = db.FWYCategory.Where(c => c.Name == CategoryName || c.ARName == CategoryArName && c.IsDeleted == false).ToList();
            foreach (var item in Category.FWYCategory1)
            {
                if (model.CategoryList.Where(c => c.ID == item.ID).ToList().Count == 0)
                {
                    model.CategoryList.Add(item);
                }
            }

            //model.CategoryList = Category.FWYCategory1.ToList();
            return PartialView("_CategoryList", model);
        }


        ////////////////////////////////////////////////////////////////////

        //////////////////Product Quick View//////////////////////////
        public ActionResult FilterProductsByCategory(int ID)
        {
            ProductModel model = new ProductModel();
            model.ProductList = new List<FWYProduct>();
            model.ProList = new Dictionary<int, FWYProduct>();
            model.Dis = new Dictionary<int, double>();
            model.Suppliers = db.AspNetUsers.Where(c => c.RoleID == "2").ToList();
            if (ID != 0)
            {
                var Category = db.FWYCategory.SingleOrDefault(c => c.ID == ID);
                //var CategoryList = db.FWYCategory.Where(c => c.Name == Category.Name || c.ARName == Category.ARName && c.IsDeleted == false).ToList();

                List<FWYCategory> myList = new List<FWYCategory>();
                myList.Add(Category);
                if (myList.Count > 0)
                {
                    GetChilds(Category, myList);
                }
                foreach (var item in myList)
                {
                    var CategoryProducts = new List<FWYProduct>();
                    CategoryProducts = db.FWYProduct.Where(c => c.CategoryID == item.ID || c.CategoryID == ID).ToList();
                    foreach (var i in CategoryProducts)
                    {
                        var prod = model.ProductList.Find(c => c.ID == i.ID);
                        if (prod == null)
                        {
                            model.ProductList.Add(i);
                        }
                    }
                }

            }
            else
            {
                model.ProductList = db.FWYProduct.ToList();
            }
            model.ProductList = model.ProductList.Where(c => c.IsDeleted == false).ToList();

            //foreach (var item in model.ProductList)
            //{
            //    foreach (var item1 in item.FWYProductImage)
            //    {
            //        item1.Image = "~/Images/Products/" + item1.Image;//Helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
            //    }
            //}

            model.ProductList1 = model.ProductList.ToPagedList(1, 20);

            return PartialView("_ProductList", model);
        }
        ////////////////////////////////////////////////////////////////////

        //////////////////Product Quick View//////////////////////////
        public ActionResult ProductQuickView(int ID)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = db.FWYProduct.SingleOrDefault(c => c.ID == ID);
            model.ProductColorList = new List<FWYColor>();
            model.ProductSizeList = new List<FWYSize>();
            foreach (var item in model.Product.FWYStoreProduct)
            {
                if (model.ProductSizeList.Where(c => c.ID == item.SizeID).ToList().Count == 0)
                {
                    if (item.FWYSize != null)
                    {
                        model.ProductSizeList.Add(item.FWYSize);
                    }
                }
            }
            foreach (var item in model.Product.FWYStoreProduct)
            {
                if (model.ProductColorList.Where(c => c.ID == item.ColorID).ToList().Count == 0)
                {
                    if (item.FWYColor != null)
                    {
                        model.ProductColorList.Add(item.FWYColor);
                    }
                }
            }


            return PartialView("_ProductQuickView", model);
        }
        ////////////////////////////////////////////////////////////////////

        //////////////////for cookies cart and fav//////////////////////////
        public bool AddFavorite(int productId, int quantity)
        {
            var CurrentUserID = User.Identity.GetUserId();
            if (CurrentUserID != null)
            {
                if (db.FWYWishList.Where(c => c.UserID == CurrentUserID && c.ProductID == productId).Count() == 0)
                {
                    var wish = new FWYWishList()
                    {
                        UserID = CurrentUserID,
                        ProductID = productId,
                        IsDeleted = false,
                        DateIn = DateTime.Now
                    };
                    db.FWYWishList.Add(wish);
                    db.SaveChanges();
                }
                return true;
            }
            return false;
            //If the cart cookie doesn't exist, create it.
            //if (Request.Cookies["Fav"] == null)
            //{
            //    Response.Cookies.Add(new HttpCookie("Fav"));
            //}

            //Helper method here.
            //var values = Helper.GenerateCookiesCollection(Request.Cookies["Fav"], productId, quantity);
            //Response.Cookies["Fav"].Values.Add(values);

        }

        public void AddCart(int ProductID, int quantity, int SizeID, int colorID)
        {
            //If the cart cookie doesn't exist, create it.
            if (Request.Cookies["Cart"] == null)
            {
                Response.Cookies.Add(new HttpCookie("Cart"));
            }

            //Helper method here.
            var values = Helper.CreateCookies(Request.Cookies["Cart"], ProductID, SizeID, colorID, quantity);
            Response.Cookies["Cart"].Values.Add(values);
        }

        public bool RemoveFavorite(int ProductID)
        {
            var CurrentUserID = User.Identity.GetUserId();
            if (CurrentUserID != null)
            {
                var fav = db.FWYWishList.FirstOrDefault(c => c.UserID == CurrentUserID && c.ProductID == ProductID);
                if (fav != null)
                {
                    db.FWYWishList.Remove(fav);
                    db.SaveChanges();
                }
                return true;
            }
            return false;
            //var values = Helper.RemoveCookies(Request.Cookies["Fav"], ProductID);
            //Response.Cookies["Fav"].Values.Add(values);
        }
        public void RemoveCart(int ProductID, int SizeID, int ColorID)
        {
            string CookVal = ProductID + "-" + SizeID + "-" + ColorID;
            var values = Helper.RemoveCartCookies(Request.Cookies["Cart"], CookVal);
            Response.Cookies["Cart"].Values.Add(values);
            //Helper.RemoveCookie("Cart", CookVal,null);
        }
        public void RemoveCart2()
        {
            Response.Cookies["Cart"].Expires = DateTime.Now.AddDays(-1);
        }


        ////////////////////////////////////////////////////////////////////



        ///////////////////////////////////ShoppingCart//////////////////////
        public ActionResult ShoppingCart()
        {
            List<string> ShoppingCartCollection = TagHelper.GetCookiesCollection(Request.Cookies["Cart"]).ToList();
            var ProductCollection = db.FWYProduct.Where(c => c.IsDeleted == false).ToList();
            //List<FWYProduct> ProductList = new List<FWYProduct>();
            var ProductList = new List<ShoppingCartStruct>();

            foreach (var item in ShoppingCartCollection)
            {
                var SplitShopCart = item.Split('-');

                foreach (var item2 in ProductCollection)
                {
                    if (SplitShopCart[0] == (item2.ID).ToString())
                    {
                        if (SplitShopCart.Length > 0)
                        {
                            if (!SplitShopCart[1].Equals("") && !SplitShopCart[2].Equals(""))
                            {
                                int sizeid = int.Parse(SplitShopCart[1]);
                                int colorid = int.Parse(SplitShopCart[2]);
                                var Colors = db.FWYColor.FirstOrDefault(c => c.ID == colorid);
                                var Sizes = db.FWYSize.FirstOrDefault(c => c.ID == sizeid);

                                ProductList.Add(new ShoppingCartStruct(item2, sizeid, colorid, Colors, Sizes));
                            }
                            else
                                ProductList.Add(new ShoppingCartStruct(item2, 0, 0, null, null));
                        }
                    }
                }
            }
            //foreach (var item in ProductList)
            //{
            //    foreach (var item1 in item.Product.FWYProductImage)
            //    {
            //        item1.Image = "~/Images/Products/" + item1.Image;//Helper.ConnvertToImageSrc(item1.Image, item1.ImageExtension);
            //    }
            //}
            return View(ProductList);
        }


        public ActionResult DisForCouponCode(string CouponCode, string ProductArray)
        {
            var Coupons = db.FWYCoupon.Where(c => c.CouponCode == CouponCode).ToList();
            if (Coupons.Count > 0)
            {
                var myarr = ProductArray.Split('_');
                var Coupon = Coupons.First();
                var ProductList = new List<FWYProduct>();
                decimal total = 0;
                foreach (var item in myarr)
                {
                    var ID = Int32.Parse(item);
                    if (ID != 0)
                    {
                        var p = db.FWYProduct.SingleOrDefault(c => c.ID == ID);
                        ProductList.Add(p);
                        if (p.FWYStoreProduct.Count > 0)
                        {
                            var proSupplier = p.FWYStoreProduct.First().FWYStore.SupplierID;
                            var couponSupplier = Coupon.SupplierID;
                            if (proSupplier == couponSupplier)
                            {
                                p.FinalPrice = p.FinalPrice - (p.FinalPrice * Coupon.Discount / 100);
                            }
                            total += (decimal)p.FinalPrice;
                        }

                    }

                }
                return Json(total, JsonRequestBehavior.AllowGet);
            }
            return Json(0, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateOrder(DateTime dateTimeValue, List<int> ProductIDs, List<int> SupIDS, List<int> ProSizeList, List<int> ProColorList, List<string> checkboxProduct,
        List<decimal> Price, List<int> Quantity, List<decimal> ProductTotalPrice, decimal TotalPrice, string CouponCode, decimal FinalTotalPrice)
        {
            var UserID = User.Identity.GetUserId();
            var MyUser = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
            List<string> SupplierIDs = new List<string>();
            List<FWYProduct> ProductList = new List<FWYProduct>();
            foreach (var item in ProductIDs)
            {
                if (item > 0)
                {
                    var product = db.FWYProduct.SingleOrDefault(c => c.ID == item);
                    ProductList.Add(product);
                    if (product.FWYStoreProduct.Count > 0)
                    {
                        var supID = product.FWYStoreProduct.First().FWYStore.SupplierID;
                        if (SupplierIDs.Where(c => c == supID).ToList().Count == 0)
                        {
                            SupplierIDs.Add(supID);
                        }
                    }
                }
            }
            var InvoiceOrderID = 0;
            foreach (var item in SupplierIDs)
            {
                FWYInvoiceOrder InvoiceOrder = new FWYInvoiceOrder()
                {
                    UserID = UserID,
                    OrderTypeID = (int)(OrderType.Pending),
                    SupplierID = item,
                    //SubTotal = TotalPrice,
                    //TotalPrice = FinalTotalPrice,                   
                    //OrderDateTime = DateTime.Parse(dateTimeValue),
                    //OrderDateTime = DateTime.UtcNow,
                    OrderDateTime = dateTimeValue,
                    CouponID = null,
                    Discount = 0
                };
                db.FWYInvoiceOrder.Add(InvoiceOrder);
                db.SaveChanges();
                InvoiceOrderID = InvoiceOrder.ID;
                var Coupon = new FWYCoupon();
                if (!CouponCode.Equals("0"))
                {
                    Coupon = db.FWYCoupon.FirstOrDefault(c => c.CouponCode == CouponCode);
                }

                var i = 0;
                decimal subTotal = 0;

                foreach (var item1 in ProductList)
                {
                    if (item1.FWYStoreProduct.Count > 0)
                    {
                        if (item1.FWYStoreProduct.First().FWYStore.SupplierID == item)
                        {
                            var price = Price[i];
                            foreach (var pp in item1.FWYProductPriceRange)
                            {
                                if (Quantity[i] >= pp.FromQuantity && Quantity[i] < pp.ToQuantity)
                                {
                                    price = (decimal)pp.Price;
                                }
                            }
                            if (Quantity[i] > item1.FWYProductPriceRange.Max(c => c.ToQuantity))
                            {
                                price = (decimal)item1.FWYProductPriceRange.Min(c => c.Price);
                            }
                            FWYInvoiceOrderProduct InvoiceOrderProduct = new FWYInvoiceOrderProduct()
                            {
                                InvoiceOrderID = InvoiceOrderID,
                                ProductID = item1.ID,
                                UnitPrice = price,
                                Quantity = Quantity[i],
                                TotalPrice = ProductTotalPrice[i],
                                ProductSizeID = ProSizeList[i],
                                ProductColorID = ProColorList[i],
                            };
                            if (ProSizeList[i] == 0)
                                InvoiceOrderProduct.ProductSizeID = null;
                            if (ProColorList[i] == 0)
                                InvoiceOrderProduct.ProductColorID = null;

                            db.FWYInvoiceOrderProduct.Add(InvoiceOrderProduct);
                            if (Coupon != null)
                            {
                                if (Coupon.SupplierID == item)
                                {
                                    InvoiceOrder.Discount = Coupon.Discount;
                                }
                            }
                            db.SaveChanges();

                            subTotal += ((decimal)InvoiceOrderProduct.UnitPrice * (decimal)InvoiceOrderProduct.Quantity);
                            //RemoveCart(ProductIDs[i], ProSizeList[i], ProColorList[i]);
                        }
                    }
                    i++;
                }
                Response.Cookies["Cart"].Expires = DateTime.UtcNow.AddDays(-1);
                decimal totalPrice = subTotal - (subTotal * (decimal)(InvoiceOrder.Discount / 100));
                InvoiceOrder.SubTotal = subTotal;
                InvoiceOrder.TotalPrice = totalPrice;
                db.SaveChanges();

                var Notification = new FWYNotification()
                {
                    Type = (int)OrderType.Pending,
                    Title = "New Order",
                    Details = MyUser.Name + " made a new request order",
                    DetailsURL = "/SupplierCorporation/CustomerOrder/" + InvoiceOrder.ID,
                    DateTime = DateTime.Now,
                    SendFrom = InvoiceOrder.UserID,
                    SendTo = InvoiceOrder.SupplierID,
                    ArTitle = "طلبية جديدة",
                    ArDetails = "قام " + MyUser.Name + "بعمل طلب لبعض المنتجات"
                };
                db.FWYNotification.Add(Notification);
                db.SaveChanges();
            }
            // Add New Product&& Product Detalis 

            return RedirectToAction("GetOrders", "Customer");
        }

        /////////////////////////////////////////////////////////////////////

        ///////////////////////////////////Add product review//////////////////////////////////
        [Authorize]
        [HttpPost]
        public ActionResult AddReview(int ProductID, int rating, string review)
        {
            var Product = db.FWYProduct.SingleOrDefault(c => c.ID == ProductID);
            bool check = false;
            foreach (var item in Product.FWYInvoiceOrderProduct)
            {
                string CurrentUserID = Session["UserID"].ToString();
                if (item.FWYInvoiceOrder.UserID == CurrentUserID && item.FWYInvoiceOrder.OrderTypeID == 3)
                {
                    check = true;
                }
            }
            if (check == true)
            {
                FWYProductReview ProductReview = new FWYProductReview()
                {
                    UserID = "34eb24ce-1226-409a-aca3-91c45025c4e5",
                    ProductID = ProductID,
                    Rate = rating,
                    Review = review
                };
                db.FWYProductReview.Add(ProductReview);
                db.SaveChanges();
                return Json(1, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        /////////////////////////////////////////////////////////////////////

        //////////////////////////////////Discount Page///////////////////////////////////
        public ActionResult Discounts()
        {
            HomeModel model = new HomeModel();
            model.ProductList = db.FWYProduct.Where(c => c.IsDeleted == false).OrderByDescending(c => c.ID).ToList().ToPagedList(1,20);
            ViewBag.FWYProductReview = db.FWYProductReview.GroupBy(t => t.ProductID).Select(t => new { ID = t.Key, Value = t.Sum(u => u.Rate) }).ToList().OrderByDescending(c => c.Value);
            model.ProductReviewList = new List<int>();
            foreach (var item in ViewBag.FWYProductReview)
            {
                model.ProductReviewList.Add(item.ID);
            }
            model.BlogList = db.FWYBlog.ToList();
            //foreach (var item1 in model.ProductList)
            //{
            //    foreach (var item in item1.FWYProductImage)
            //    {
            //        item.Image = "~/Images/Products/" + item.Image;//Helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            //    }
            //}
            return View(model);
        }
        /////////////////////////////////////////////////////////////////////////////////

        // GET: Products
        //public ActionResult Index(string ProductName, int? CategoryID, int? TypeID, decimal? longitude, decimal? latitude, string location, int? pageNumber = 1)
        //{

        //    ProductModel model = new ProductModel();
        //    model.CategoryList = new List<FWYCategory>();
        //    model.CategoryList = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).ToList();

        //    if (CategoryID != null && CategoryID != 0)
        //    {
        //        model.CategoryList = db.FWYCategory.Where(c => c.ID == CategoryID && c.IsDeleted == false).ToList();
        //    }
        //    model.ProductList = new List<FWYProduct>();
        //    model.Dis = new Dictionary<int, double>();
        //    model.ProList = new Dictionary<int, FWYProduct>();
        //    model.Suppliers = db.AspNetUsers.Where(c => c.RoleID == "2").ToList();
        //    if (TypeID != null)
        //    {
        //        model.ProductList = db.FWYProduct
        //            .Where(c => c.IsDeleted == false &&  c.TypeID == TypeID).ToList();
        //        //foreach (var item2 in model.ProductList)
        //        //{
        //        //    var Category = db.FWYCategory.SingleOrDefault(c => c.ID == item2.CategoryID);
        //        //    model.CategoryList.Add(Category);
        //        //    if (model.CategoryList.Count > 0)
        //        //    {
        //        //        GetChilds(Category, model.CategoryList);
        //        //    }
        //        //}
        //    }

        //    if (ProductName != null)
        //    {
        //        var SearchString = ProductName;
        //        int? SearchResult = null;
        //        string UserID = null;
        //        string UserName = "Guest";
        //        if (User.Identity.IsAuthenticated)
        //        {
        //            UserID = User.Identity.GetUserId();
        //            var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
        //            UserName = user.Name;
        //        }
        //        var Pro = db.FWYProduct.ToList();
        //        var Cat = db.FWYCategory.Where(c => c.IsDeleted == false).ToList();
        //        if (Pro.Count != 0)
        //        {
        //            foreach (var item in Pro)
        //            {
        //                if (item.Name.ToLower().Contains(ProductName.ToLower()) || item.ARName.Contains(ProductName))
        //                {
        //                    model.ProductList.Add(item);
        //                    foreach (var item2 in Cat)
        //                    {
        //                        if (item.CategoryID == item2.ID)
        //                        {
        //                            //model.CategoryList.Add(item2);
        //                            SearchResult = item2.CategoryID;
        //                        }
        //                    }
        //                }
        //            }
        //            model.ProductList1 = model.ProductList.Where(c => c.IsDeleted == false).ToPagedList((int)pageNumber, 20);
        //            if (model.ProductList1.Count != 0)
        //            {
        //                //cal distance                      
        //                for (int i = 0; i < model.ProductList1.Count; i++)
        //                {
        //                    double lat = (double)model.ProductList1[i].FWYStoreProduct.FirstOrDefault().FWYStore.AspNetUsers.Latitude;
        //                    double lon = (double)model.ProductList1[i].FWYStoreProduct.FirstOrDefault().FWYStore.AspNetUsers.Longitude;
        //                    var sCoord = new GeoCoordinate(lat, lon);
        //                    var eCoord = new GeoCoordinate((double)latitude, (double)longitude);
        //                    if (model.ProductList1[i].IsDeleted == false)
        //                    {
        //                        model.Dis.Add(i, sCoord.GetDistanceTo(eCoord));
        //                       /* foreach(var img in model.ProductList1[i].FWYProductImage)
        //                        {
        //                            img.Image = Helper.ConnvertToImageSrc(img.Image, img.ImageExtension);
        //                        }*/
        //                        model.ProList.Add(i, model.ProductList1[i]);
        //                    }

        //                }
        //                if (model.Dis.Count > 0)
        //                {
        //                    model.Dis.Min(c => c.Value);
        //                }
        //            }
        //        }

        //        FWYLockupTable lockup = new FWYLockupTable()
        //        {
        //            SearchString = SearchString,
        //            SearchResultID = SearchResult,
        //            UserID = UserID,
        //            UserName = UserName,
        //            Longitude = longitude,
        //            Latitude = latitude,
        //            Location = location,
        //            SearchDateTime = DateTime.Now
        //        };
        //        db.FWYLockupTable.Add(lockup);
        //        db.SaveChanges();
        //    }
        //    if (ProductName == null && TypeID == null)
        //    {
        //        //foreach (var item in db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).ToList())
        //        //{
        //        //    model.CategoryList.Add(item);
        //        //}
        //        model.ProductList = db.FWYProduct.Include(f => f.FWYBrand).Include(f => f.FWYUnit).Include(f=>f.FWYProductImage)
        //            .Where(c => c.IsDeleted == false && c.CategoryID == CategoryID).ToList();
        //        model.FilterSortBy = 1;
        //        model.FilterPrice = 1;
        //        model.FilterTag = 0;
        //        model.Suppliers = new List<AspNetUsers>();

        //    }
        //    model.ProductList1 = model.ProductList.ToPagedList((int)pageNumber, 20);
        //    var x = 0;
        //    foreach (var item1 in model.ProductList1)
        //    {
        //        var y = 0;
        //     /*   foreach (var item in item1.FWYProductImage)
        //        {
        //            item.Image = Helper.ConnvertToImageSrc(item.Image, item.ImageExtension);

        //        }*/


        //        //to get the sppliername 
        //        FWYStoreProduct storeProduct = db.FWYStoreProduct.Include("FWYStore")
        //          .Where(s => s.ProductID == item1.ID ).FirstOrDefault();
        //        //to get Seller Name '
        //        AspNetUsers user = new AspNetUsers();

        //        if (storeProduct != null)
        //        {
        //             if (storeProduct.FWYStore != null)
        //            {
        //                user = db.AspNetUsers.FirstOrDefault(u => u.Id == storeProduct.FWYStore.SupplierID);
        //                 if(user != null)
        //                {
        //                    model.Suppliers.Add(user);
        //                }
        //            }
        //        }

        //    }

        //    model.PageCount = model.ProductList.Count / 20;
        //    model.PageNumber = (int)pageNumber;
        //    model.TypeId = TypeID;
        //    model.CategoryID = CategoryID;
        //    model.ProductName = ProductName;
        //    model.location = location;
        //    model.latitude = latitude;
        //    model.longitude = longitude;




        //    return View(model);
        //}

        public ActionResult Index(string ProductName, int? CategoryID, int? TypeID, decimal? longitude, decimal? latitude, string location, int? SortBy, int? Price, string SupplierID, int? pageNumber = 1)
        {
            ViewBag.SortBy = SortBy;
            ViewBag.Price = Price;
            ViewBag.SupplierID = SupplierID;

            ProductModel model = new ProductModel();
            model.CategoryList = new List<FWYCategory>();
            model.CategoryList = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            model.ProductList = new List<FWYProduct>();
            model.ProList = new Dictionary<int, FWYProduct>();
            model.Suppliers = db.AspNetUsers.Where(c => c.RoleID == "2").ToList();
            var query = "select * from FWYProduct where IsDeleted = 0 ";
            if (CategoryID != null && CategoryID != 0)
            {
                model.CategoryList = db.FWYCategory.Where(c => c.ID == CategoryID && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
                query += " and CategoryID = " + CategoryID;
            }

            if (TypeID != null)
            {
                query += " and TypeID = " + TypeID;
            }

            if (ProductName != null)
            {
                ProductName = ProductName.ToLower();
                query += " and (Name like '%" + ProductName + "%' or ARName like '%" + ProductName + "%')";
                model.ProductList = db.FWYProduct.SqlQuery(query).ToList();
                string UserID = null;
                string UserName = "Guest";
                if (User.Identity.IsAuthenticated)
                {
                    UserID = User.Identity.GetUserId();
                    var user = db.AspNetUsers.SingleOrDefault(c => c.Id == UserID);
                    UserName = user.Name;
                }

                FWYLockupTable lockup = new FWYLockupTable()
                {
                    SearchString = ProductName,
                    SearchResultID = model.ProductList != null && model.ProductList.Count() > 0 ? model.ProductList.First().CategoryID : null,
                    UserID = UserID,
                    UserName = UserName,
                    Longitude = longitude,
                    Latitude = latitude,
                    Location = location,
                    SearchDateTime = DateTime.Now
                };
                db.FWYLockupTable.Add(lockup);
                db.SaveChanges();
            }
            if (ProductName == null && TypeID == null)
            {
                model.FilterSortBy = 1;
                model.FilterPrice = 1;
                model.FilterTag = 0;
                model.Suppliers = new List<AspNetUsers>();
            }
            //.Include(f => f.FWYBrand).Include(f => f.FWYUnit).Include(f => f.FWYProductImage)
            model.ProductList = db.FWYProduct.SqlQuery(query).ToList();

            if(model.ProductList != null && model.ProductList.Count > 0)
            {
                model.ProductList = FilterProductsResult(model.ProductList, SortBy, Price, SupplierID);
            }
            

            model.ProductList1 = model.ProductList.ToPagedList((int)pageNumber, 20);
            foreach (var item1 in model.ProductList1)
            {
                //to get the sppliername 
                FWYStoreProduct storeProduct = db.FWYStoreProduct.Include("FWYStore")
                  .Where(s => s.ProductID == item1.ID).FirstOrDefault();
                //to get Seller Name '
                AspNetUsers user = new AspNetUsers();

                if (storeProduct != null)
                {
                    if (storeProduct.FWYStore != null)
                    {
                        user = db.AspNetUsers.FirstOrDefault(u => u.Id == storeProduct.FWYStore.SupplierID);
                        if (user != null)
                        {
                            model.Suppliers.Add(user);
                        }
                    }
                }

            }

            
            float pagesF = (float)model.ProductList.Count / 20;
            int pagesI = model.ProductList.Count / 20;
            int pages = pagesI;
            if(pagesF > (float)pagesI)
            {
                pages += 1;
            }
            model.PageCount = pages;
            model.PageNumber = (int)pageNumber;
            model.TypeId = TypeID;
            model.CategoryID = CategoryID;
            model.ProductName = ProductName;
            model.location = location;
            model.latitude = latitude;
            model.longitude = longitude;




            return View(model);
        }



        // GET: Products/Details/5
        public ActionResult Details(int ID)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = db.FWYProduct.SingleOrDefault(c => c.ID == ID);
            model.ProductColorList = new List<FWYColor>();
            model.ProductSizeList = new List<FWYSize>();
            foreach (var item in model.Product.FWYStoreProduct)
            {
                if (model.ProductSizeList.Where(c => c.ID == item.SizeID).ToList().Count == 0)
                {
                    if (item.FWYSize != null)
                    {
                        model.ProductSizeList.Add(item.FWYSize);
                    }
                }
            }
            foreach (var item in model.Product.FWYStoreProduct)
            {
                if (model.ProductColorList.Where(c => c.ID == item.ColorID).ToList().Count == 0)
                {
                    if (item.FWYColor != null)
                    {
                        model.ProductColorList.Add(item.FWYColor);
                    }
                }
            }
            model.RelatedProductList = db.FWYProduct.Where(c => c.CategoryID == model.Product.CategoryID && c.IsDeleted == false).ToList();
            /* foreach (var item in model.Product.FWYProductImage)
             {
                 item.Image = Helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
             }
             foreach (var item1 in model.RelatedProductList)
             {
                 foreach(var item in item1.FWYProductImage)
                 {
                     item.Image = Helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
                 }
             }*/
            return View(model);
        }


        public ActionResult GetPriceOfQuantity(int ProductID, int Quantity)
        {
            var product = db.FWYProduct.SingleOrDefault(c => c.ID == ProductID);
            decimal Price = (decimal)product.FinalPrice;
            foreach (var pp in product.FWYProductPriceRange)
            {
                if (Quantity >= pp.FromQuantity && Quantity < pp.ToQuantity)
                {
                    Price = (decimal)pp.Price;
                }
            }
            if (Quantity > product.FWYProductPriceRange.Max(c => c.ToQuantity))
            {
                Price = (decimal)product.FWYProductPriceRange.Min(c => c.Price);
            }
            return Json(Price, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSizeColorList(int SizeID, int ProductID)
        {
            var FWYColorList = new List<FWYColor>();
            var storeProduct = db.FWYStoreProduct.Where(c => c.ProductID == ProductID && c.SizeID == SizeID).ToList();
            foreach (var item in storeProduct)
            {
                if (item.ColorID != null)
                {
                    FWYColorList.Add(item.FWYColor);
                }
            }
            var ColorList = new SelectList(FWYColorList, "ID", "Name");
            return Json(ColorList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetColorSizeList(int ColorID, int ProductID)
        {
            var FWYSizeList = new List<FWYSize>();
            var storeProduct = db.FWYStoreProduct.Where(c => c.ProductID == ProductID && c.ColorID == ColorID).ToList();
            foreach (var item in storeProduct)
            {
                if (item.SizeID != null)
                {
                    FWYSizeList.Add(item.FWYSize);
                }
            }
            var SizeList = new SelectList(FWYSizeList, "ID", "Name");
            return Json(SizeList, JsonRequestBehavior.AllowGet);
        }



        #region Web Views for product
        private TagHelper helper = new TagHelper();
        [HttpGet]
        public ActionResult SupplierProducts(string SupplierId)
        {
            var user = db.AspNetUsers.FirstOrDefault(c => c.Id == SupplierId);
            if (user != null && user.IsSupplier == true)
            {
                FWYSupplierCooperation fWYSupplierCorporation = db.FWYSupplierCooperation.SingleOrDefault(c => c.SupplierID == SupplierId);
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
                ViewBag.IsSupplier = true;
                return View(fWYSupplierCorporation);
            }
            ViewBag.IsSupplier = false;
            return View(new FWYSupplierCooperation());
        }

        [HttpGet]
        public ActionResult CreateProduct(string SupplierId)
        {
            var user = db.AspNetUsers.FirstOrDefault(c => c.Id == SupplierId);
            if (user != null && user.IsSupplier == true)
            {
                StoreProduct StoreProduct = new StoreProduct();
                var store = new FWYStore();
                var stores = new List<FWYStore>();
                stores = db.FWYStore.Where(c => c.SupplierID == SupplierId).ToList();
                if (stores == null || stores.Count() == 0)
                {
                    db.FWYStore.Add(new FWYStore()
                    {
                        Name = "My Store",
                        ARName = "My store",
                        SupplierID = SupplierId
                    });
                    db.SaveChanges();
                }
                store = db.FWYStore.First(c => c.SupplierID == SupplierId);
                StoreProduct.StoreName = store.Name;
                StoreProduct.Product = new FWYProduct();
                StoreProduct.Product.IsAvailable = true;
                StoreProduct.Product.IsFeatured = false;
                StoreProduct.StoreID = store.ID;
                StoreProduct.UnitList = new SelectList(db.FWYUnit.Where(c => c.IsDeleted == false).ToList(), "ID", "Name");
                StoreProduct.ProductTypeList = System.Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Select(v => new SelectListItem
                {
                    Value = ((int)v).ToString(),
                    Text = v.ToString()
                }).ToList();
                var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;

                var Categories = new List<FWYCategory>();
                Categories = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).ToList();
                //ProductsController p = new ProductsController();
                //if (store.AspNetUsers.FWYSupplierCooperation.Count > 0)
                //{
                //    var company = store.AspNetUsers.FWYSupplierCooperation.First();

                //    foreach (var i in company.FWYSupplierCooperationCategory)
                //    {
                //        if (i.FWYCategory.IsDeleted == false)
                //        {
                //            Categories.Add(i.FWYCategory);
                //        }
                //    }
                //    if (lang == "ar-EG")
                //        StoreProduct.CategoryList = new SelectList(Categories, "ID", "ARName");
                //    else
                //        StoreProduct.CategoryList = new SelectList(Categories, "ID", "Name");

                //}

                if (lang == "ar-EG")
                    StoreProduct.CategoryList = new SelectList(Categories, "ID", "ARName");
                else
                    StoreProduct.CategoryList = new SelectList(Categories, "ID", "Name");
                var Categories2 = new List<FWYCategory>();

                StoreProduct.CategoryList2 = new SelectList(Categories2, "ID", "Name");

                StoreProduct.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "Name");
                if (lang == "ar-EG")
                {
                    StoreProduct.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "ARName");
                    StoreProduct.UnitList = new SelectList(db.FWYUnit.Where(c => c.IsDeleted == false).ToList(), "ID", "ARName");
                    var item1 = new SelectListItem { Value = "1", Text = "محلي" };
                    var item2 = new SelectListItem { Value = "2", Text = "مستورد" };
                    StoreProduct.ProductTypeList = new List<SelectListItem>();
                    StoreProduct.ProductTypeList.Add(item1);
                    StoreProduct.ProductTypeList.Add(item2);
                }
                ViewBag.IsSupplier = true;
                return View(StoreProduct);
            }
            ViewBag.IsSupplier = false;
            return View(new StoreProduct());
        }


        [HttpPost]
        public ActionResult CreateProduct(StoreProduct StoreProduct, List<System.Web.HttpPostedFileBase> Images, List<int> edPPIDs)
        {
            StoreProduct.Product.Name = StoreProduct.Product.ARName;
            StoreProduct.Product.CategoryID = StoreProduct.CateID;
            StoreProduct.Product.FinalPrice = StoreProduct.Product.Price;
            StoreProduct.Product.CreatedDateTime = DateTime.UtcNow;
            
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;

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

            return RedirectToAction("SupplierProducts", new { SupplierId = user.Id });
        }



        [HttpGet]
        public ActionResult EditProduct(int Id)
        {
            StoreProduct StoreProduct = new StoreProduct();
            StoreProduct.Product = new FWYProduct();
            StoreProduct.Product = db.FWYProduct.SingleOrDefault(c => c.ID == Id);
            StoreProduct.CateID = (int)StoreProduct.Product.CategoryID;
            var store = StoreProduct.Product.FWYStoreProduct.First().FWYStore;

            StoreProduct.StoreName = store.Name;
            StoreProduct.StoreID = store.ID;
            StoreProduct.UnitList = new SelectList(db.FWYUnit.Where(c => c.IsDeleted == false).ToList(), "ID", "Name");
            StoreProduct.ProductTypeList = System.Enum.GetValues(typeof(ProductType)).Cast<ProductType>().Select(v => new SelectListItem
            {
                Value = ((int)v).ToString(),
                Text = v.ToString()
            }).ToList();
            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;

            var Categories = new List<FWYCategory>();
            Categories = db.FWYCategory.Where(c => c.CategoryID == null && c.IsDeleted == false).ToList();
            //if (store.AspNetUsers.FWYSupplierCooperation.Count > 0)
            //{
            //    var company = store.AspNetUsers.FWYSupplierCooperation.First();

            //    foreach (var i in company.FWYSupplierCooperationCategory)
            //    {
            //        if (i.FWYCategory.IsDeleted == false)
            //        {
            //            Categories.Add(i.FWYCategory);
            //        }
            //    }
            //    if (lang == "ar-EG")
            //        StoreProduct.CategoryList = new SelectList(Categories, "ID", "ARName");
            //    else
            //        StoreProduct.CategoryList = new SelectList(Categories, "ID", "Name");

            //}

            if (lang == "ar-EG")
                StoreProduct.CategoryList = new SelectList(Categories, "ID", "ARName");
            else
                StoreProduct.CategoryList = new SelectList(Categories, "ID", "Name");

            var Categories2 = new List<FWYCategory>();
            var mainCategory = db.FWYCategory.FirstOrDefault(c => c.ID == StoreProduct.Product.CategoryID).FWYCategory2;
            ViewBag.mainCategory = mainCategory;
            Categories2 = db.FWYCategory.Where(c => c.CategoryID == mainCategory.ID).ToList();
            StoreProduct.CategoryList2 = new SelectList(Categories2, "ID", "Name");

            StoreProduct.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "Name");
            if (lang == "ar-EG")
            {
                StoreProduct.CountryList = new SelectList(db.FWYCountry.Where(c => c.IsDeleted == false).ToList(), "ID", "ARName");
                StoreProduct.UnitList = new SelectList(db.FWYUnit.Where(c => c.IsDeleted == false).ToList(), "ID", "ARName");
                var item1 = new SelectListItem { Value = "1", Text = "محلي" };
                var item2 = new SelectListItem { Value = "2", Text = "مستورد" };
                StoreProduct.ProductTypeList = new List<SelectListItem>();
                StoreProduct.ProductTypeList.Add(item1);
                StoreProduct.ProductTypeList.Add(item2);
            }
            return View(StoreProduct);
        }


        [HttpPost]
        public ActionResult EditProduct(StoreProduct StoreProduct, List<System.Web.HttpPostedFileBase> Images, List<int> edPPIDs)
        {

            StoreProduct.Product.Name = StoreProduct.Product.ARName;
            StoreProduct.Product.CategoryID = StoreProduct.CateID;
            StoreProduct.Product.FinalPrice = StoreProduct.Product.Price;

            var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;
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

            return RedirectToAction(nameof(SupplierProducts), new { SupplierId = user.Id });
        }



        [HttpGet]
        public ActionResult DeleteProduct(int id)
        {
            var product = db.FWYProduct.SingleOrDefault(c => c.ID == id);
            var supId = product.FWYStoreProduct.First().FWYStore.AspNetUsers.Id;
            product.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction(nameof(SupplierProducts), new { SupplierId = supId });
        }

        [HttpGet]
        public ActionResult ProductDetails(int id)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = db.FWYProduct.SingleOrDefault(c => c.ID == id);
            model.ProductColorList = new List<FWYColor>();
            model.ProductSizeList = new List<FWYSize>();
            foreach (var item in model.Product.FWYStoreProduct)
            {
                if (model.ProductSizeList.Where(c => c.ID == item.SizeID).ToList().Count == 0)
                {
                    if (item.FWYSize != null)
                    {
                        model.ProductSizeList.Add(item.FWYSize);
                    }
                }
            }
            foreach (var item in model.Product.FWYStoreProduct)
            {
                if (model.ProductColorList.Where(c => c.ID == item.ColorID).ToList().Count == 0)
                {
                    if (item.FWYColor != null)
                    {
                        model.ProductColorList.Add(item.FWYColor);
                    }
                }
            }
            model.RelatedProductList = db.FWYProduct.Where(c => c.CategoryID == model.Product.CategoryID && c.IsDeleted == false).ToList();
            return View(model);
        }

        #endregion


        public ActionResult GetAllMyFav()
        {
            var products = new List<ProductVM>();
            var CurrentUserId = User.Identity.GetUserId();
            if (CurrentUserId != null)
            {
                List<FWYWishList> fWYWishLists = db.FWYWishList.Where(w => w.IsDeleted == false && w.UserID == CurrentUserId).ToList();
                if (fWYWishLists.Count > 0)
                {
                    foreach (var item in fWYWishLists)
                    {
                        FWYProduct fWYProduct = db.FWYProduct
                            .Include("FWYProductImage")
                            .Include("FWYProductPriceRange")
                            .Include("FWYBrand")
                            .Include("FWYStoreProduct")
                            .FirstOrDefault(p => p.ID == item.ProductID);


                        if (fWYProduct != null)
                        {
                            //to get the sppliername 
                            FWYStoreProduct storeProduct = db.FWYStoreProduct.Include("FWYStore")
                              .Where(s => s.ProductID == fWYProduct.ID).FirstOrDefault();
                            //to get Seller Name '
                            AspNetUsers user = new AspNetUsers();
                            FWYSupplierCooperation Company = new FWYSupplierCooperation();
                            string supName = "";
                            bool isSupplierVerified = false;
                            if (storeProduct != null)
                            {
                                if (storeProduct.FWYStore != null)
                                {
                                    Company = db.FWYSupplierCooperation.FirstOrDefault(u => u.SupplierID == storeProduct.FWYStore.SupplierID);
                                    if (Company != null)
                                    {
                                        supName = Company.ArName;
                                        isSupplierVerified = Company.IsVerified;
                                    }
                                }
                            }
                            products.Add(new ProductVM
                            {
                                ID = fWYProduct.ID,
                                ARName = fWYProduct.ARName,
                                Name = fWYProduct.Name,
                                isFavorite = true,
                                SupplierName = supName,
                                Price = fWYProduct.Price ?? 0,
                                MinAmount = fWYProduct.FWYProductPriceRange == null || fWYProduct.FWYProductPriceRange.Count() <= 0 ? 0 : fWYProduct.FWYProductPriceRange.Min(x => x.FromQuantity),
                                Min = fWYProduct.FWYProductPriceRange == null || fWYProduct.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)fWYProduct.FWYProductPriceRange.Min(c => c.Price)),
                                Max = fWYProduct.FWYProductPriceRange == null || fWYProduct.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)fWYProduct.FWYProductPriceRange.Max(c => c.Price)),
                                Image = fWYProduct.FWYProductImage.FirstOrDefault(c => c.IsMain == true) == null ? "" : fWYProduct.FWYProductImage.FirstOrDefault(c => c.IsMain == true).Image,
                                LessQuantityGomla = fWYProduct.LessQuantityGomla,
                                IsSupplierVerified = isSupplierVerified,
                            });
                        }
                    }
                }
            }
            return View(products);
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
