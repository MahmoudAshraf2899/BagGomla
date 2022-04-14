using BagGomla.Attributes;
using BagGomla.Business;
using BagGomla.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Suppliers")]
    public class SuppliersController : ApiController
    {

        SuppliersPageAPIService service = new SuppliersPageAPIService();
        [HttpPost]
        [CustomAuthorize(Role = UserRole.Supplier)]
        [Route("AddProduct")]
        public HttpResponseMessage AddProduct(AddProductViewModel model)
        {
            ProductPageAPIService appService = new ProductPageAPIService();
            Response result = appService.AddProduct(model);
            if (result.IsSccuessCode)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }


        [HttpGet]
        [Route("{id}/GetSupplierProducts")]
        public HttpResponseMessage GetSupplierProducts(int id,string currentUserId = "", int page = 1)
        {
            Response result = service.GetSupplierProducts(id, currentUserId, page);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }

        //   api/Subcategories/1
        [HttpGet]
        [Route("Getall/{page}")]
        public HttpResponseMessage getSuppliers(int page)
        {
            Response<List<SuppliersVM>> response = service.getAllSuppliers(page);

            if (response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

        [Route("supplierStores/{id}")]
        [HttpGet]
        public HttpResponseMessage getsupplierStores(string id)
        {
            Response<List<SupplierStoreVM>> response = service.getSupplierStores(id);

            if (response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }


        [HttpGet]
        [Route("filterSuppliers")]
        public HttpResponseMessage filterSuppliers(int? productsNum, bool? isVerified, int? categoryId, int? subCategoryId, int page = 1)
        {
            Response<List<SuppliersVM>> response = service.filterSuppliers(productsNum, isVerified, categoryId, subCategoryId, page);

            if (response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

    }
}
