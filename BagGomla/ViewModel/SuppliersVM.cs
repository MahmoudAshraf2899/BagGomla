using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class SuppliersVM
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public string SupplierImage { get; set; }
        public string SupplierImageExtension { get; set; }
        public bool IsSupplierVerified { get; set; }
        public int? ProductsNumber { get; set; }
    }
}