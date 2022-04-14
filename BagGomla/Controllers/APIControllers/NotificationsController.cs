using BagGomla.Business;
using BagGomla.ViewModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Notifications")]
    public class NotificationsController : ApiController
    {
        [HttpGet]
        [Authorize]
        [Route("GetMyNotificataion")]
        public HttpResponseMessage GetMyNotificataion()
        {
            NotificationsAppService appService = new NotificationsAppService();
            Response result = appService.GetMyNotifications();
            if (result.IsSccuessCode)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, result);
        }
    }
}