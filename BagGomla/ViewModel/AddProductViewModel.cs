using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class AddProductViewModel
    {
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public int WholesaleQuantity { get; set; }
        public string MeasuringUnit { get; set; }
        public decimal WholesalePrice { get; set; }
        public decimal ConsumerPrice { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string ImageExtension { get; set; }
    }
}