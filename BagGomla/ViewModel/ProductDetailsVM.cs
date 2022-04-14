using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class ProductDetailsVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string arName { get; set; } 
        
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }

        public decimal? Price { get; set; }

        public decimal minPrice { get; set; }

        public decimal maxPrice { get; set; }

        public string Description { get; set; }

        public string CompanyName { get; set; }
        public string arCompanyName { get; set; }

        public string SupplierPhone { get; set; }
        public int SupplierId { get; set; }
        public string UserId { get; set; }
        public int minPeices { get; set; }
        public bool IsSupllierVerified { get; set; }

        public List<string> productImages { get; set; }
        public string Country { get; set; }

    }
}