using System.Linq;
using System.Web.Mvc;
using BagGomla.Models;
using IdentityLibrary.DataModel;
using BagGomla.Helper;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    public class LatestNewsController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();

        [HttpGet]
        public ActionResult Index()
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var LatestNews = db.LatestNews.OrderByDescending(c => c.ID).ToList();
                return View(LatestNews);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }
        }

        [HttpGet]
        public ActionResult EditCreateLatestNews(int ID)
        {
            var LatestNews = new LatestNews();
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ID > 0)
                    LatestNews = db.LatestNews.Find(ID);

                return View(LatestNews);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Account");
            }

        }

        [HttpPost]
        public ActionResult EditCreateLatestNews(LatestNews LatestNews)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);

            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if (LatestNews.Name == "" || LatestNews.Name == null)
                        LatestNews.Name = LatestNews.ArName;

                    if (LatestNews.ID > 0)
                    {
                        var item = db.LatestNews.SingleOrDefault(c => c.ID == LatestNews.ID);
                        item.Name = LatestNews.Name;
                        item.ArName = LatestNews.ArName;
                    }
                    else
                        db.LatestNews.Add(LatestNews);

                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
                return RedirectToAction("AccessDenied", "Account");
        }

        [HttpGet]
        public ActionResult DeleteLatestNews(int id)
        {
            ConfirmDeleteModel model = new ConfirmDeleteModel()
            {
                ControllerName = "LatestNews",
                ActionName = "DeleteLatestNews",
                IntID = id
            };
            return PartialView("_DeleteView", model);
        }

        [HttpPost]
        [ActionName("DeleteLatestNews")]
        public ActionResult ConfirmDeleteLatestNews(int ID)
        {
            var Del = db.LatestNews.Find(ID);
            db.LatestNews.Remove(Del);
            db.SaveChanges();
            return helper.SweetAlert("تم", "تم الحذف بنجاح", (int)SweetAlertType.success, "/LatestNews");
        }
    }
}