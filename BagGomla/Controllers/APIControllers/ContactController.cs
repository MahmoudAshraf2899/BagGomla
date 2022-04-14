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
    [RoutePrefix("api/Contact")]
    public class ContactController : ApiController
    {
        ContactPageAPIService service = new ContactPageAPIService();
        [Route("get")]
        [HttpGet]
        public HttpResponseMessage ShowContactInfo()
        {
            return Request.CreateResponse(HttpStatusCode.OK, service.getContactInfo());
        }
    }
}
