using BagGomla.Helper;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    [Authorize]
    public class SupplierRequestController : Lan
    {
        DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();
        // GET: SupplierRequest
        public ActionResult Index(int page = 1, string requestState = "new",string searchStr = "")
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            ViewBag.RequestState = "new";
            ViewBag.RequestTitle = "طلبات جديدة";
            if (IsInRole)
            {
                var requests = new List<FWYSupplierCooperation>();
                if(requestState == "new")
                {
                    requests = db.FWYSupplierCooperation.Where(c => c.IsAccepted == false && c.IsRejected == false && c.IsRequestDeleted == false).OrderByDescending(c => c.ID).ToList();
                }
                else if (requestState == "rejected")
                {
                    requests = db.FWYSupplierCooperation.Where(c => c.IsRejected == true).OrderByDescending(c => c.ID).ToList();
                    ViewBag.RequestState = "rejected";
                    ViewBag.RequestTitle = "طلبات مرفوضة";
                }
                else if (requestState == "accepted")
                {
                    requests = db.FWYSupplierCooperation.Where(c => c.IsAccepted == true).OrderByDescending(c => c.ID).ToList();
                    ViewBag.RequestState = "accepted";
                    ViewBag.RequestTitle = "طلبات موافق عليها";
                }
                else if (requestState == "deleted")
                {
                    requests = db.FWYSupplierCooperation.Where(c => c.IsRequestDeleted == true).OrderByDescending(c => c.ID).ToList();
                    ViewBag.RequestState = "deleted";
                    ViewBag.RequestTitle = "طلبات محذوفة";
                }
                
                if(searchStr != null && searchStr != "")
                {
                    searchStr = searchStr.ToLower();
                    requests= requests.Where(c => (c.AspNetUsers.Name != null && c.AspNetUsers.Name.ToLower().Contains(searchStr))
                    || (c.AspNetUsers.PhoneNumber != null && c.AspNetUsers.PhoneNumber.ToLower().Contains(searchStr))
                    || (c.AspNetUsers.Email != null && c.AspNetUsers.Email.ToLower().Contains(searchStr))
                    || (c.AspNetUsers.CreatedDateTime != null && c.AspNetUsers.CreatedDateTime.ToString().Contains(searchStr))
                    || (c.AspNetUsers.Address != null && c.AspNetUsers.Address.ToLower().Contains(searchStr))).ToList();
                }

                ViewBag.RequestCount = requests.Count();
                ViewBag.searchStr = searchStr;

                return View(requests.ToPagedList(page,10));
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult SupplierCompany(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var Company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                Company.Logo = helper.ConnvertToImageSrc(Company.Logo, Company.LogoExtension);
                string ProfilePicture = "";
                if (Company.ProfilePictureID != null)
                {
                    var gallery = db.FWYGallery.SingleOrDefault(c => c.ID == Company.ProfilePictureID);
                    ProfilePicture = helper.ConnvertToImageSrc(gallery.Image, gallery.ImageExtension);
                }
                ViewBag.ProfilePicture = ProfilePicture;
                return View(Company);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult Accept(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                if (User.Identity.IsAuthenticated)
                {
                    company.IsAccepted = true;
                    company.IsRejected = false;
                    if(company.AspNetUsers.FWYStore.Count == 0)
                    {
                        FWYStore store = new FWYStore()
                        {
                            Name = "My Store",
                            ARName = "المخزن",
                            SupplierID = company.SupplierID
                        };
                        db.FWYStore.Add(store);
                    }
                    db.SaveChanges();
                    var UserID = User.Identity.GetUserId();
                    var Notification = new FWYNotification()
                    {
                        Title = "Request Accepted",
                        Details = "Your request is accepted",
                        DetailsURL = "",
                        DateTime = DateTime.Now,
                        SendFrom = UserID,
                        SendTo = company.SupplierID,
                        ArTitle = "تم الموافقة على طلبك",
                        ArDetails = "تم الموافقة ع طلبك للاشتراك كمورد"
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();
                }

                return RedirectToAction("SupplierCompany", new { ID = company.ID });
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult Reject(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                if (User.Identity.IsAuthenticated)
                {
                    company.IsRejected = true;
                    var UserID = User.Identity.GetUserId();
                    var Notification = new FWYNotification()
                    {
                        Title = "Request Rejected",
                        Details = "Your request is rejected",
                        DetailsURL = "",
                        DateTime = DateTime.Now,
                        SendFrom = UserID,
                        SendTo = company.SupplierID,
                        ArTitle = "تم رفض  طلبك",
                        ArDetails = "تم رفض طلبك للاشتراك كمورد"
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();
                }

                return RedirectToAction(nameof(Index),new { requestState = "rejected"});
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        public ActionResult DeleteRequest(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                if (User.Identity.IsAuthenticated)
                {
                    company.IsRequestDeleted = true;
                    var UserID = User.Identity.GetUserId();
                    var Notification = new FWYNotification()
                    {
                        Title = "Request Deleted",
                        Details = "Your request is deleted",
                        DetailsURL = "",
                        DateTime = DateTime.Now,
                        SendFrom = UserID,
                        SendTo = company.SupplierID,
                        ArTitle = "تم حذف  طلبك",
                        ArDetails = "تم حذف طلبك للاشتراك كمورد"
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();
                }

                return RedirectToAction(nameof(Index), new { requestState = "deleted" });
            }
            return RedirectToAction("AccessDenied", "Account");
        }

        public ActionResult Verify(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == ID);
                if (User.Identity.IsAuthenticated)
                {
                    company.IsVerified = true;
                    db.SaveChanges();
                    var UserID = User.Identity.GetUserId();
                    var Notification = new FWYNotification()
                    {
                        Title = "Company Verified",
                        Details = "Your company is verified",
                        DetailsURL = "",
                        DateTime = DateTime.Now,
                        SendFrom = UserID,
                        SendTo = company.SupplierID,
                        ArTitle = "تم التحقق من الشركة",
                        ArDetails = "تم التجقق من الشركة "
                    };
                    db.FWYNotification.Add(Notification);
                    db.SaveChanges();
                }

                return RedirectToAction("SupplierCompany", new { ID = company.ID });
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public ActionResult DownloadFile(string file)
        {
            //var relativePath = "~/Images/" + file;
            //var absolutePath = HttpContext.Server.MapPath(relativePath);
            //if (System.IO.File.Exists(absolutePath))
            //{
            //    return File(absolutePath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.ChangeExtension(file, Path.GetExtension(relativePath)));
            //}
            //else
            //{
                return Content("");
            //}
        }


        public ActionResult AllowAddStore(int id, bool state)
        {
            var company = db.FWYSupplierCooperation.SingleOrDefault(c => c.ID == id);
            company.AllowAddStore = state;
            db.SaveChanges();
            return RedirectToAction("SupplierCompany", new { ID = id });
        }
    }
}