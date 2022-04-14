namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FWYAds
    {
        public int Id { get; set; }

        public string DetailsURL { get; set; }

        public string Image { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateIn { get; set; }

        public string ImageExtension { get; set; }
    }
}
