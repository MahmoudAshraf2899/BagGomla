using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using BagGomla.Helper;
using static BagGomla.Models.Enum;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using BagGomla.Attributes;
using PagedList;
using PagedList.Mvc;

namespace BagGomla.Controllers
{
    [CustomAuthorize(Role = UserRole.Admin)]
    public class NotificationController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();
        private ApplicationUserManager _userManager;
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
        // GET: Notification
        public ActionResult Index()
        {
            var fWYNotification = db.FWYNotification.Where(c => c.IsManual == true).OrderByDescending(c => c.Id);
            foreach (var item in fWYNotification)
            {
                item.Image = helper.ConnvertToImageSrc(item.Image, item.ImageExtension);
            }
            return View(fWYNotification.ToList());
        }

        // GET: Notification/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FWYNotification fWYNotification = db.FWYNotification.Find(id);
            if (fWYNotification == null)
            {
                return HttpNotFound();
            }
            fWYNotification.Image = helper.ConnvertToImageSrc(fWYNotification.Image, fWYNotification.ImageExtension);
            return View(fWYNotification);
        }

        // GET: Notification/Create
        public async Task<ActionResult> Create()
        {
            string role = ((int)UserRole.Admin).ToString();
            string userId = User.Identity.GetUserId();
            bool IsInRole = await UserManager.IsInRoleAsync(userId, role);
            if (IsInRole)
            {
                return View();
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        // POST: Notification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FWYNotification fWYNotification, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    var fileData = helper.ConvertFileToBase64(Image);
                    fWYNotification.Image = fileData.FileBase64;
                    fWYNotification.ImageExtension = fileData.FileExtension;
                }
                fWYNotification.IsManual = true;
                db.FWYNotification.Add(fWYNotification);
                db.SaveChanges();
                if (fWYNotification.IsRead == true)
                {
                    DeactiveNoti(fWYNotification.Id);
                }
                return RedirectToAction("Index");
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        public void DeactiveNoti(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                foreach (var item in db.FWYNotification.Where(c => c.IsManual == true && c.Id != ID).ToList())
                {
                    item.IsRead = false;
                    db.SaveChanges();
                }
            }            
        }

        public void ActiveNoti(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var noti = db.FWYNotification.SingleOrDefault(c => c.Id == ID);
                noti.IsRead = true;
                db.SaveChanges();
            }
        }

        public ActionResult ActiveNotification(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                ActiveNoti(ID);
                DeactiveNoti(ID);
                return RedirectToAction("Index");
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }
        // GET: Notification/Edit/5
        public ActionResult Edit(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                FWYNotification fWYNotification = db.FWYNotification.Find(ID);
                return View(fWYNotification);
            }
            return RedirectToAction("AccessDenied", "Account");
           
        }

        // POST: Notification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FWYNotification fWYNotification, HttpPostedFileBase Image)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    var noti = db.FWYNotification.SingleOrDefault(c => c.Id == fWYNotification.Id);
                    noti.Title = fWYNotification.Title;
                    noti.Details = fWYNotification.Details;
                    noti.IsRead = fWYNotification.IsRead;
                    noti.ArTitle = fWYNotification.ArTitle;
                    noti.ArDetails = fWYNotification.ArDetails;
                    if (Image != null)
                    {
                        var fileData = helper.ConvertFileToBase64(Image);
                        noti.Image = fileData.FileBase64;
                        noti.ImageExtension = fileData.FileExtension;
                    }

                    db.SaveChanges();
                    if (fWYNotification.IsRead == true)
                    {
                        DeactiveNoti(fWYNotification.Id);
                    }
                    return RedirectToAction("Index");
                }
                return View(fWYNotification);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        // GET: Notification/Delete/5
        public ActionResult Delete(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                FWYNotification fWYNotification = db.FWYNotification.Find(ID);
                db.FWYNotification.Remove(fWYNotification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [Authorize]
        public ActionResult UserNotifications(int page = 1)
        {
            var UserID = User.Identity.GetUserId();
            var fWYNotification = db.FWYNotification.Where(c => c.SendTo == UserID).OrderByDescending(c => c.DateTime).ToList();
            ViewBag.PageCount = fWYNotification.Count() / 10;
            ViewBag.PageNumber = page;
            return View(fWYNotification.ToPagedList(page, 10));
        }

        [Authorize]
        public ActionResult MarkAllAsRead()
        {
            var UserID = User.Identity.GetUserId();
            var fWYNotification = db.FWYNotification.Where(c => c.SendTo == UserID).ToList();
            foreach (var item in fWYNotification)
            {
                item.IsRead = true;
                db.SaveChanges();
            }
            return RedirectToAction("UserNotifications");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
