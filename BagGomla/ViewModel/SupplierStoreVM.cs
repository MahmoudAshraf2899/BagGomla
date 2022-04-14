using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class SupplierStoreVM
    {
        public int Id { get; set; }
        public string EnName { get; set; }
        public string ArName { get; set; }
        public string EnLocation { get; set; }
        public string ArLocation { get; set; }
        public string Long { get; set; }
        public string Lat { get; set; }
    }
}