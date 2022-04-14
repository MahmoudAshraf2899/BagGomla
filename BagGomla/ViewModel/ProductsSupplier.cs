using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.ViewModel
{
    public class ProductsSupplier
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? WholesalePrice { get; set; }
        public decimal? Discount { get; set; }
        public string ARName { get; set; }
        public string ARDescription { get; set; }
        public int? CategoryID { get; set; }
        public int? TotalQuantity { get; set; }
        public bool IsFeatured { get; set; }
        public decimal? FinalPrice { get; set; }
        public int? LessQuantityGomla { get; set; }
        public bool IsAvailable { get; set; }
        public string Image { get; set; }
        public string ImageExtension { get; set; }
    }
}