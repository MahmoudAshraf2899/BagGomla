namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYStoreProduct")]
    public partial class FWYStoreProduct
    {
        public int ID { get; set; }

        public int? StoreID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(128)]
        public string SupplierID { get; set; }

        public bool IsDeleted { get; set; }

        public int? ColorID { get; set; }

        public int? SizeID { get; set; }

        public int? Quantity { get; set; }

        public int? SalesNum { get; set; }

        public virtual FWYColor FWYColor { get; set; }

        public virtual FWYProduct FWYProduct { get; set; }

        public virtual FWYSize FWYSize { get; set; }

        public virtual FWYStore FWYStore { get; set; }
    }
}
