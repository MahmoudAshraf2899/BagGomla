using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Models
{
    public class BlogModel
    {
        public FWYBlog Blog { get; set; }
        public List<FWYCategory> CategoryList { get; set; }
        public List<FWYProduct> FeaturedProductList { get; set; }
        public List<FWYBlog> BlogList { get; set; }
    }
}