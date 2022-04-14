namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYStore")]
    public partial class FWYStore
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYStore()
        {
            FWYStoreProduct = new HashSet<FWYStoreProduct>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string ARName { get; set; }

        public string ARLocation { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(128)]
        public string SupplierID { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Longitude { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Latitude { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYStoreProduct> FWYStoreProduct { get; set; }
    }
}
