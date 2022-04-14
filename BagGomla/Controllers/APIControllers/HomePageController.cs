using BagGomla.Business;
using BagGomla.Enums;
using BagGomla.Helper;
using BagGomla.Models;
using BagGomla.ViewModel;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace BagGomla.Controllers.APIControllers
{
    [RoutePrefix("api/Home")]
    public class HomePageController : ApiController
    {
        HomePageApiService service = new HomePageApiService();
        private Response ResponseVM = new Response();

        //to get the Home Page Content
        //api/Home/HomeContent
        [Route("HomeContent")]
        [HttpGet]
        public HttpResponseMessage HomeContect (int page = 1)
        {
            HomePageVM homePage = service.homePage(page);
            if (homePage == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
            }
            return  Request.CreateResponse(HttpStatusCode.OK, homePage);
        }

        [Route("Search/{productName}")]
        [HttpGet]
        public HttpResponseMessage Search(string productName, string currentUserId, int page = 1)
        {
            List<ProductVM> searchResult = service.Search(productName, currentUserId, page);
            if (searchResult == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ResponseVM);
            }
            return Request.CreateResponse(HttpStatusCode.OK, searchResult);
        }

    }
}
