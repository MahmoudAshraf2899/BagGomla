using IdentityLibrary.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BagGomla.Models
{
    public struct ShoppingCartStruct
    {
       
        public ShoppingCartStruct(FWYProduct Products, int SizeID, int ColorID, FWYColor Colors, FWYSize Sizes) : this()
        {
            this.Product = Products;
            this.ProductSize = SizeID;
            this.ProductColor = ColorID;
            this.Color = Colors;
            this.Size = Sizes;
        }

        public FWYProduct Product{ get; set; }
        public FWYColor Color { get; set; }
        public FWYSize Size { get; set; }
        public int ProductSize { get; set; }
        public int ProductColor { get; set; }
    }
}

