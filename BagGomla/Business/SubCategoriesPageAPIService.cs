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
    public class SubCategoriesPageAPIService
    {
        DatabaseContext db = new DatabaseContext();
        //public Response<List<ProductVM>> getProducts(int SubCategoryId,int page = 1)
        //{
        //    Response<List<ProductVM>> responseData = new Response<List<ProductVM>>();
        //    responseData.DataResult = new List<ProductVM>();

        //    List<FWYProduct> products = db.FWYProduct
        //         .Include("FWYProductPriceRange")
        //         .Include("FWYProductImage")
        //        .Where(p => p.IsDeleted == false &&  p.CategoryID == SubCategoryId).OrderByDescending(c=>c.ID).Skip((page - 1) * 20).Take(20).ToList();

        //    if (products.Count > 0)
        //    {
        //        //to check if the product in the wihlist or not 
        //        string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
        //        FWYWishList ProductinWishList = new FWYWishList();
        //        foreach (var item in products)
        //        {
        //            //to get the sppliername 
        //            FWYStoreProduct storeProduct = db.FWYStoreProduct.Include("FWYStore")
        //              .Where(s => s.ProductID == item.ID).FirstOrDefault();
        //            //to get Seller Name '
        //            AspNetUsers user = new AspNetUsers();
        //            FWYSupplierCooperation Company = new FWYSupplierCooperation();
        //            if (storeProduct != null)
        //            {
        //                if (storeProduct.FWYStore != null)
        //                {
        //                    user = db.AspNetUsers.FirstOrDefault(u => u.Id == storeProduct.FWYStore.SupplierID);
        //                    Company = db.FWYSupplierCooperation.FirstOrDefault(u => u.SupplierID == storeProduct.FWYStore.SupplierID);

        //                }
        //            }
        //            responseData.DataResult.Add(new ProductVM
        //            {
        //                ID = item.ID,
        //                ARName = item.ARName == null ? "" : item.ARName,
        //                Name = item.Name == null ? "" : item.Name,
        //                PhoneNumber = user != null ?user.PhoneNumber : "",
        //                CpmanyName = Company == null ? "" : Company.Name , 
        //                ArCpmanyName = Company == null ? "" : Company.ArName,
        //                isFavorite = ProductinWishList == null ? false : true ,
        //                Price = item.Price > 0 ? item.Price : 0,
        //                MinAmount = item.FWYProductPriceRange.Count > 0 ? item.FWYProductPriceRange.FirstOrDefault().FromQuantity : 0,
        //                SupplierName = user == null ? "" : user.Name,
        //                Country = item.Country != "" ? item.Country : "",
        //                ArCountry = item.ArCountry != "" ? item.ArCountry : "",
        //                Image = item.FWYProductImage.Count == 0 ? "" : item.FWYProductImage.FirstOrDefault().Image,
        //                IsSupplierVerified = Company == null ? false : Company.IsVerified

        //            }) ;
        //        }

        //        if (responseData.DataResult.Count > 0)
        //            responseData.Code = ResponseCode.Success;
        //    }

        //    return responseData;
        //}

        public Response<List<ProductVM>> getProducts(int SubCategoryId, int page = 1)
        {
            Response<List<ProductVM>> responseData = new Response<List<ProductVM>>();
            responseData.DataResult = new List<ProductVM>();
            var products = new List<FWYProduct>();
            products = db.FWYProduct.Where(p => p.IsDeleted != true && p.CategoryID == SubCategoryId)
                           .Include("FWYProductPriceRange")
                           .Include("FWYBrand")
                           .Include("FWYProductImage")
                           .Include("FWYStoreProduct")
                           .Include("FWYCountry").ToList();

            products = products.OrderByDescending(c => c.ID).Skip((page - 1) * 20).Take(20).ToList();
            if (products.Count > 0)
            {
                //to check if the product in the wihlist or not 
                string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
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

                    responseData.DataResult.Add(new ProductVM
                    {
                        ID = item.ID,
                        ARName = item.ARName,
                        Name = item.Name,
                        isFavorite = ProductinWishList == null ? false : true,
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
                        Country = item.FWYCountry.Name,
                        ArCountry = item.FWYCountry.ArName
                    });
                }
            }
            responseData.Code = ResponseCode.Success;
            return responseData;
        }

    }
}