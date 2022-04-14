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
    [RoutePrefix("api/FavSupplier")]
    public class FavSupplierController : ApiController
    {
        FavSupplierAPIService service = new FavSupplierAPIService(); 
        [Route("Add")]
        [HttpPost]
        //[Authorize]
        public HttpResponseMessage AddToFavSupplier(string supId,string userId)
        {
            Response response = service.AddtoFavSupplier(supId,userId);

            if(response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }



        [Route("Show")]
        [HttpGet]
        //[Authorize]
        public HttpResponseMessage getFavSupplier(string userId)
        {
            Response<List<SuppliersVM>> response = service.getFavSupplier(userId);

            if (response.Code == Enums.ResponseCode.Success)

                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }


        [Route("Delete")]
        [HttpPost]
        //[Authorize]
        public HttpResponseMessage DeletefromFavSupplier(string supId, string userId)
        {
            Response response = service.DeleteItemFromFavSupplier(supId,userId);

            if (response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

    }
}
