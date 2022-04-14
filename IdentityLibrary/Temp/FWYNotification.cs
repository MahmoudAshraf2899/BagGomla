namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYNotification")]
    public partial class FWYNotification
    {
        public int Id { get; set; }

        public int? Type { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        [StringLength(500)]
        public string DetailsURL { get; set; }

        public bool IsRead { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(128)]
        public string SendTo { get; set; }

        public DateTime? DateTime { get; set; }

        [StringLength(128)]
        public string SendFrom { get; set; }

        public bool IsManual { get; set; }

        public string Image { get; set; }

        public string ImageExtension { get; set; }

        public string ArTitle { get; set; }

        public string ArDetails { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        public virtual AspNetUsers AspNetUsers1 { get; set; }
    }
}
