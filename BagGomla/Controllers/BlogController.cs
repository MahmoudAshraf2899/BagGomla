using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using BagGomla.Models;

namespace BagGomla.Controllers
{
    public class BlogController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Blog
        public ActionResult Index()
        {
            BlogModel model = new BlogModel();
            model.BlogList = db.FWYBlog.ToList();
            model.CategoryList = db.FWYCategory.Where(c => c.CategoryID == null).ToList();
            model.FeaturedProductList = db.FWYProduct.Where(c => c.IsFeatured == true).ToList();
            return View(model);
        }

        // GET: Blog/Details/5
        public ActionResult Details(int ID)
        {
            BlogModel model = new BlogModel();
            model.Blog = db.FWYBlog.SingleOrDefault(c => c.ID == ID);
            model.CategoryList = db.FWYCategory.Where(c => c.CategoryID == null).ToList();
            model.FeaturedProductList = db.FWYProduct.Where(c => c.IsFeatured == true).ToList();
            return View(model);
        }

        // GET: Blog/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName");
            return View();
        }

        // POST: Blog/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Title,Description,DateTime,Image,UserID")] FWYBlog fWYBlog)
        {
            if (ModelState.IsValid)
            {
                db.FWYBlog.Add(fWYBlog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName", fWYBlog.UserID);
            return View(fWYBlog);
        }

        // GET: Blog/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FWYBlog fWYBlog = db.FWYBlog.Find(id);
            if (fWYBlog == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName", fWYBlog.UserID);
            return View(fWYBlog);
        }

        // POST: Blog/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,DateTime,Image,UserID")] FWYBlog fWYBlog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fWYBlog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "UserName", fWYBlog.UserID);
            return View(fWYBlog);
        }

        // GET: Blog/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FWYBlog fWYBlog = db.FWYBlog.Find(id);
            if (fWYBlog == null)
            {
                return HttpNotFound();
            }
            return View(fWYBlog);
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FWYBlog fWYBlog = db.FWYBlog.Find(id);
            db.FWYBlog.Remove(fWYBlog);
            db.SaveChanges();
            return RedirectToAction("Index");
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
