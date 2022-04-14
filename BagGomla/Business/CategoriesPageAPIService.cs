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
    public class CategoriesPageAPIService
    {
        DatabaseContext db = new DatabaseContext();

        public Response<List<SuppliersVM>> SuppliersInCategories(int id, int page = 1)
        {
            Response<List<SuppliersVM>> result = new Response<List<SuppliersVM>>();
            List<SuppliersVM> suppliers = new List<SuppliersVM>();
            var companies = db.FWYSupplierCooperation.Where(c => c.IsDeleted == false && c.FWYSupplierCooperationCategory.Where(x => x.CategoryID == id).Count() > 0).OrderByDescending(c => c.ID).Skip((page - 1) * 20).Take(20).ToList();
            foreach(var item in companies)
            {
                suppliers.Add(new SuppliersVM
                {
                    Id = item.ID.ToString(),
                    Address = item.AspNetUsers.Address,
                    Name = item.AspNetUsers.Name,
                    PhoneNumber = item.AspNetUsers.PhoneNumber,
                    SupplierImage = item.AspNetUsers.Image != null ? "/Images/Users/" + item.AspNetUsers.Image : "",
                    SupplierImageExtension = item.AspNetUsers.ImageExtension,
                    IsSupplierVerified = item.IsVerified
                });
            }
            //get the SubCategories of the this Categories 
            //List<FWYCategory> subCategories = db.FWYCategory.Where(c => c.CategoryID == id && c.IsDeleted != true).OrderBy(c => c.Number).ThenBy(c => c.ID).ToList();
            //if (subCategories.Count > 0)
            //{
            //    foreach (var item in subCategories)
            //    {
            //        var supplierCooperationCategory = db.FWYSupplierCooperationCategory.Where(sc=>sc.CategoryID == id).ToList();
            //        if (supplierCooperationCategory != null)
            //        {
            //            foreach (var item2 in supplierCooperationCategory)
            //            {
            //                var suppliersCorp = db.FWYSupplierCooperation.FirstOrDefault(c => c.ID == item2.CompanyID);
            //                var supplier = db.AspNetUsers.FirstOrDefault(u => u.Id == suppliersCorp.SupplierID && u.IsDeleted == false);
            //                if (supplier != null)
            //                {
            //                    if(suppliers.Where(c=> c.Id == suppliersCorp.ID.ToString()).Count() == 0)
            //                    {
            //                        suppliers.Add(new SuppliersVM
            //                        {
            //                            Id = suppliersCorp.ID.ToString(),
            //                            Address = supplier.Address,
            //                            Name = supplier.Name,
            //                            PhoneNumber = supplier.PhoneNumber,
            //                            SupplierImage = "/Images/Users/" + supplier.Image,
            //                            SupplierImageExtension = supplier.ImageExtension,
            //                            IsSupplierVerified = suppliersCorp.IsVerified
            //                        });
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    result.DataResult = suppliers;
            //}
            result.DataResult = suppliers;
            if (suppliers.Count <= 0)
            {
                result.Code = ResponseCode.NoSuppliersFound;
                result.Message = "there is no suppliers";
            }
            return result;
        }
        public Response<List<SubCategoriesVM>> GetSubCategories(int id)
        {
            Response<List<SubCategoriesVM>> result = new Response<List<SubCategoriesVM>>();
            try
            {
                if (db.FWYCategory.Where(c => c.ID == id && c.IsDeleted == false).Count() == 0)
                {
                    result.Code = ResponseCode.NoCategoryFound;
                    result.Message = "This category is deleted";
                    return result;
                }
                List<SubCategoriesVM> subcategories = db.FWYCategory.Where(c => c.CategoryID == id && c.IsDeleted == false).OrderBy(c => c.Number).ThenBy(c => c.ID).Select(c => new SubCategoriesVM
                {
                    Id = c.ID,
                    Name = c.Name,
                    ArName = c.ARName,
                    Image = "/Images/Categories/" + c.Image
                }).ToList();
                if(subcategories == null || subcategories.Count == 0)
                {
                    result.Code = ResponseCode.NoSuppliersFound;
                    result.Message = "There are no sub categories for this category";
                    return result;
                }
                result.DataResult = subcategories;
                result.Code = ResponseCode.Success;
            }
            catch(Exception ex)
            {

            }
            return result;
        }
        public List<CategoriesHomeVM> GetCategories()
        {
            List<CategoriesHomeVM> categories = new List<CategoriesHomeVM>();
            categories = db.FWYCategory.Include(c=>c.FWYProduct)
                .Where(c=>c.IsDeleted != true && c.CategoryID == null).OrderBy(c => c.Number).ThenBy(c => c.ID).Select(c=> new CategoriesHomeVM 
                {
                    catID = c.ID,
                    Name = c.Name,
                    ArName = c.ARName,
                    Image = "/Images/Categories/" + c.Image,
                    TotalProduct = c.FWYProduct.Count
                }).ToList();

            return categories;
        }

    }
}