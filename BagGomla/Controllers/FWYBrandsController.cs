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
    public class FWYBrandsController : Lan
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
                return View(db.FWYBrand.Where(c => c.IsDeleted == false && c.SupplierID == UserID).ToList());
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
                FWYBrand FWYBrand = new FWYBrand();
                FWYBrand.SupplierID = User.Identity.GetUserId();

                if (ID > 0)
                    FWYBrand = db.FWYBrand.Find(ID);

                return PartialView("_EditCreate", FWYBrand);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(FWYBrand FWYBrand)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if (FWYBrand.ID > 0)
                    {
                        db.Entry(FWYBrand).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.FWYBrand.Add(FWYBrand);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return PartialView("_EditCreate", FWYBrand);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public JsonResult Delete(int Id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYBrand.FirstOrDefault(a => a.ID == Id);
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
