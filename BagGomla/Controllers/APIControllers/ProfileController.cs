using BagGomla.Attributes;
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
    [RoutePrefix("api/Profile")]
    public class ProfileController : ApiController
    {
        ProfileAppService service = new ProfileAppService();
        [Route("Details")]
        [HttpGet]
        [CustomAuthorize]
        public HttpResponseMessage getProfileDetails()
        {
            Response productDetails = service.GetMyProfileInfo();
            if(productDetails == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            return Request.CreateResponse(HttpStatusCode.OK , productDetails);
        }

        [Route("UpdateInfo")]
        [HttpPost]
        [CustomAuthorize]
        public HttpResponseMessage UpdateInfo(UpdateProfileViewModel model)
        {
            Response result = service.UpdateMyProfileInfo(model);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }
        [Route("UpdatePicture")]
        [HttpPost]
        [CustomAuthorize]
        public HttpResponseMessage UpdatePicture(UpdateProfilePictureViewModel model)
        {
            Response result = service.UpdateMyProfilePicture(model);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }
        [Route("UpdateNantiolId")]
        [HttpPost]
        [CustomAuthorize]
        public HttpResponseMessage UpdateNantiolId(UpdateNationalPictureViewModel model)
        {
            Response result = service.UpdateMyNationalId(model);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }
        [Route("UpdateCommercialCertificate")]
        [HttpPost]
        [CustomAuthorize]
        public HttpResponseMessage UpdateCommercialCertificate(UpdateCommercialCertificateViewModel model)
        {
            Response result = service.UpdateMyCommercialCertificate(model);
            if (result.IsSccuessCode)
                return Request.CreateResponse(HttpStatusCode.OK, result);
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }

    }
}