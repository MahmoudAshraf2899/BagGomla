using BagGomla.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IdentityLibrary.DataModel;
using Microsoft.AspNet.Identity;

namespace BagGomla.Helper
{
    public class TagHelper : Controller
    {
        public static DatabaseContext db = new DatabaseContext();
        public NameValueCollection GenerateCookiesCollection(HttpCookie cookie, int productId, int quantity)
        {
            var collection = new NameValueCollection();
            foreach (var value in cookie.Values)
            {
                //If the current element isn't the first empty element.
                if (value != null)
                {
                    collection.Add(value.ToString(), cookie.Values[value.ToString()]);
                }
            }

            //Does this product exist in the cookie?
            if (cookie.Values[productId.ToString()] != null)
            {
                collection.Remove(productId.ToString());
                //Get current count of item in cart.
                int tmpAmount = Convert.ToInt32(cookie.Values[productId.ToString()]);
                int total = tmpAmount + quantity;
                collection.Add(productId.ToString(), total.ToString());
            }
            else //It doesn't exist, so add it.
            {
                collection.Add(productId.ToString(), quantity.ToString());
                cookie.Expires= DateTime.Now.AddYears(1);
            }

            if (!collection.HasKeys())
                collection.Add(productId.ToString(), quantity.ToString());

            return collection;
        }

        public static NameValueCollection CountCookiesCollection(HttpCookie cookie)
        {
            var collection = new NameValueCollection();
            if (cookie!=null)
            {
                foreach (var value in cookie.Values)
                {
                    //If the current element isn't the first empty element.
                    if (value != null)
                    {
                        collection.Add(value.ToString(), cookie.Values[value.ToString()]);
                    }
                }
            }       
            return collection;
        }

        public static int GetMyFavCount(string CurrentUserId)
        {
            if(CurrentUserId != null)
            {
               
                return db.FWYWishList.Where(c => c.UserID == CurrentUserId).Count();
            }
            return 0;
        }
        public static List<string> GetCookiesCollection(HttpCookie cookie)
        {
            var ProductListIDS = new List<string>();
            if (cookie != null)
            {
                foreach (var value in cookie.Values)
                {
                    if (value != null)
                    {
                        ProductListIDS.Add(value.ToString());
                    }
                }
            }
            return ProductListIDS;
        }

        public static bool  CookiesHereOrNot(HttpCookie cookie, int productId)
        {
            bool Flag = false;
            if (cookie != null)
            {
                if (cookie.Values[productId.ToString()] != null)
                {
                    Flag = true;
                }
            }
            return Flag;
        }

        public static bool IsProductInMyFav(int proId, string currentUserId)
        {
            if(currentUserId != null)
            {
                if(db.FWYWishList.Where(c => c.UserID == currentUserId && c.ProductID == proId).Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public NameValueCollection RemoveCookies(HttpCookie cookie, int productId)
        {
            var collection = new NameValueCollection();
            foreach (var value in cookie.Values)
            {
                if (value != null)
                {
                    collection.Add(value.ToString(), cookie.Values[value.ToString()]);
                }
            }
            //Does this product exist in the cookie?
            if (cookie.Values[productId.ToString()] != null)
            {
                collection.Remove(productId.ToString());                
            }
            return collection;
        }

        public NameValueCollection RemoveCartCookies(HttpCookie cookie, string product)
        {
            var collection = new NameValueCollection();
            foreach (var value in cookie.Values)
            {
                if (value != null)
                {
                    collection.Add(value.ToString(), cookie.Values[value.ToString()]);
                }
            }
            //Does this product exist in the cookie?
            if (cookie.Values[product] != null)
            {
                collection.Remove(product);
            }
            return collection;
        }

        public NameValueCollection CreateCookies(HttpCookie cookie, int productId, int SizeID, int colorID, int quantity)
        {
            var collection = new NameValueCollection();
            foreach (var value in cookie.Values)
            {
                //If the current element isn't the first empty element.
                if (value != null)
                {
                    collection.Add(value.ToString(), cookie.Values[value.ToString()]);
                }
            }

            var CookVal = productId + "-" + SizeID + "-" + colorID;
            //Does this product exist in the cookie?
            if (cookie.Values[CookVal] != null)
            {
                collection.Remove(CookVal);
                //Get current count of item in cart.
                int tmpAmount = Convert.ToInt32(cookie.Values[CookVal]);
                int total = tmpAmount + quantity;
                collection.Add(CookVal, total.ToString());

            }
            else //It doesn't exist, so add it.
            {
                collection.Add(CookVal, quantity.ToString());
                cookie.Expires = DateTime.Now.AddYears(1);
            }

            if (!collection.HasKeys())
                collection.Add(CookVal, quantity.ToString());

            return collection;
        }

        public FileData ConvertFileToBase64(System.Web.HttpPostedFileBase YourFile)
        {
            FileData fileData = new FileData();
            if (YourFile != null)
            {
                string theFileName = Path.GetFileName(YourFile.FileName);
                //data: image / jpg; base64,
                var list = theFileName.Split('.');
                var extension = "";
                if (list.ToList().Count > 0)
                {
                    extension = list.Last();
                }
                byte[] thePictureAsBytes = new byte[YourFile.ContentLength];
                using (BinaryReader theReader = new BinaryReader(YourFile.InputStream))
                {
                    thePictureAsBytes = theReader.ReadBytes(YourFile.ContentLength);
                }
                string thePictureDataAsString = Convert.ToBase64String(thePictureAsBytes);
                fileData.FileBase64 = thePictureDataAsString;
                fileData.FileExtension = extension;
            }
            else
            {
                fileData.FileBase64 = "";
                fileData.FileExtension = "";
            }
            
            return fileData;
        }

        public string ConnvertToImageSrc(string base64, string extension)
        {
            var ImageSrc = "";
            if(base64 != "" && base64 != null && extension != "" && extension != null)
            {
                var x = "data:image/" + extension + ";base64,";
                if (base64.Contains(x))
                {
                    ImageSrc = base64;
                }
                else
                {
                    ImageSrc = x + base64;
                }
            }
            return ImageSrc;
        }

        public ActionResult SweetAlert(string Title, string Message, int Type, string ReturnUrl)
        {
            var StrType = "";
            if (Type == 1)
            {
                StrType = "success";
            }
            else
            {
                StrType = "error";
            }
            List<string> SweetAlert = new List<string>() { Title, Message, StrType, ReturnUrl };
            return Json(SweetAlert, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SweetAlert(string Title, string Message, int Type)
        {
            var StrType = "";
            if (Type == 1)
            {
                StrType = "success";
            }
            else
            {
                StrType = "error";
            }
            List<string> SweetAlert = new List<string>() { Title, Message, StrType };
            return Json(SweetAlert, JsonRequestBehavior.AllowGet);
        }

    }
}