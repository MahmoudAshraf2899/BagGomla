namespace IdentityLibrary.DataModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FWYProduct")]
    public partial class FWYProduct
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FWYProduct()
        {
            FWYInvoiceOrderProduct = new HashSet<FWYInvoiceOrderProduct>();
            FWYProductImage = new HashSet<FWYProductImage>();
            FWYProductPriceRange = new HashSet<FWYProductPriceRange>();
            FWYProductReview = new HashSet<FWYProductReview>();
            FWYStoreProduct = new HashSet<FWYStoreProduct>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public int? Number { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }
        public decimal? WholesalePrice { get; set; }


        public decimal? Discount { get; set; }

        public int? BrandID { get; set; }

        public string ARName { get; set; }

        public string ARDescription { get; set; }

        public int? CategoryID { get; set; }

        public int? UnitID { get; set; }

        public int? TotalQuantity { get; set; }

        public bool IsDeleted { get; set; }

        public int? FavouritesNum { get; set; }

        public bool IsFeatured { get; set; }

        public decimal? FinalPrice { get; set; }

        public int SalesNum { get; set; }

        public int? TypeID { get; set; }

        public int? LessQuantityGomla { get; set; }

        public int? CountryID { get; set; }

        public bool IsAvailable { get; set; }
        public bool Show { get; set; }

        public string Country { get; set; }

        public string ArCountry { get; set; }
        public DateTime? CreatedDateTime { get; set; }

        public virtual FWYBrand FWYBrand { get; set; }

        public virtual FWYCategory FWYCategory { get; set; }

        public virtual FWYCountry FWYCountry { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYInvoiceOrderProduct> FWYInvoiceOrderProduct { get; set; }

        public virtual FWYUnit FWYUnit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYProductImage> FWYProductImage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYProductPriceRange> FWYProductPriceRange { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYProductReview> FWYProductReview { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FWYStoreProduct> FWYStoreProduct { get; set; }
    }
}
