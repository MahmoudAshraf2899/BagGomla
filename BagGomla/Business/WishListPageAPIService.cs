using BagGomla.Enums;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Business
{
    public class WishListPageAPIService
    {
        DatabaseContext db = new DatabaseContext();
        public Response AddtoWishList(int productId, string userid)
        {
            Response response = new Response();
            if (userid == "" || userid == null)
            {
                response.Code = ResponseCode.UserNotFound;
                response.Message = "User not found";
                return response;
            }
            if (productId > 0 && userid != "")
            {
                try
                {
                    var oldwishlist = db.FWYWishList.Where(c => c.UserID == userid && c.ProductID == productId).ToList();
                    if(oldwishlist != null && oldwishlist.Count > 0)
                    {
                        response.Code = ResponseCode.ProductAlreadyAdded;
                        response.Message = "This Product is already existing in your favourite";
                    }
                    else
                    {
                        db.FWYWishList.Add(new FWYWishList
                        {
                            DateIn = DateTime.Today.Date,
                            IsDeleted = false,
                            ProductID = productId,
                            UserID = userid
                        });
                        db.SaveChanges();
                        response.Code = ResponseCode.Success;
                        response.Message = "Product is added to favourit successfully";
                    }
                }
                catch
                {
                    response.Code = ResponseCode.Error;
                    response.Message = "The process is failed to add please try again in another time";
                    return response;
                }
            }

            return response;
        }

        public Response<List<ProductVM>> getWishList(string userid)
        {
            Response<List<ProductVM>> responseData = new Response<List<ProductVM>>();
            responseData.DataResult = new List<ProductVM>();
            if (userid == "" || userid == null)
            {
                responseData.Code = ResponseCode.UserNotFound;
                responseData.Message = "User not found";
                return responseData;
            }

            if (userid != "")
            {
                List<FWYWishList> fWYWishLists = db.FWYWishList.Where(w => w.IsDeleted == false && w.UserID == userid).ToList();
                responseData.Code = ResponseCode.Error;
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
                            string phone = "";
                            decimal lati = 0;
                            decimal longi = 0;
                            bool isSupplierVerified = false;
                            if (storeProduct != null)
                            {
                                if (storeProduct.FWYStore != null)
                                {
                                    user = db.AspNetUsers.FirstOrDefault(u => u.Id == storeProduct.FWYStore.SupplierID);
                                    Company = db.FWYSupplierCooperation.FirstOrDefault(u => u.SupplierID == storeProduct.FWYStore.SupplierID);
                                    if(user != null)
                                    {
                                        phone = user.PhoneNumber;
                                        lati = (decimal)user.Latitude;
                                        longi = (decimal)user.Longitude;
                                    }
                                    if(Company != null)
                                    {
                                        supName = Company.ArName;
                                        isSupplierVerified = Company.IsVerified;
                                    }
                                }
                            }
                            responseData.DataResult.Add(new ProductVM
                            {
                                ID = fWYProduct.ID,
                                ARName = fWYProduct.ARName,
                                Name = fWYProduct.Name,
                                isFavorite = fWYWishLists == null ? false : true,
                                PhoneNumber = phone,
                                SupplierName = supName,
                                Price = fWYProduct.Price ?? 0,
                                MinAmount = fWYProduct.FWYProductPriceRange == null || fWYProduct.FWYProductPriceRange.Count() <= 0 ? 0 : fWYProduct.FWYProductPriceRange.Min(x => x.FromQuantity),
                                Min = fWYProduct.FWYProductPriceRange == null || fWYProduct.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)fWYProduct.FWYProductPriceRange.Min(c => c.Price)),
                                Max = fWYProduct.FWYProductPriceRange == null || fWYProduct.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)fWYProduct.FWYProductPriceRange.Max(c => c.Price)),
                                ImageList = fWYProduct.FWYProductImage == null || fWYProduct.FWYProductImage.Count() <= 0 ? null : fWYProduct.FWYProductImage.Select(c =>
                                {
                                    string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                                    return imgPath;
                                }).ToList(),
                                latitude = lati,
                                longtitude = longi,
                                LessQuantityGomla = fWYProduct.LessQuantityGomla,
                                IsSupplierVerified = isSupplierVerified,
                                Country = fWYProduct.FWYCountry != null ? fWYProduct.FWYCountry.Name : "",
                                ArCountry = fWYProduct.FWYCountry != null ? fWYProduct.FWYCountry.ArName : ""
                            });

                            responseData.Code = ResponseCode.Success;
                        }
                    }
                }
            }

            return responseData;
        }

        public Response DeleteItemFromWishList(int productId, string userid)
        {
            Response response = new Response();
            if (userid == "" || userid == null)
            {
                response.Code = ResponseCode.UserNotFound;
                response.Message = "User not found";
                return response;
            }
            if (productId > 0 && userid != "")
            {
                FWYWishList wishList = db.FWYWishList.FirstOrDefault(w =>  w.ProductID == productId && w.UserID == userid);

                if (wishList != null)
                {
                    try
                    {
                        db.FWYWishList.Remove(wishList);
                        db.SaveChanges();
                        response.Code = ResponseCode.Success;
                    }
                    catch (Exception ex)
                    {
                        return response;
                    }
                }
                else
                {
                    response.Code = ResponseCode.Error;
                    response.Message = "This item doesn't exist in wisht list";
                }


            }
            return response;
        }
    }



}