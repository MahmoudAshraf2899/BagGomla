namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYInvoiceOrderProduct")]
    public partial class FWYInvoiceOrderProduct
    {
        public int ID { get; set; }

        public int? InvoiceOrderID { get; set; }

        public int? ProductID { get; set; }

        public int? ProductSizeID { get; set; }

        public int? ProductColorID { get; set; }

        public decimal? UnitPrice { get; set; }

        public int? Quantity { get; set; }

        public decimal? TotalPrice { get; set; }

        public bool IsDeleted { get; set; }

        public int? StoreProductID { get; set; }

        public virtual FWYInvoiceOrder FWYInvoiceOrder { get; set; }

        public virtual FWYProduct FWYProduct { get; set; }
    }
}
