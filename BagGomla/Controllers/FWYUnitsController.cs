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
using BagGomla.Models;

namespace BagGomla.Controllers
{
    [Authorize]
    public class FWYUnitsController : Lan
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();


        [HttpGet]
        public ActionResult Index()
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var Role2 = ((int)UserRole.Admin).ToString();

            var IsInRole = User.IsInRole(Role);
            var IsInRole2 = User.IsInRole(Role2);

            if (IsInRole|| IsInRole2)
            {               
                return View(db.FWYUnit.Where(c => c.IsDeleted == false).OrderByDescending(c => c.ID).ToList());
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpGet]
        public ActionResult EditCreate(int ID)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                FWYUnit FWYUnit = new FWYUnit();
                if (ID > 0)
                    FWYUnit = db.FWYUnit.Find(ID);

                return PartialView("_EditCreate", FWYUnit);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(FWYUnit FWYUnit)
        {
            var Role = ((int)UserRole.Admin).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if (FWYUnit.ARName == null || FWYUnit.ARName == "")
                    {
                        FWYUnit.ARName = FWYUnit.Name;
                    }
                    if (FWYUnit.ID > 0)
                    {
                        db.Entry(FWYUnit).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.FWYUnit.Add(FWYUnit);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(FWYUnit);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            ConfirmDeleteModel model = new ConfirmDeleteModel()
            {
                ControllerName = "FWYUnits",
                ActionName = "Delete",
                IntID = id
            };
            return PartialView("_DeleteView", model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int ID)
        {
            var Del = db.FWYUnit.Find(ID);
            if (Del.FWYProduct.Count == 0)
            {
                db.FWYUnit.Remove(Del);
                db.SaveChanges();
                return helper.SweetAlert("تم", "تم حذف الوحدة بنجاح", (int)SweetAlertType.success, "/FWYUnits/Index");
            }
            else
            {
                return helper.SweetAlert("خطأ", "لم يتم حذف الوحدة ", (int)SweetAlertType.error, "/FWYUnits/Index");
            }
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
