namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYProductPriceRange")]
    public partial class FWYProductPriceRange
    {
        public int ID { get; set; }

        public int? ProductID { get; set; }

        public int? FromQuantity { get; set; }

        public int? ToQuantity { get; set; }

        public decimal? Price { get; set; }

        public virtual FWYProduct FWYProduct { get; set; }
    }
}
