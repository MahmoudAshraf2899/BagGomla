using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class ProductVM
    {
        public ProductVM()
        {
            ImageList = new List<string>();
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string ARName { get; set; }
        public decimal? Price { get; set; }
        public decimal latitude { get; set; }
        public int? LessQuantityGomla { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
        public decimal longtitude { get; set; }
        public string PhoneNumber { get; set; }
        public bool isFavorite { get; set; }
        public string SupplierName { get; set; }
        public string CpmanyName { get; set; }
        public string ArCpmanyName { get; set; }
        public int? MinAmount { get; set; }
        public  string Image { get; set; }
        public  string Country { get; set; }
        public  string ArCountry { get; set; }
        public List<string> ImageList { get; set; }
        public bool IsSupplierVerified { get; set; }

    }
}