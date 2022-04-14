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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
namespace BagGomla.Business
{
    public class HomePageApiService
    {
        DatabaseContext db = new DatabaseContext();

        public HomePageVM homePage(int page)
        {
            HomePageVM homePageVM = new HomePageVM();
            homePageVM.Categories = new List<CategoriesHomeVM>();
            homePageVM.Products = new List<ProductVM>();
            //get Categories and their total number of products  
            homePageVM.Categories = db.FWYCategory.Include(c => c.FWYProduct).Where(c => c.IsDeleted != true && c.CategoryID == null).OrderBy(c => c.Number).ThenBy(c => c.ID).Select(c => new CategoriesHomeVM
            {
                catID = c.ID,
                Name = c.Name,
                ArName = c.ARName,
                TotalProduct = c.FWYProduct.Count,
                Image = c.Image == null ? "" : "/Images/Categories/" + c.Image
            }).ToList();

            //get the special products

            List<FWYProduct> products = db.FWYProduct.Where(p => p.IsDeleted != true && p.IsAvailable == true && p.Show == true)
           .Include("FWYProductPriceRange")
           .Include("FWYBrand")
           .Include("FWYProductImage")
           .Include("FWYStoreProduct")
           .AsEnumerable().OrderByDescending(c => c.ID).Skip((page - 1) * 20).Take(20)
           .ToList();

            if (products.Count > 0)
            {
                //to check if the product in the wihlist or not 
                string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
                FWYWishList ProductinWishList = new FWYWishList();
                foreach (var item in products)
                {
                    string CompanyName = "";
                    string phoneNumber = "";
                    bool isSupplierVerified = false;
                    decimal lati = 0;
                    decimal longi = 0;
                    //to get the Supplier information
                    if (item.FWYStoreProduct != null && item.FWYStoreProduct.Count() > 0)
                    {
                        var x = item.FWYStoreProduct.First().FWYStore.SupplierID;
                        var users = db.AspNetUsers.FirstOrDefault(users => users.Id == x);
                        if (users != null)
                        {
                            CompanyName = users.Name;
                            phoneNumber = users.PhoneNumber;
                            var sub = db.FWYSupplierCooperation.FirstOrDefault(c => c.SupplierID == users.Id);
                            if (sub != null)
                                isSupplierVerified = sub.IsVerified;
                            lati = (decimal)users.Latitude;
                            longi = (decimal)users.Longitude;
                        }
                    }

                    //to check if the product in the wihlist or not 

                    if (CurrentUserid != "")
                    {
                        ProductinWishList = db.FWYWishList.FirstOrDefault(w => w.UserID == CurrentUserid && w.ProductID == item.ID);
                    }

                    homePageVM.Products.Add(new ProductVM
                    {
                        ID = item.ID,
                        ARName = item.ARName,
                        Name = item.Name,
                        isFavorite = ProductinWishList != null && ProductinWishList.Id > 0 ? true : false,
                        PhoneNumber = phoneNumber,
                        SupplierName = CompanyName,
                        Price = item.Price ?? 0,
                        MinAmount = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? 0 : item.FWYProductPriceRange.Min(x => x.FromQuantity),
                        Min = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Min(c => c.Price)),
                        Max = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Max(c => c.Price)),
                        ImageList = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? null : item.FWYProductImage.Select(c =>
                        {
                            string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                            return imgPath;
                        }).ToList(),
                        //Image = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? "" : "/Images/Products/" + item.FWYProductImage.FirstOrDefault().Image,
                        latitude = lati,
                        longtitude = longi,
                        LessQuantityGomla = item.LessQuantityGomla,
                        IsSupplierVerified = isSupplierVerified
                    });
                }
            }

            homePageVM.AdsSlider = db.FWYAds.Where(s => s.IsDeleted == false).Select(s => new AdSliderVM
            {
                Id = s.Id,
                DetailsURL = s.DetailsURL,
                Image = s.Image == null ? "" : "/Images/SliderAds/" + s.Image,
                ImageExtension = s.ImageExtension,
            }).ToList();

            return homePageVM;
        }
        public List<ProductVM> Search(string productName, string currentUserId, int page = 1)
        {
            if (productName != null)
                productName = productName.ToLower();
            var products = db.FWYProduct.Where(p => (p.ARName.Contains(productName) || p.Name.Contains(productName)) && p.IsDeleted == false)
            .Include("FWYProductPriceRange")
                           .Include("FWYBrand")
                           .Include("FWYProductImage")
                           .Include("FWYStoreProduct")
                           .Include("FWYCountry")
                           .OrderByDescending(c => c.ID).Skip((page - 1) * 20)
            .Take(20).ToList();
            var myProducts = new List<ProductVM>();
            if (products.Count > 0)
            {
                //to check if the product in the wihlist or not 
                string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
                if (CurrentUserid == null)
                    CurrentUserid = currentUserId;
                FWYWishList ProductinWishList = new FWYWishList();
                foreach (var item in products)
                {
                    string CompanyName = "";
                    string phoneNumber = "";
                    decimal lati = 0;
                    decimal longi = 0;
                    bool isSupplierVerified = false;
                    //to get the Supplier information
                    if (item.FWYStoreProduct != null && item.FWYStoreProduct.Count() > 0)
                    {
                        var x = item.FWYStoreProduct.First().FWYStore.SupplierID;
                        var users = db.AspNetUsers.FirstOrDefault(users => users.Id == x);
                        if (users != null)
                        {
                            CompanyName = users.Name;
                            phoneNumber = users.PhoneNumber;
                            var sup = db.FWYSupplierCooperation.FirstOrDefault(c => c.SupplierID == users.Id);
                            if (sup != null)
                                isSupplierVerified = sup.IsVerified;
                            lati = (decimal)users.Latitude;
                            longi = (decimal)users.Longitude;
                        }
                    }

                    //to check if the product in the wihlist or not 

                    if (CurrentUserid != "" && CurrentUserid != null)
                    {
                        ProductinWishList = db.FWYWishList.FirstOrDefault(w => w.UserID == CurrentUserid && w.ProductID == item.ID);
                    }

                    myProducts.Add(new ProductVM
                    {
                        ID = item.ID,
                        ARName = item.ARName,
                        Name = item.Name,
                        isFavorite = ProductinWishList != null && ProductinWishList.Id > 0 ? true : false,
                        PhoneNumber = phoneNumber,
                        SupplierName = CompanyName,
                        Price = item.Price ?? 0,
                        MinAmount = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? 0 : item.FWYProductPriceRange.Min(x => x.FromQuantity),
                        Min = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Min(c => c.Price)),
                        Max = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Max(c => c.Price)),
                        ImageList = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? null : item.FWYProductImage.Select(c =>
                        {
                            string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                            return imgPath;
                        }).ToList(),
                        latitude = lati,
                        longtitude = longi,
                        LessQuantityGomla = item.LessQuantityGomla,
                        IsSupplierVerified = isSupplierVerified,
                        Country = item.FWYCountry != null ? item.FWYCountry.Name : "",
                        ArCountry = item.FWYCountry != null ? item.FWYCountry.ArName : ""
                    });
                }
            }
            return myProducts;
        }
    }
}