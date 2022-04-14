using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityLibrary.DataModel
{
    [Table("FWYWishList")]
    public partial class FWYWishList
    {
        public int Id { get; set; }
        [ForeignKey("Product")]
        public int ProductID { get; set; }
        [ForeignKey("User")]
        public string UserID { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateIn { get; set; }
        public virtual FWYProduct Product { get; set; }
        public virtual AspNetUsers User { get; set; }
    }
}
