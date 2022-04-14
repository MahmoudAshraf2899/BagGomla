namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYBrand")]
    public partial class FWYBrand
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYBrand()
        {
            FWYProduct = new HashSet<FWYProduct>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ARName { get; set; }

        public string ARDesciption { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(128)]
        public string SupplierID { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYProduct> FWYProduct { get; set; }
    }
}
