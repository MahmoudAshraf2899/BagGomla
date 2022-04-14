using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityLibrary.DataModel
{
    [Table("BigGomlaDatabase.FWYFavSupplier")]
    public partial class FWYFavSupplier
    {
        public int Id { get; set; }

        public int SupplierID { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateIn { get; set; }

        //public virtual AspNetUsers AspNetUsers { get; set; }

        //public virtual FWYSupplierCooperation FWYSupplierCooperation { get; set; }
    }
}
