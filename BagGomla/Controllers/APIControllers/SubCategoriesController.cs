using BagGomla.Business;
using BagGomla.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Subcategories")]
    public class SubCategoriesController : ApiController
    {
        SubCategoriesPageAPIService subCategoryService = new SubCategoriesPageAPIService();

        [HttpGet]
        [Route("{subCategoryId}/getProducts")]
        public HttpResponseMessage GetProducts(int subCategoryId,int page = 1)
        {
            Response result = subCategoryService.getProducts(subCategoryId,page);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }
    }
}
