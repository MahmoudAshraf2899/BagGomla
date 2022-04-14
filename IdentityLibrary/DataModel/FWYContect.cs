namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYContect")]
    public partial class FWYContect
    {
        public int ID { get; set; }

        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        public string Address { get; set; }

        public string Password { get; set; }

        public string Facebook { get; set; }

        public string Instagram { get; set; }

        public string Linkedin { get; set; }
    }
}
