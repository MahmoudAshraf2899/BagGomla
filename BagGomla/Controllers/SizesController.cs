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
    public class SizesController : Lan
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
                return View(db.FWYSize.Where(c => c.IsDeleted == false && c.SupplierID == UserID).ToList());
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
                FWYSize FWYSize = new FWYSize();
                FWYSize.SupplierID = User.Identity.GetUserId();

                if (ID > 0)
                    FWYSize = db.FWYSize.Find(ID);

                return PartialView("_EditCreate", FWYSize);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(FWYSize FWYSize)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if (FWYSize.ARName == null || FWYSize.ARName == "")
                    {
                        FWYSize.ARName = FWYSize.Name;
                    } 
                    if (FWYSize.ID > 0)
                    {
                        db.Entry(FWYSize).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.FWYSize.Add(FWYSize);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return PartialView(FWYSize);
            }
            return RedirectToAction("AccessDenied", "Account");
           
        }

        public JsonResult Delete(int Id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYSize.FirstOrDefault(a => a.ID == Id);
                if(del.FWYStoreProduct.Count == 0)
                {
                    del.IsDeleted = true;
                    db.SaveChanges();
                    return Json(new { status = "Success" });
                }
                
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
