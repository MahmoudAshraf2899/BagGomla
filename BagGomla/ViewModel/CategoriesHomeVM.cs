using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class CategoriesHomeVM
    {
        public int catID { get; set; }
        public string Name { get; set; }
        public string ArName { get; set; }
        public string Image { get; set; }
        public int TotalProduct { get; set; }

    }
}