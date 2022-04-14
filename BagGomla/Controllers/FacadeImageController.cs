using BagGomla.Helper;
using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BagGomla.Controllers
{
    public class FacadeImageController : Controller
    {
        DatabaseContext db = new DatabaseContext();
        private TagHelper helper = new TagHelper();

        // GET: FacadeImage
        public ActionResult Index()
        {
            List<FWYAds> ads = new List<FWYAds>();
            ads = db.FWYAds.Where(c => c.IsDeleted == false).OrderByDescending(c => c.Id).ToList();
            return View(ads);
        }

        public ActionResult Create()
        {
            return View();
        } 
        
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase picture)
        {
            try
            {
                if (picture != null)
                {
                    FWYAds ads = new FWYAds();
                    //var fileData = helper.ConvertFileToBase64(picture);
                    ads.Image = FileHelper.UploadFile(picture, "/Images/SliderAds");
                    //ads.ImageExtension = fileData.FileExtension;
                    ads.DateIn = DateTime.Today;
                    db.FWYAds.Add(ads);
                    db.SaveChanges();
                }
                else
                {
                        // Determine the suffix found not a picture, prompt the user
                       
                }
              
            }
            catch (Exception ex)
            {
                throw;
            }
            return RedirectToAction("Index");
        }
    
        
        public ActionResult Delete(int id)
        {
            FWYAds ads = db.FWYAds.FirstOrDefault(ads => ads.Id == id);
            if(ads != null)
            {
                ads.IsDeleted = true;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
             
    
    }
 }