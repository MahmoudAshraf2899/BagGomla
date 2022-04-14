using BagGomla.Enums;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BagGomla.ViewModel;
using Microsoft.AspNet.Identity;
using System.Data.Entity;

namespace BagGomla.Business
{
    public class ProductPageAPIService
    {
        DatabaseContext db = new DatabaseContext();
        public Response<ProductDetailsVM> productDetials(int id)
        {
            Response<ProductDetailsVM> result = new Response<ProductDetailsVM>();
            ProductDetailsVM productDetails = new ProductDetailsVM();
            FWYProduct product = db.FWYProduct
                .Include("FWYProductPriceRange")
                .Include("FWYProductImage")
                .Include("FWYCountry")
                .Where(p => p.ID == id).FirstOrDefault();
            if (product != null)
            {
                productDetails.Id = product.ID;
                productDetails.Name = product.Name == "" ? "" : product.Name;
                productDetails.arName = product.ARName == "" ? "" : product.ARName;
                productDetails.Price = product.Price ?? 0;
                productDetails.Description = product.ARDescription == "" ? "" : product.ARDescription;
                productDetails.minPeices = (int)product.LessQuantityGomla;
                productDetails.Country = product.FWYCountry != null ? product.FWYCountry.ArName : "";
                if (product.FWYProductImage.Count > 0)
                {
                    productDetails.productImages = new List<string>();
                    foreach (var item in product.FWYProductImage)
                    {
                        if (item.Image != "" && item.Image != null)
                        {
                            productDetails.productImages.Add("/Images/Products/" + item.Image);
                        }
                    }
                }
                if (product.FWYProductPriceRange.Count > 0)
                {
                    productDetails.minPrice = (decimal)product.FWYProductPriceRange.Min(c => c.Price);
                    productDetails.maxPrice = (decimal)product.FWYProductPriceRange.Max(c => c.Price);
                }
                productDetails.Name = product.Name == "" ? "" : product.Name;
                productDetails.Price = product.Price > 0 ? product.Price : 0;
                productDetails.arName = product.ARName == "" ? "" : product.ARName;
                productDetails.Description = product.ARDescription == "" ? "" : product.ARDescription;

                //to get the sppliername 
                FWYStoreProduct storeProduct = db.FWYStoreProduct.Include("FWYStore")
                                              .FirstOrDefault(s => s.ProductID == product.ID);

                //to get Company Name '
                FWYSupplierCooperation Company = new FWYSupplierCooperation();
                if (storeProduct != null)
                {
                    if (storeProduct.FWYStore != null)
                    {
                        Company = db.FWYSupplierCooperation.FirstOrDefault(u => u.SupplierID == storeProduct.FWYStore.SupplierID);
                        if (Company != null)
                        {
                            productDetails.CompanyName = Company.Name;
                            productDetails.arCompanyName = Company.ArName;
                            productDetails.SupplierId = Company.ID;
                            productDetails.IsSupllierVerified = Company.IsVerified;
                            var user = db.AspNetUsers.Find(Company.SupplierID);
                            if (user != null)
                            {
                                productDetails.Latitude = (decimal)user.Latitude;
                                productDetails.Longtitude = (decimal)user.Longitude;
                                productDetails.SupplierPhone = user.PhoneNumber;
                                productDetails.UserId = user.Id;
                            }
                        }

                    }
                }

            }
            result.DataResult = productDetails;
            result.Code = ResponseCode.Success;
            return result;
        }

