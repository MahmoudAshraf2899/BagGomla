namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYProductReview")]
    public partial class FWYProductReview
    {
        public int ID { get; set; }

        public int? ProductID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public int? Rate { get; set; }

        public string Review { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual FWYProduct FWYProduct { get; set; }
    }
}
