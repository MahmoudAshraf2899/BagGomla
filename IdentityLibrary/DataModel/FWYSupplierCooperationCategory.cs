namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYSupplierCooperationCategory")]
    public partial class FWYSupplierCooperationCategory
    {
        public int ID { get; set; }

        public int? CompanyID { get; set; }

        public int? CategoryID { get; set; }

        public virtual FWYCategory FWYCategory { get; set; }

        public virtual FWYSupplierCooperation FWYSupplierCooperation { get; set; }
    }
}
