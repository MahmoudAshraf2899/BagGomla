namespace IdentityLibrary.Temp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYSupplierCooperation")]
    public partial class FWYSupplierCooperation
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYSupplierCooperation()
        {
            FWYFavSupplier = new HashSet<FWYFavSupplier>();
            FWYSupplierCooperationCategory = new HashSet<FWYSupplierCooperationCategory>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public string About { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string WebsiteUrl { get; set; }

        public string Facebook { get; set; }

        public string Instagram { get; set; }

        public string Linkedin { get; set; }

        public string Logo { get; set; }

        public string NationalCopyID { get; set; }

        public string CommericialRegister { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsVerified { get; set; }

        public bool IsAgreedTerms { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(128)]
        public string SupplierID { get; set; }

        public string LogoExtension { get; set; }

        public string NationalCopyIDExtension { get; set; }

        public string CommericialRegisterExtension { get; set; }

        public int? ProfilePictureID { get; set; }

        public string ArName { get; set; }

        public string ArAbout { get; set; }

        public int? TypeID { get; set; }

        public bool AllowAddStore { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYFavSupplier> FWYFavSupplier { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYSupplierCooperationCategory> FWYSupplierCooperationCategory { get; set; }
    }
}
