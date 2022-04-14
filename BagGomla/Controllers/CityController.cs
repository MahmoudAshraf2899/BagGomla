using BagGomla.Attributes;
using BagGomla.Helper;
using BagGomla.Models;
using IdentityLibrary.DataModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BagGomla.Models.Enum;

namespace BagGomla.Controllers
{
    [CustomAuthorize(Role = UserRole.Admin)]
    public class CityController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();
        // GET: City
        public ActionResult Index(string searchStr, int? countryId, int page = 1)
        {
            var cities = new List<City>();
            cities = db.City.ToList();
            if (searchStr != null)
            {
                searchStr = searchStr.ToLower();
                cities = cities.Where(c => c.ArName.ToLower().Contains(searchStr)
                || c.EnName.ToLower().Contains(searchStr)
                || c.Country.ArName.ToLower().Contains(searchStr)
                || c.Country.Name.ToLower().Contains(searchStr)).ToList();
            }

            if(countryId != null)
            {
                cities = cities.Where(c => c.CountryId == countryId).ToList();
            }

            ViewBag.SearchStr = searchStr;
            ViewBag.CountryId = countryId;
            cities = cities.OrderByDescending(c => c.Id).ToList();
            return View(cities.ToPagedList(page, 10));
        }

        public ActionResult Create()
        {
            ViewBag.CountryList = new SelectList(db.FWYCountry.Where(c=>c.IsDeleted == false).ToList(), "ID", "ArName");
            ViewBag.ActionName = nameof(Create);
            return View("EditCreate", new City());
        }

        [HttpPost]
        public ActionResult Create(City model)
        {
            if (ModelState.IsValid)
            {
                db.City.Add(model);
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CountryList = new SelectList(db.FWYCountry.Where(c=>c.IsDeleted == false).ToList(), "ID", "ArName");
            ViewBag.ActionName = nameof(Create);
            return View("EditCreate", model);
        }


        public ActionResult Edit(int id)
        {
            var model = db.City.Find(id);
            ViewBag.CountryList = new SelectList(db.FWYCountry.Where(c=>c.IsDeleted == false).ToList(), "ID", "ArName");
            ViewBag.ActionName = nameof(Edit);
            return View("EditCreate", model);
        }

        [HttpPost]
        public ActionResult Edit(City model)
        {
            if (ModelState.IsValid)
            {
                var oldModel = db.City.Find(model.Id);
                oldModel.ArName = model.ArName;
                oldModel.EnName = model.EnName;
                oldModel.CountryId = model.CountryId;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CountryList = new SelectList(db.FWYCountry.Where(c=>c.IsDeleted == false).ToList(), "ID", "ArName");
            ViewBag.ActionName = nameof(Edit);
            return View("EditCreate", model);
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            ConfirmDeleteModel model = new ConfirmDeleteModel()
            {
                ControllerName = "City",
                ActionName = "Delete",
                IntID = id
            };
            return PartialView("_DeleteView", model);
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(int ID)
        {
            try
            {
                var item = db.City.Find(ID);
                db.City.Remove(item);
                db.SaveChanges();
                return helper.SweetAlert("تم", "تم حذف المدينة بنجاح", (int)SweetAlertType.success, "/City/Index");
            }
            catch
            {
                return helper.SweetAlert("خطأ", "لا يمكن حذف هذه المدينة", (int)SweetAlertType.error, "/City/Index");
            }
        }

    }
}