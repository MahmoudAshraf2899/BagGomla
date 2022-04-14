using BagGomla.Attributes;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    [CustomAuthorize(Role = UserRole.Admin)]
    public class TermsAndConditionController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        // GET: TermsAndCondition
        public ActionResult Index()
        {
            var model = new FWYAbout();
            if(db.FWYAbout != null && db.FWYAbout.Count() > 0)
            {
                model = db.FWYAbout.FirstOrDefault();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FWYAbout model)
        {
            if(model.ID > 0)
            {
                var item = db.FWYAbout.Find(model.ID);
                item.TermsAnConditionsAr = model.TermsAnConditionsAr;
                item.TermsAnConditionsEn = model.TermsAnConditionsEn;
            }
            else
            {
                db.FWYAbout.Add(model);
            }
            db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


    }
}