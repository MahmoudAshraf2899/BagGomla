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
    [RoutePrefix("api/WishList")]
    public class WishListController : ApiController
    {
        WishListPageAPIService service = new WishListPageAPIService(); 
        [Route("Add/{productId}")]
        [HttpPost]
        //[Authorize]
        public HttpResponseMessage AddTowishList(int productId,string userId)
        {
            Response response = service.AddtoWishList(productId,userId);

            if(response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }



        [Route("Show")]
        [HttpGet]
        //[Authorize]
        public HttpResponseMessage getwishList(string userId)
        {
            Response<List<ProductVM>> response = service.getWishList(userId);

            if (response.Code == Enums.ResponseCode.Success)

                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }


        [Route("Delete/{productId}")]
        [HttpPost]
        //[Authorize]
        public HttpResponseMessage DeletefromWishList(int productId, string userId)
        {
            Response response = service.DeleteItemFromWishList(productId,userId);

            if (response.Code == Enums.ResponseCode.Success)
                return Request.CreateResponse(HttpStatusCode.OK, response);

            return Request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

    }
}
