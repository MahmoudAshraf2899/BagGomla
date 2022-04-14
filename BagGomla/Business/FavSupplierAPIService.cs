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
    public class FavSupplierAPIService
    {
        DatabaseContext db = new DatabaseContext();
        public Response AddtoFavSupplier(string supId, string userid)
        {
            Response response = new Response();
            if (userid == "" || userid == null || supId == "" || supId == null)
            {
                response.Code = ResponseCode.UserNotFound;
                response.Message = "User not found";
                return response;
            }
            if (supId != null && userid != null)
            {
                try
                {
                    var sup = db.FWYSupplierCooperation.FirstOrDefault(c => c.SupplierID == supId);
                    if (sup != null)
                    {
                        var oldFavSupplier = db.FWYFavSupplier.Where(c => c.UserID == userid && c.SupplierID == sup.ID).ToList();
                        if (oldFavSupplier != null && oldFavSupplier.Count > 0)
                        {
                            response.Code = ResponseCode.ProductAlreadyAdded;
                            response.Message = "This Supplier is already existing in your favourite";
                        }
                        else
                        {
                            db.FWYFavSupplier.Add(new FWYFavSupplier
                            {
                                DateIn = DateTime.Today.Date,
                                IsDeleted = false,
                                SupplierID = sup.ID,
                                UserID = userid
                            });
                            db.SaveChanges();
                            response.Code = ResponseCode.Success;
                            response.Message = "Supplier is added to favourit successfully";
                        }
                    }
                    else
                    {
                        response.Code = ResponseCode.NoSupplierFound;
                        response.Message = "No Supplier Found";
                    }

                }
                catch(Exception e)
                {
                    response.Code = ResponseCode.Error;
                    response.Message = e.Message;
                    return response;
                }
            }

            return response;
        }

        public Response<List<SuppliersVM>> getFavSupplier(string userid)
        {
            Response<List<SuppliersVM>> responseData = new Response<List<SuppliersVM>>();
            responseData.DataResult = new List<SuppliersVM>();
            if (userid == "" || userid == null)
            {
                responseData.Code = ResponseCode.UserNotFound;
                responseData.Message = "User not found";
                return responseData;
            }

            if (userid != null)
            {
                List<FWYFavSupplier> fWYFavSuppliers = new List<FWYFavSupplier>();
                fWYFavSuppliers = db.FWYFavSupplier.Where(w => w.IsDeleted == false && w.UserID == userid).ToList();
                
                if (fWYFavSuppliers.Count > 0)
                {
                    var sup = new FWYSupplierCooperation();
                    foreach (var item in fWYFavSuppliers)
                    {
                        sup = db.FWYSupplierCooperation.FirstOrDefault(c => c.ID == item.SupplierID);
                        if(sup != null)
                        {
                            responseData.DataResult.Add(new SuppliersVM
                            {
                                Id = sup.AspNetUsers.Id,
                                Address = sup.AspNetUsers.Address,
                                Name = sup.AspNetUsers.Name,
                                PhoneNumber = sup.AspNetUsers.PhoneNumber,
                                SupplierImage = sup.AspNetUsers.Image
                            });
                        }
                       
                        responseData.Code = ResponseCode.Success;
                    }
                }
                else
                {
                    responseData.Code = ResponseCode.NoSuppliersFound;
                    responseData.Message = "No Suppliers found in your favourite";
                }
            }

            return responseData;
        }

        public Response DeleteItemFromFavSupplier(string supId, string userid)
        {
            Response response = new Response();
            if (userid == "" || userid == null || supId == "" || supId == null)
            {
                response.Code = ResponseCode.UserNotFound;
                response.Message = "User not found";
                return response;
            }
            if (supId != null && userid != null)
            {
                var sup = db.FWYSupplierCooperation.FirstOrDefault(c => c.SupplierID == supId);
                if (sup != null)
                {
                    FWYFavSupplier FavSupplier = db.FWYFavSupplier.FirstOrDefault(w => w.IsDeleted != true && w.SupplierID == sup.ID && w.UserID == userid);

                    if (FavSupplier != null)
                    {
                        try
                        {
                            db.FWYFavSupplier.Remove(FavSupplier);
                            db.SaveChanges();
                            response.Code = ResponseCode.Success;
                            response.Message = "Supplier is Deleted Successfully";
                        }
                        catch (Exception ex)
                        {
                            response.Code = ResponseCode.TryAgain;
                            response.Message = "There is a problem please try again in another time";
                            return response;
                        }
                    }
                    else
                    {
                        response.Code = ResponseCode.NoSupplierFound;
                        response.Message = "This Supplier is not added to your favourite";
                    }
                }
                else
                {
                    response.Code = ResponseCode.NoSupplierFound;
                    response.Message = "No Supplier Found";
                }
            }
            return response;
        }
    }



}