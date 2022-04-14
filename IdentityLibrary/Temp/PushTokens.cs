namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PushTokens
    {
        public int Id { get; set; }

        public string Token { get; set; }

        public int OS { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime DateIn { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