        public Response AddProduct(AddProductViewModel model)
        {
            Response result = new Response();
            try
            {
                Response valid = ValidateProduct(model);
                if (!valid.IsSccuessCode)
                {
                    return valid;
                }
                Response<int> productIsCreated = CreateProduct(model);
                if (!productIsCreated.IsSccuessCode)
                {
                    return productIsCreated;
                }
                string currentuserId = HttpContext.Current.User.Identity.GetUserId();
                Response linkedToSupplierStore = LinkToStoreProduct(productIsCreated.DataResult, currentuserId, model);
                if (!productIsCreated.IsSccuessCode)
                {
                    return productIsCreated;
                }
                result.Code = ResponseCode.Success;
                result.Message = "Product Is Added successfully";
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        private Response LinkToStoreProduct(int productId, string supplierId, AddProductViewModel model)
        {
            Response result = new Response();
            FWYStore store = db.FWYStore.FirstOrDefault(s => s.SupplierID == supplierId && s.IsDeleted == false);
            FWYStoreProduct storeProduct = new FWYStoreProduct
            {
                IsDeleted = false,
                ProductID = productId,
                Quantity = model.WholesaleQuantity,
                SupplierID = supplierId,
                StoreID = store?.ID
            };
            db.FWYStoreProduct.Add(storeProduct);
            db.SaveChanges();
            result.Code = ResponseCode.Success;
            return result;
        }

        private Response ValidateProduct(AddProductViewModel model)
        {
            Response result = new Response();
            if (string.IsNullOrWhiteSpace(model.ProductName))
            {
                result.Message = "ProductName Is Not Provided";
                return result;
            }
            result.Code = ResponseCode.Success;
            return result;
        }
        private Response<int> CreateProduct(AddProductViewModel model)
        {
            Response<int> result = new Response<int>();
            FWYProduct product = new FWYProduct
            {
                Country = model.Country,
                ArCountry = model.Country,
                CategoryID = model.CategoryId,
                ARName = model.ProductName,
                ARDescription = model.Description,
                Description = model.Description,
                WholesalePrice = model.WholesalePrice,
                FinalPrice = model.ConsumerPrice,
                LessQuantityGomla = model.WholesaleQuantity,
                FWYProductImage = new List<FWYProductImage> { new FWYProductImage { Image = model.Image, ImageExtension = model.ImageExtension, IsMain = true, IsDeleted = false } }
            };
            db.FWYProduct.Add(product);
            if (db.SaveChanges() <= 0)
            {
                result.Code = ResponseCode.CannotCreateProduct;
                result.Message = "Cannot Create Product";
                return result;
            }
            result.DataResult = product.ID;
            result.Code = ResponseCode.Success;
            return result;
        }

        public Response<List<ProductVM>> getNationalProducts(int typeId)
        {
            Response<List<ProductVM>> responseData = new Response<List<ProductVM>>();
            responseData.DataResult = new List<ProductVM>();

            List<FWYProduct> products = db.FWYProduct
                 .Include("FWYProductPriceRange")
                 .Include("FWYProductImage")
                .Where(p => p.TypeID == typeId).Take(5).ToList();

            if (products.Count > 0)
            {
                //to check if the product in the wihlist or not 
                string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
                FWYWishList ProductinWishList = new FWYWishList();
                foreach (var item in products)
                {
                    //to get the sppliername 
                    FWYStoreProduct storeProduct = db.FWYStoreProduct.Include("FWYStore")
                      .Where(s => s.ProductID == item.ID).FirstOrDefault();
                    //to get Seller Name '
                    AspNetUsers user = new AspNetUsers();
                    FWYSupplierCooperation Company = new FWYSupplierCooperation();
                    if (storeProduct != null)
                    {
                        if (storeProduct.FWYStore != null)
                        {
                            user = db.AspNetUsers.FirstOrDefault(u => u.Id == storeProduct.FWYStore.SupplierID);
                            Company = db.FWYSupplierCooperation.FirstOrDefault(u => u.SupplierID == storeProduct.FWYStore.SupplierID);

                        }
                    }
                    //to check if the product in the wihlist or not 
                    if (CurrentUserid != "")
                    {
                        ProductinWishList = db.FWYWishList.FirstOrDefault(w => w.UserID == CurrentUserid && w.ProductID == item.ID);
                    }
                    responseData.DataResult.Add(new ProductVM
                    {
                        ID = item.ID,
                        ARName = item.ARName == null ? "" : item.ARName,
                        Name = item.Name == null ? "" : item.Name,
                        isFavorite = ProductinWishList == null ? false : true,
                        PhoneNumber = user != null ? user.PhoneNumber : "",
                        Price = item.Price > 0 ? item.Price : 0,
                        MinAmount = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? 0 : item.FWYProductPriceRange.Min(x => x.FromQuantity),
                        Min = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Min(c => c.Price)),
                        Max = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Max(c => c.Price)),
                        SupplierName = user == null ? "" : user.Name,
                        CpmanyName = Company == null ? "" : Company.Name,
                        ArCpmanyName = Company == null ? "" : Company.ArName,
                        Country = item.Country != "" ? item.Country : "",
                        ArCountry = item.ArCountry != "" ? item.ArCountry : "",
                        Image = item.FWYProductImage.Count == 0 ? "" : item.FWYProductImage.FirstOrDefault().Image,
                        ImageList = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? null : item.FWYProductImage.Select(c =>
                        {
                            string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                            return imgPath;
                        }).ToList(),
                        IsSupplierVerified = Company.IsVerified
                    });
                }

                if (responseData.DataResult.Count > 0)
                    responseData.Code = ResponseCode.Success;
            }

            return responseData;
        }

