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
using BagGomla.Models;
using static BagGomla.Models.Enum;
using System.Globalization;
using System.Drawing;

namespace BagGomla.Controllers
{
    [Authorize]
    public class FWYColorsController : Lan
    {
        private DatabaseContext db = new DatabaseContext();


        private Color GetSystemDrawingColorFromHexString(string hexString)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(hexString, @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
                throw new ArgumentException();
            int red = int.Parse(hexString.Substring(1, 2), NumberStyles.HexNumber);
            int green = int.Parse(hexString.Substring(3, 2), NumberStyles.HexNumber);
            int blue = int.Parse(hexString.Substring(5, 2), NumberStyles.HexNumber);
            return Color.FromArgb(red, green, blue);
        }

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
                var Colors = db.FWYColor.Where(c => c.IsDeleted == false && c.SupplierID == UserID).ToList();
                //var ColorNames = new List<string>();
                //var lang = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.CultureName;
                //foreach (var item in Colors)
                //{
                //    Color color = GetSystemDrawingColorFromHexString(item.Name);
                //    string colorName = "UnKnown";
                //    if(lang == "ar-EG")
                //    {
                //        colorName = "غير معروف";
                //    }
                //    foreach (KnownColor kc in System.Enum.GetValues(typeof(KnownColor)))
                //    {
                //        Color known = Color.FromKnownColor(kc);
                //        if (color.ToArgb() == known.ToArgb())
                //        {
                //            colorName = known.Name;
                //            break;
                //        }
                //    }
                //    ColorNames.Add(colorName);
                //}
                //ViewBag.ColorNames = ColorNames;
                return View(Colors);
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
                FWYColor FWYColor = new FWYColor();
                FWYColor.SupplierID = User.Identity.GetUserId();

                if (ID > 0)
                    FWYColor = db.FWYColor.Find(ID);

                return PartialView("_EditCreate", FWYColor);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditCreate(FWYColor FWYColor)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                if (ModelState.IsValid)
                {
                    if (FWYColor.ID > 0)
                    {
                        db.Entry(FWYColor).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.FWYColor.Add(FWYColor);
                        db.SaveChanges();
                    }
                    return RedirectToAction("Index");
                }
                return View(FWYColor);
            }
            return RedirectToAction("AccessDenied", "Account");
            
        }

        public JsonResult Delete(int Id)
        {
            var Role = ((int)UserRole.Supplier).ToString();
            var IsInRole = User.IsInRole(Role);
            if (IsInRole)
            {
                var del = db.FWYColor.FirstOrDefault(a => a.ID == Id);
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
