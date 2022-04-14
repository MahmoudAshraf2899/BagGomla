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
    [RoutePrefix("api/FacadeImage")]
    public class FacadeImageController : ApiController
    {
        FacadeImageAppService service = new FacadeImageAppService();

        [HttpGet]
        [AllowAnonymous]
        [Route("Get")]
        public HttpResponseMessage GetAllAds()
        {
            return Request.CreateResponse(HttpStatusCode.OK, service.GetAll());
        }

    }
}