        public Response<ProductPagination> getPageProducts(int page, int? categoryId, string currentUserId = "")
        {
            Response<ProductPagination> responseData = new Response<ProductPagination>();
            responseData.DataResult = new ProductPagination()
            {
                ProductList = new List<ProductVM>()
            };
            var products = new List<FWYProduct>();
            if (categoryId != null)
            {
                products = db.FWYProduct.Where(p => p.IsDeleted != true && p.CategoryID == categoryId)
                           .Include("FWYProductPriceRange")
                           .Include("FWYBrand")
                           .Include("FWYProductImage")
                           .Include("FWYStoreProduct")
                           .Include("FWYCountry").ToList();
            }
            else
            {
                products = db.FWYProduct.Where(p => p.IsDeleted != true)
                           .Include("FWYProductPriceRange")
                           .Include("FWYBrand")
                           .Include("FWYProductImage")
                           .Include("FWYStoreProduct")
                           .Include("FWYCountry").ToList();
            }

            float xx = (float)products.Count / 20;
            int mm = products.Count / 20;
            if (xx > ((float)mm))
            {
                mm += 1;
            }
            responseData.DataResult.PageCount = mm;
            responseData.DataResult.PageNumber = page;
            responseData.DataResult.AllItemsCount = products.Count;
            products = products.OrderByDescending(c => c.ID).Skip((page - 1) * 20).Take(20).ToList();
            if (products.Count > 0)
            {
                //to check if the product in the wihlist or not 
                string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
                if (CurrentUserid == "" || CurrentUserid == null)
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

                    responseData.DataResult.ProductList.Add(new ProductVM
                    {
                        ID = item.ID,
                        ARName = item.ARName,
                        Name = item.Name,
                        isFavorite = ProductinWishList != null && ProductinWishList.Id > 0 ? true : false,
                        PhoneNumber = phoneNumber,
                        SupplierName = CompanyName,
                        Price = item.Price ?? 0,
                        MinAmount = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? 0 : item.FWYProductPriceRange.Min(x => x.FromQuantity),
                        ImageList = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? null : item.FWYProductImage.Select(c =>
                        {
                            string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                            return imgPath;
                        }).ToList(),
                        latitude = lati,
                        longtitude = longi,
                        LessQuantityGomla = item.LessQuantityGomla,
                        IsSupplierVerified = isSupplierVerified,
                        Country = item.Country != "" ? item.Country : "",
                        ArCountry = item.ArCountry != "" ? item.ArCountry : "",
                        Min = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Min(c => c.Price)),
                        Max = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Max(c => c.Price)),
                    });
                }
            }
            responseData.Code = ResponseCode.Success;
            return responseData;
        }

        public Response<ProductPagination> filterProducts(int page, int? subcategoryid, int? categoryid, decimal? fromprice, decimal? toprice, string ordertype, bool? orderbylessquantitygomala, int? lessQuantityGomla, int? countryId, string searchStr, string currentUserId)
        {
            Response<ProductPagination> responseData = new Response<ProductPagination>();
            responseData.DataResult = new ProductPagination()
            {
                ProductList = new List<ProductVM>()
            };
            string query = "select * from FWYProduct where IsDeleted  = 0";
            var products = new List<FWYProduct>();
            //products = db.FWYProduct.Where(p => p.IsDeleted != true)
            //               .Include("FWYProductPriceRange")
            //               .Include("FWYBrand")
            //               .Include("FWYProductImage")
            //               .Include("FWYStoreProduct")
            //               .Include("FWYCountry").ToList();

            if (subcategoryid != null)
            {
                //products = products.Where(c => c.CategoryID == subcategoryid).ToList();
                query += " and CategoryID = " + subcategoryid;
            }
            if (categoryid != null)
            {
                var childs = db.FWYCategory.Where(c => c.CategoryID == categoryid);
                //products = products.Where(c => childs.Where(x => x.ID == c.CategoryID).Count() > 0).ToList();
                var strSubIds = "(0";
                foreach (var item in childs)
                {
                    strSubIds += ", " + item.ID;
                }
                strSubIds += ")";
                query += " and CategoryID in " + strSubIds;
            }

            if (fromprice != null)
            {
                //products = products.Where(c => c.FinalPrice >= fromprice).ToList();
                query += " and Price >= " + fromprice;
            }
            if (toprice != null)
            {
                //products = products.Where(c => c.FinalPrice <= toprice).ToList();
                query += " and Price <= " + toprice;
            }

            if(lessQuantityGomla != null)
            {
                query += " and LessQuantityGomla <= " + lessQuantityGomla;
            }

            if(countryId != null)
            {
                query += " and CountryID = " + countryId;
            }

            if (ordertype != null && orderbylessquantitygomala != null)
            {
                if (ordertype == "asc")
                {
                    //products = products.OrderBy(c => c.LessQuantityGomla).ToList();
                    query += " order by LessQuantityGomla";
                }
                else
                {
                    //products = products.OrderByDescending(c => c.LessQuantityGomla).ToList();
                    query += " order by LessQuantityGomla desc";
                }
            }

            var all = db.FWYProduct.SqlQuery(query).ToList();

            if (searchStr != null && searchStr != "")
            {
                searchStr = searchStr.ToLower();
                all = all.Where(c => c.Name.ToLower().Contains(searchStr) || c.ARName.ToLower().Contains(searchStr)).ToList();
            }

            float xx = (float)all.Count / 20;
            int mm = all.Count / 20;
            if (xx > ((float)mm))
            {
                mm += 1;
            }

            products = all.Skip((page - 1) * 20).Take(20).ToList();

            responseData.DataResult.PageCount = mm;
            responseData.DataResult.PageNumber = page;
            responseData.DataResult.AllItemsCount = all.Count();
            products = products.Skip((page - 1) * 20).Take(20).ToList();
            
            if (products != null && products.Count > 0)
            {
                //to check if the product in the wihlist or not 
                //string CurrentUserid = HttpContext.Current.User.Identity.GetUserId();
                FWYWishList ProductinWishList = new FWYWishList();
                responseData.DataResult.ProductList = new List<ProductVM>();
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

                    if (currentUserId != "" && currentUserId != null)
                    {
                        ProductinWishList = db.FWYWishList.FirstOrDefault(w => w.UserID == currentUserId && w.ProductID == item.ID);
                    }

                    try
                    {
                        var pro = new ProductVM();
                        pro.ID = item.ID;
                        pro.ARName = item.ARName;
                        pro.Name = item.Name;
                        pro.isFavorite = ProductinWishList != null && ProductinWishList.Id > 0 ? true : false;
                        pro.PhoneNumber = phoneNumber;
                        pro.SupplierName = CompanyName;
                        pro.Price = item.Price ?? 0;
                        pro.MinAmount = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? 0 : item.FWYProductPriceRange.Min(x => x.FromQuantity);
                        pro.Min = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Min(c => c.Price));
                        pro.Max = item.FWYProductPriceRange == null || item.FWYProductPriceRange.Count() <= 0 ? "0" : String.Format("{0:0.00}", (decimal)item.FWYProductPriceRange.Max(c => c.Price));
                        pro.ImageList = item.FWYProductImage == null || item.FWYProductImage.Count() <= 0 ? null : item.FWYProductImage.Select(c =>
                        {
                            string imgPath = c.Image == null ? "" : "/Images/Products/" + c.Image;
                            return imgPath;
                        }).ToList();
                        pro.latitude = lati;
                        pro.longtitude = longi;
                        pro.LessQuantityGomla = item.LessQuantityGomla;
                        pro.IsSupplierVerified = isSupplierVerified;
                        pro.Country = item.FWYCountry != null ? item.FWYCountry.Name : "";
                        pro.ArCountry = item.FWYCountry != null ? item.FWYCountry.ArName : "";
                        responseData.DataResult.ProductList.Add(pro);
                    }
                    catch (Exception e)
                    {
                        var m = e.Message;
                    }
                }
            }
            responseData.Code = ResponseCode.Success;
            return responseData;
        }


    }

    public class ProductPagination
    {
        public int PageCount { get; set; }
        public int PageNumber { get; set; }
        public int AllItemsCount { get; set; }
        public List<ProductVM> ProductList { get; set; }
    }
}