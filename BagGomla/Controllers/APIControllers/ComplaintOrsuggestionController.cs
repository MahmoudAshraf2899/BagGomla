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
    [RoutePrefix("api/ComplaintOrsuggestion")]
    public class ComplaintOrsuggestionController : ApiController
    {
        ComplaintOrsuggestionPageAPIService service = new ComplaintOrsuggestionPageAPIService();

        [Route("Add")]
        [HttpPost]
        [Authorize]
        public HttpResponseMessage AddReview(AddComplaintOrsuggestionVM model)
        {
            Response result = service.AddComplaintOrsuggestion(model);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }


        [Route("MakeRead/{ComplaintOrsuggestionId}")]
        [HttpPost]
      
        public HttpResponseMessage MakeRead(int ComplaintOrsuggestionId)
        {
            Response result = service.MakeSuggestionRead(ComplaintOrsuggestionId);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }


       [Route("getall")]
       [HttpGet]
       public HttpResponseMessage GetAll()
        {
            Response<List<ComplaintOrsuggestionVM>> result = service.getComplaint();
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }


    }
}
