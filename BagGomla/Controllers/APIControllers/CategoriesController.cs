using BagGomla.Business;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Categories")]
    public class CategoriesController : ApiController
    { 
        CategoriesPageAPIService service = new CategoriesPageAPIService();
       
        [Route("getCategories")]
        [HttpGet]
        public HttpResponseMessage getCategories()
        {
            List<CategoriesHomeVM> categories = service.GetCategories();
            if(categories.Count == 0)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            return Request.CreateResponse(HttpStatusCode.OK, categories);
        }

        [Route("{id}/Suppliers")]
        [HttpGet]
        public HttpResponseMessage getSuppliersDependOnCategories(int id, int page = 1)
        {
            return Request.CreateResponse(HttpStatusCode.OK, service.SuppliersInCategories(id,page));
        }

        [Route("{id}/getSubCategories")]
        [HttpGet]
        public HttpResponseMessage getSubCategories(int id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, service.GetSubCategories(id));
        }
    }
}
