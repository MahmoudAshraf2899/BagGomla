using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityLibrary.DataModel;
using PagedList;

namespace BagGomla.Models
{
    public class HomeModel
    {
        public IPagedList<FWYProduct> ProductList { get; set; }
        public List<FWYProductReview> TopRateProductList { get; set; }
        public List<int> ProductReviewList { get; set; }
        public List<FWYBlog> BlogList { get; set; }
        public List<FWYCategory> CategoryList { get; set; }

        public List<FWYNotification> NotificationList { get; set; }

    }
}