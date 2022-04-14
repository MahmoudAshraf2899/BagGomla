using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;
using BagGomla.Helper;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    [Authorize]
    public class FWYCouponsController : Lan
    {
        private DatabaseContext db = new DatabaseContext();

        [HttpGet]
        public ActionResult Index()
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                string UserID = "";
                if (User.Identity.IsAuthenticated)
                {
                    UserID = User.Identity.GetUserId();
                }
                return View(db.FWYCoupon.Where(c => c.IsDeleted == false && c.SupplierID == UserID).ToList());
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpGet]
        public ActionResult EditCreate(int ID)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                FWYCoupon FWYCoupon = new FWYCoupon();
                FWYCoupon.SupplierID = User.Identity.GetUserId();

                if (ID > 0)
                    FWYCoupon = db.FWYCoupon.Find(ID);

                return PartialView("_EditCreate", FWYCoupon);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(FWYCoupon FWYCoupon)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    var Coupons = db.FWYCoupon.Where(c => c.CouponCode == FWYCoupon.CouponCode).ToList();
                    if (FWYCoupon.ID > 0)
                    {
                        if(Coupons.Count == 0)
                        {
                            db.Entry(FWYCoupon).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Coupons.Remove(FWYCoupon);
                        if(Coupons.Count == 0)
                        {
                            db.FWYCoupon.Add(FWYCoupon);
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
                return View(FWYCoupon);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public JsonResult Delete(int Id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYCoupon.FirstOrDefault(a => a.ID == Id);
                del.IsDeleted = true;
                db.SaveChanges();
                return Json(new { status = "Success" });
            }
            return Json(new { status = "Fail" });

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
