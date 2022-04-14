using BagGomla.Business;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Product")]
    public class ProductController : ApiController
    {
        ProductPageAPIService service = new ProductPageAPIService();
        [Route("Details/{id}")]
        [HttpGet]
        public HttpResponseMessage getProductDetails(int id)
        {
            Response productDetails = service.productDetials(id);

            if (productDetails == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            return Request.CreateResponse(HttpStatusCode.OK, productDetails);

        }

        [Route("getProdcutwithType/{id}")]
        [HttpGet]
        public HttpResponseMessage getNationalProduct(int id)
        {
            Response<List<ProductVM>> result = service.getNationalProducts(id);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);

        }

        [Route("getPageProducts")]
        [HttpGet]
        public HttpResponseMessage getPageProducts(int page = 1, int? categoryId = null)
        {
            var result = service.getPageProducts(page, categoryId);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result.DataResult);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result.DataResult);
        }

        private string GetBaseUrl()
        {
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                             HttpContext.Current.Request.ApplicationPath.TrimEnd('/') + "/";
            return baseUrl;
        }

        [Route("myProducts")]
        [HttpGet]
        public HttpResponseMessage SupplierProducts(string SupplierId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetBaseUrl() + "Products/SupplierProducts?SupplierId=" + SupplierId);
        }

        [Route("addProduct")]
        [HttpGet]
        public HttpResponseMessage AddProduct(string SupplierId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetBaseUrl() + "Products/CreateProduct?SupplierId=" + SupplierId);
        }


        [Route("editProduct")]
        [HttpGet]
        public HttpResponseMessage EditProduct(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetBaseUrl() + "Products/EditProduct/" + id);
        }

        [Route("deleteProduct")]
        [HttpGet]
        public HttpResponseMessage DeleteProduct(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetBaseUrl() + "Products/DeleteProduct/" + id);
        }

        [Route("productDetails")]
        [HttpGet]
        public HttpResponseMessage productDetails(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, GetBaseUrl() + "Products/ProductDetails/" + id);
        }

        [Route("filterProducts")]
        [HttpGet]
        public HttpResponseMessage FilterProducts(int page, int? subcategoryid, int? categoryid, decimal? fromprice, decimal? toprice, string ordertype, bool? orderbylessquantitygomala, int? lessQuantityGomla, int? countryId, string searchStr = "", string currentUserId = "")
        {
            var result = service.filterProducts(page, subcategoryid, categoryid, fromprice, toprice, ordertype, orderbylessquantitygomala, lessQuantityGomla, countryId, searchStr, currentUserId);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result.DataResult);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result.DataResult);
        }
    }
}