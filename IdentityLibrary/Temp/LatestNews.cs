namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LatestNews
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string ArName { get; set; }
    }
}
