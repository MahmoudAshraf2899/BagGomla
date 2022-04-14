namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYInvoiceOrder")]
    public partial class FWYInvoiceOrder
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYInvoiceOrder()
        {
            FWYInvoiceOrderProduct = new HashSet<FWYInvoiceOrderProduct>();
        }

        public int ID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public int? OrderTypeID { get; set; }

        [StringLength(128)]
        public string SupplierID { get; set; }

        public int? CouponID { get; set; }

        public DateTime? OrderDateTime { get; set; }

        public decimal? DeliveryServicePrice { get; set; }

        public decimal? SubTotal { get; set; }

        public decimal? Discount { get; set; }

        public decimal? TotalPrice { get; set; }

        public bool IsDeleted { get; set; }

        public string ShippingCompany { get; set; }

        public int? TrackNumber { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual AspNetUsers AspNetUsers1 { get; set; }

        public virtual FWYCoupon FWYCoupon { get; set; }

        public virtual FWYOrderType FWYOrderType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYInvoiceOrderProduct> FWYInvoiceOrderProduct { get; set; }
    }
}
