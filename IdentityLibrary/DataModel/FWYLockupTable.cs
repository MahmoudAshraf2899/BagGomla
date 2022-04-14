namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYLockupTable")]
    public partial class FWYLockupTable
    {
        public int ID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public string UserName { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Longitude { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? Latitude { get; set; }

        public string Location { get; set; }

        public string SearchString { get; set; }

        public int? SearchResultID { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual FWYCategory FWYCategory { get; set; }
        public DateTime? SearchDateTime { get; set; }
    }
}
