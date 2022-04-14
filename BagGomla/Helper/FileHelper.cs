using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BagGomla.Helper
{
    public static class FileHelper
    {
        public static string ConvertFromBase64ToJpg(string base64,string path)
        {
            if (base64.Contains("data:image/jpg;base64,"))
            {
                base64 = base64.Replace("data:image/jpg;base64,", "");
            }
            if (base64.Contains("data:image/jpeg;base64,"))
            {
                base64 = base64.Replace("data:image/jpeg;base64,", "");
            }
            if (base64.Contains("data:image/png;base64,"))
            {
                base64 = base64.Replace("data:image/png;base64,", "");
            }
            string uniqueFileName = Guid.NewGuid().ToString() + ".jpg";
            File.WriteAllBytes(HttpContext.Current.Server.MapPath(Path.Combine(path, uniqueFileName)), Convert.FromBase64String(base64));
            return uniqueFileName;
        }

        public static string UploadFile(HttpPostedFileBase file, string path)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + file.FileName;
            path = HttpContext.Current.Server.MapPath("~" + path);
            file.SaveAs(Path.Combine(path, uniqueFileName));
            return uniqueFileName;
        }
    }
}