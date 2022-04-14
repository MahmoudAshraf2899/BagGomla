using BagGomla.Enums;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BagGomla.Business
{
    public class SuppliersPageAPIService
    {

        DatabaseContext db = new DatabaseContext();


        public Response GetSupplierProducts(int id, string currentUserId, int page = 1)
        {
            Response<List<ProductVM>> result = new Response<List<ProductVM>>();
            try
            {
                List<ProductVM> productsVM = new List<ProductVM>();
                var sup = db.FWYSupplierCooperation.FirstOrDefault(c => c.ID == id);
                if (sup != null)
                {
                    if (sup.AspNetUsers != null)
                    {
                        var supplierId = sup.AspNetUsers.Id;
                        var storeProducts = db.FWYStoreProduct.Where(p => p.FWYStore.SupplierID == supplierId && p.IsDeleted == false).ToList();
                        bool isSupplierVerified = false;
                        if (storeProducts != null && storeProducts.Count() > 0)
                        {
                            string CompanyName = "";
                            string phoneNumber = "";
                            decimal lati = 0;
                            decimal longi = 0;
                            AspNetUsers users = db.AspNetUsers.FirstOrDefault(users => users.Id == supplierId && users.IsDeleted == false);
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
                            var productIds = storeProducts.Select(sp => sp.ProductID).ToList();
                            var products = db.FWYProduct.Include("FWYProductPriceRange").Include("FWYProductImage")
                                .Where(p => productIds.Contains(p.ID) && p.IsDeleted == false).ToList();
                            var message = "";
                            if (products.Count() <= 0)
                                message = "There is no products for this supplier";
                            products = products.OrderByDescending(c => c.ID).Skip((page - 1) * 20).Take(20).ToList();
                            if (products.Count() <= 0)
                                message = "Not products in this page";
                            if (products.Count() > 0)
                            {
                                //to check if the product in the wihlist or not 
                                foreach (var item in products)
                                {
                                    //to check if the product in the wihlist or not 
                                    FWYWishList ProductinWishList = new FWYWishList();
                                    ProductinWishList = db.FWYWishList.FirstOrDefault(w => w.UserID == currentUserId && w.ProductID == item.ID && w.IsDeleted == false);

                                    productsVM.Add(new ProductVM
                                    {
                                        ID = item.ID,
                                        ARName = item.ARName,
                                        Name = item.Name,
                                        isFavorite = ProductinWishList != null && ProductinWishList.Id > 0 ? true : false,
                                        PhoneNumber = phoneNumber,
                                        SupplierName = CompanyName,
                                        Price = item.Price ?? 0,
                                        MinAmount = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? 0 : item.FWYProductPriceRange.Min(c => c.FromQuantity),
                                        Image = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? "" : item.FWYProductImage.FirstOrDefault().Image,
                                        Min = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Min(c => c.Price)),
                                        Max = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Max(c => c.Price)),
                                        ImageList = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? null : item.FWYProductImage.Select(c =>
                                        {
                                            string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                                            return imgPath;
                                        }).ToList(),
                                        LessQuantityGomla = item.LessQuantityGomla,
                                        latitude = lati,
                                        longtitude = longi,
                                        IsSupplierVerified = isSupplierVerified
                                    });
                                }
                                result.DataResult = productsVM;
                                result.Code = ResponseCode.Success;
                            }
                            else
                            {
                                result.Message = message;
                                result.Code = ResponseCode.Success;
                            }
                        }
                        else
                        {
                            result.Message = "There is no store for this supplier";
                            result.Code = ResponseCode.Success;
                        }
                    }
                    else
                    {
                        result.Message = "There is no user";
                        result.Code = ResponseCode.UserNotFound;
                    }
                }
                else
                {
                    result.Message = "There is no supplier";
                    result.Code = ResponseCode.NoSuppliersFound;
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
        public Response<List<SuppliersVM>> getAllSuppliers(int page)
        {
            Response<List<SuppliersVM>> responseData = new Response<List<SuppliersVM>>();
            responseData.DataResult = db.AspNetUsers.Where(u => u.IsSupplier && u.IsDeleted != true).OrderByDescending(p => p.Id).Skip(page * 10).Take(10).Select(u => new SuppliersVM
            {
                Id = u.Id,
                Address = u.Address,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber,
                SupplierImage = u.Image,
                IsSupplierVerified = u.FWYSupplierCooperation.First().IsVerified
            }).ToList();

            if (responseData.DataResult.Count > 0)
                responseData.Code = Enums.ResponseCode.Success;

            return responseData;
        }


        public Response<List<SupplierStoreVM>> getSupplierStores(string id)
        {
            Response<List<SupplierStoreVM>> responseData = new Response<List<SupplierStoreVM>>();
            responseData.DataResult = new List<SupplierStoreVM>();
            responseData.DataResult = db.FWYStore.Where(c => c.SupplierID == id).Select(item => new SupplierStoreVM()
            {
                Id = item.ID,
                EnName = item.Name,
                ArName = item.ARName,
                EnLocation = item.Location,
                ArLocation = item.ARLocation,
                Long = item.Longitude != null ? item.Longitude.ToString() : "",
                Lat = item.Latitude != null ? item.Latitude.ToString() : ""
            }).ToList();
            return responseData;
        }

        public Response<List<SuppliersVM>> filterSuppliers(int? productsNum, bool? isVerified,int? categoryId, int? subCategoryId, int page = 1)
        {

            var suppliers = db.FWYSupplierCooperation.Where(c => c.IsDeleted == false).ToList();
            if(productsNum != null)
            {
                suppliers = suppliers.Where(c => c.AspNetUsers.FWYStore.Sum(n => n.FWYStoreProduct.Where(r => r.FWYProduct.IsDeleted == false).Count()) <= productsNum).ToList();
            }
            
            if(isVerified != null)
            {
                suppliers = suppliers.Where(c => c.IsVerified == isVerified).ToList();
            }

            if(categoryId != null)
            {
                suppliers = suppliers.Where(c => c.FWYSupplierCooperationCategory.Where(x => x.CategoryID == categoryId).Count() > 0).ToList();
            }

            if(subCategoryId != null)
            {
                var subcate = db.FWYCategory.FirstOrDefault(c => c.ID == subCategoryId);
                if(subcate != null)
                {
                    if(subcate.CategoryID != null)
                    {
                        suppliers = suppliers.Where(c => c.FWYSupplierCooperationCategory.Where(x => x.CategoryID == subcate.CategoryID).Count() > 0).ToList();
                    }
                }
            }

            suppliers = suppliers.OrderByDescending(c => c.ID).Skip((page - 1) * 20).Take(20).ToList();
            var suppliersMainData = new List<SuppliersVM>();
            foreach(var item in suppliers)
            {
                var x = item.AspNetUsers.FWYStore.Sum(c => c.FWYStoreProduct.Count());

                suppliersMainData.Add(new SuppliersVM()
                {
                    Id = item.AspNetUsers.Id,
                    Address = item.AspNetUsers.Address,
                    Name = item.AspNetUsers.Name,
                    PhoneNumber = item.AspNetUsers.PhoneNumber,
                    SupplierImage = item.AspNetUsers.Image != null ? "/Images/Users/" + item.AspNetUsers.Image : "",
                    ProductsNumber = item.AspNetUsers.FWYStore.Sum(c => c.FWYStoreProduct.Where(r => r.FWYProduct.IsDeleted == false).Count()),
                    IsSupplierVerified = item.IsVerified
                });
            }
            Response<List<SuppliersVM>> responseData = new Response<List<SuppliersVM>>();
            responseData.DataResult = suppliersMainData;

            if (responseData.DataResult.Count > 0)
                responseData.Code = Enums.ResponseCode.Success;

            return responseData;
        }

    }


}