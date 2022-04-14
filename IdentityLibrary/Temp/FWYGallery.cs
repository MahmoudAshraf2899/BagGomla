namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYGallery")]
    public partial class FWYGallery
    {
        public int ID { get; set; }

        public string Image { get; set; }

        public string ImageExtension { get; set; }

        public string ImageName { get; set; }
    }
}
