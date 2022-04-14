namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYCategory")]
    public partial class FWYCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYCategory()
        {
            FWYCategory1 = new HashSet<FWYCategory>();
            FWYLockupTable = new HashSet<FWYLockupTable>();
            FWYProduct = new HashSet<FWYProduct>();
            FWYSupplierCooperationCategory = new HashSet<FWYSupplierCooperationCategory>();
        }
       
        
        public int ID { get; set; }

        public string Name { get; set; }

        public int? Number { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string ARName { get; set; }

        public string ARDescription { get; set; }

        public int? CategoryID { get; set; }

        public bool IsDeleted { get; set; }

        public string Image { get; set; }

        [StringLength(128)]
        public string SupplierID { get; set; }

        public string ImageExtension { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYCategory> FWYCategory1 { get; set; }

        public virtual FWYCategory FWYCategory2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYLockupTable> FWYLockupTable { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYProduct> FWYProduct { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYSupplierCooperationCategory> FWYSupplierCooperationCategory { get; set; }
    }
}
