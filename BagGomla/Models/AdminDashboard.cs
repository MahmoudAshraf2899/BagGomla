using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityLibrary.DataModel;

namespace BagGomla.Models
{
    public class AdminDashboard
    {
        public List<FWYSupplierCooperation> SellerList { get; set; }
        public List<AspNetUsers> CustomerList { get; set; }
        public List<FWYProduct> ProductList { get; set; }
        public List<FWYInvoiceOrder> InvoiceOrderList { get; set; }
    }
}