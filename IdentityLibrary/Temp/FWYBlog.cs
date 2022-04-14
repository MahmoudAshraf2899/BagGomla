namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYBlog")]
    public partial class FWYBlog
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DateTime { get; set; }

        public string Image { get; set; }

        [StringLength(128)]
        public string UserID { get; set; }

        public string ImageExtension { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
