namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYProductImage")]
    public partial class FWYProductImage
    {
        public int ID { get; set; }

        public string Image { get; set; }

        public int? ProductID { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsMain { get; set; }

        public string ImageExtension { get; set; }

        public virtual FWYProduct FWYProduct { get; set; }
    }
}
