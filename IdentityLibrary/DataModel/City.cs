namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("City")]
    public partial class City
    {
        public City()
        {
            Suppliers = new HashSet<FWYSupplierCooperation>();
        }
        public int Id { get; set; }

        public string EnName { get; set; }
        public string ArName { get; set; }
        public int CountryId { get; set; }
        public virtual FWYCountry Country { get; set; }
        public virtual ICollection<FWYSupplierCooperation> Suppliers { get; set; }

    }
}
