using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityLibrary.DataModel;
namespace BagGomla.ViewModel
{
    public class HomePageVM
    {
        public IList<CategoriesHomeVM> Categories{ get; set; }
        public List<AdSliderVM> AdsSlider { get; set; }
        public IList<ProductVM> Products { get; set; }
    }
}