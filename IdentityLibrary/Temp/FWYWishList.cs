namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYWishList")]
    public partial class FWYWishList
    {
        public int Id { get; set; }

        public int ProductID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateIn { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual FWYProduct FWYProduct { get; set; }
    }
}
