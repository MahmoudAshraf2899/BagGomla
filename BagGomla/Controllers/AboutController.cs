using BagGomla.Helper;
using BagGomla.Models;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace BagGomla.Controllers
{
    public class AboutController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private ApplicationUserManager _userManager;
        private TagHelper helper = new TagHelper();

        public AboutController()
        {
        }

        public AboutController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        public ActionResult About()
        {
            List<FWYAbout> about = new List<FWYAbout>();

            if (db.FWYAbout.Count() > 0)
            {
                about = db.FWYAbout.ToList();
            }
            foreach (var item in about)
            {
                item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            }
            return View(about);
        }


        [HttpGet]
        public ActionResult ContentPage()
        {
            FWYContect FWYContect = new FWYContect();
            FWYContect = db.FWYContect.FirstOrDefault();
            return View(FWYContect);
        }

        [HttpPost]
        public ActionResult ContentPage(FWYContect FWYContect, string From, string ComplaintOrsuggestion)
        {
            if (ModelState.IsValid)
            {
                string subject = "<b>" + From + "</b><br/>" + "Complaints and suggestions";
                string body = ComplaintOrsuggestion;
                EmailManager.SendEmail(From, subject, body, FWYContect.Email, FWYContect.Email, FWYContect.Password, "587", "smtp.gmail.com");
                FWYComplaintOrsuggestion FWYComplaintOrsuggestion = new FWYComplaintOrsuggestion()
                {
                    EmailFrom = From,
                    EmailTo = FWYContect.Email,
                    ComplaintOrsuggestion = ComplaintOrsuggestion,
                    SentDateTime = DateTime.Now
                };
                db.FWYComplaintOrsuggestion.Add(FWYComplaintOrsuggestion);
                db.SaveChanges();
                return Json(1);
            }
            else
            {
                return Json(2);
            }
        }


    }
}