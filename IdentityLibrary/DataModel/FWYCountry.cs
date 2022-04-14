namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYCountry")]
    public partial class FWYCountry
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYCountry()
        {
            FWYProduct = new HashSet<FWYProduct>();
            Cities = new HashSet<City>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string ArName { get; set; }

        public bool IsDeleted { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYProduct> FWYProduct { get; set; }
        public virtual ICollection<City> Cities { get; set; }
    }
}
