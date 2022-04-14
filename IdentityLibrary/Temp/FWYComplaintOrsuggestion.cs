namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYComplaintOrsuggestion")]
    public partial class FWYComplaintOrsuggestion
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string EmailFrom { get; set; }

        [StringLength(50)]
        public string EmailTo { get; set; }

        public string ComplaintOrsuggestion { get; set; }

        public bool IsRead { get; set; }
    }
}
