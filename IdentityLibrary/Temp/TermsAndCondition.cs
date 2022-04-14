namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BigGomlaDatabase.TermsAndCondition")]
    public partial class TermsAndCondition
    {
        public int Id { get; set; }

        public string English { get; set; }

        public string Arabic { get; set; }
    }
}
