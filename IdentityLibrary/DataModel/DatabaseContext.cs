namespace IdentityLibrary.DataModel
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=DatabaseContext")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<FWYAbout> FWYAbout { get; set; }
        public virtual DbSet<FWYBlog> FWYBlog { get; set; }
        public virtual DbSet<FWYBrand> FWYBrand { get; set; }
        public virtual DbSet<FWYCategory> FWYCategory { get; set; }
        public virtual DbSet<FWYColor> FWYColor { get; set; }
        public virtual DbSet<FWYComplaintOrsuggestion> FWYComplaintOrsuggestion { get; set; }
        public virtual DbSet<FWYContect> FWYContect { get; set; }
        public virtual DbSet<FWYCountry> FWYCountry { get; set; }
        public virtual DbSet<FWYCoupon> FWYCoupon { get; set; }
        public virtual DbSet<FWYGallery> FWYGallery { get; set; }
        public virtual DbSet<FWYInvoiceOrder> FWYInvoiceOrder { get; set; }
        public virtual DbSet<FWYInvoiceOrderProduct> FWYInvoiceOrderProduct { get; set; }
        public virtual DbSet<FWYLockupTable> FWYLockupTable { get; set; }
        public virtual DbSet<FWYNotification> FWYNotification { get; set; }
        public virtual DbSet<FWYOrderType> FWYOrderType { get; set; }
        public virtual DbSet<FWYProduct> FWYProduct { get; set; }
        public virtual DbSet<FWYProductImage> FWYProductImage { get; set; }
        public virtual DbSet<FWYProductPriceRange> FWYProductPriceRange { get; set; }
        public virtual DbSet<FWYProductReview> FWYProductReview { get; set; }
        public virtual DbSet<FWYSize> FWYSize { get; set; }
        public virtual DbSet<FWYStore> FWYStore { get; set; }
        public virtual DbSet<FWYStoreProduct> FWYStoreProduct { get; set; }
        public virtual DbSet<FWYSupplierCooperation> FWYSupplierCooperation { get; set; }
        public virtual DbSet<FWYSupplierCooperationCategory> FWYSupplierCooperationCategory { get; set; }
        public virtual DbSet<FWYUnit> FWYUnit { get; set; }
        public virtual DbSet<LatestNews> LatestNews { get; set; }
        public virtual DbSet<FWYAds> FWYAds { get; set; }
        public virtual DbSet<FWYWishList> FWYWishList { get; set; }
        public virtual DbSet<FWYFavSupplier> FWYFavSupplier { get; set; }
        public virtual DbSet<PushToken> PushTokens { get; set; }
        public virtual DbSet<City> City { get; set; }
        [NotMapped]
        public DbSet<SP_GetSupplierProducts_Result> SP_GetSupplierProducts { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithOptional(e => e.AspNetRoles)
                .HasForeignKey(e => e.RoleID);

            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetRoles)
                .HasForeignKey(e => e.RoleId);

            modelBuilder.Entity<AspNetUsers>()
                .Property(e => e.Latitude)
                .HasPrecision(18, 0);

            modelBuilder.Entity<AspNetUsers>()
                .Property(e => e.Longitude)
                .HasPrecision(18, 0);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserRoles)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYBlog)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYBrand)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYCategory)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYColor)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYCoupon)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYInvoiceOrder)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYInvoiceOrder1)
                .WithOptional(e => e.AspNetUsers1)
                .HasForeignKey(e => e.SupplierID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYLockupTable)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYNotification)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SendFrom);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYNotification1)
                .WithOptional(e => e.AspNetUsers1)
                .HasForeignKey(e => e.SendTo)
                .WillCascadeOnDelete();

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYProductReview)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYSize)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYStore)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYSupplierCooperation)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.FWYUnit)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.SupplierID);

            modelBuilder.Entity<FWYBrand>()
                .HasMany(e => e.FWYProduct)
                .WithOptional(e => e.FWYBrand)
                .HasForeignKey(e => e.BrandID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYCategory>()
                .HasMany(e => e.FWYCategory1)
                .WithOptional(e => e.FWYCategory2)
                .HasForeignKey(e => e.CategoryID);

            modelBuilder.Entity<FWYCategory>()
                .HasMany(e => e.FWYLockupTable)
                .WithOptional(e => e.FWYCategory)
                .HasForeignKey(e => e.SearchResultID);

            modelBuilder.Entity<FWYCategory>()
                .HasMany(e => e.FWYProduct)
                .WithOptional(e => e.FWYCategory)
                .HasForeignKey(e => e.CategoryID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYCategory>()
                .HasMany(e => e.FWYSupplierCooperationCategory)
                .WithOptional(e => e.FWYCategory)
                .HasForeignKey(e => e.CategoryID);

            modelBuilder.Entity<FWYColor>()
                .HasMany(e => e.FWYStoreProduct)
                .WithOptional(e => e.FWYColor)
                .HasForeignKey(e => e.ColorID);

            modelBuilder.Entity<FWYCountry>()
                .HasMany(e => e.FWYProduct)
                .WithOptional(e => e.FWYCountry)
                .HasForeignKey(e => e.CountryID);

            modelBuilder.Entity<FWYCountry>()
                .HasMany(e => e.Cities);

            modelBuilder.Entity<FWYCoupon>()
                .Property(e => e.Discount)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYCoupon>()
                .HasMany(e => e.FWYInvoiceOrder)
                .WithOptional(e => e.FWYCoupon)
                .HasForeignKey(e => e.CouponID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYInvoiceOrder>()
                .Property(e => e.DeliveryServicePrice)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYInvoiceOrder>()
                .Property(e => e.SubTotal)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYInvoiceOrder>()
                .Property(e => e.Discount)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYInvoiceOrder>()
                .Property(e => e.TotalPrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYInvoiceOrder>()
                .HasMany(e => e.FWYInvoiceOrderProduct)
                .WithOptional(e => e.FWYInvoiceOrder)
                .HasForeignKey(e => e.InvoiceOrderID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYInvoiceOrderProduct>()
                .Property(e => e.UnitPrice)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYInvoiceOrderProduct>()
                .Property(e => e.TotalPrice)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYLockupTable>()
                .Property(e => e.Longitude)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYLockupTable>()
                .Property(e => e.Latitude)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYOrderType>()
                .HasMany(e => e.FWYInvoiceOrder)
                .WithOptional(e => e.FWYOrderType)
                .HasForeignKey(e => e.OrderTypeID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYProduct>()
                .Property(e => e.Price)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYProduct>()
                .Property(e => e.Discount)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYProduct>()
                .Property(e => e.FinalPrice)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYProduct>()
                .HasMany(e => e.FWYInvoiceOrderProduct)
                .WithOptional(e => e.FWYProduct)
                .HasForeignKey(e => e.ProductID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYProduct>()
                .HasMany(e => e.FWYProductImage)
                .WithOptional(e => e.FWYProduct)
                .HasForeignKey(e => e.ProductID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYProduct>()
                .HasMany(e => e.FWYProductPriceRange)
                .WithOptional(e => e.FWYProduct)
                .HasForeignKey(e => e.ProductID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYProduct>()
                .HasMany(e => e.FWYProductReview)
                .WithOptional(e => e.FWYProduct)
                .HasForeignKey(e => e.ProductID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYProduct>()
                .HasMany(e => e.FWYStoreProduct)
                .WithOptional(e => e.FWYProduct)
                .HasForeignKey(e => e.ProductID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYProductPriceRange>()
                .Property(e => e.Price)
                .HasPrecision(18, 3);

            modelBuilder.Entity<FWYSize>()
                .Property(e => e.Width)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYSize>()
                .Property(e => e.Height)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYSize>()
                .HasMany(e => e.FWYStoreProduct)
                .WithOptional(e => e.FWYSize)
                .HasForeignKey(e => e.SizeID);

            modelBuilder.Entity<FWYStore>()
                .Property(e => e.Longitude)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYStore>()
                .Property(e => e.Latitude)
                .HasPrecision(18, 0);

            modelBuilder.Entity<FWYStore>()
                .HasMany(e => e.FWYStoreProduct)
                .WithOptional(e => e.FWYStore)
                .HasForeignKey(e => e.StoreID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYSupplierCooperation>()
                .HasMany(e => e.FWYSupplierCooperationCategory)
                .WithOptional(e => e.FWYSupplierCooperation)
                .HasForeignKey(e => e.CompanyID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<FWYUnit>()
                .HasMany(e => e.FWYProduct)
                .WithOptional(e => e.FWYUnit)
                .HasForeignKey(e => e.UnitID)
                .WillCascadeOnDelete();

            modelBuilder.Entity<City>()
                .HasMany(e => e.Suppliers);

            //modelBuilder.Entity<AspNetUsers>()
            //   .HasMany(e => e.FWYFavSupplier)
            //   .WithOptional(e => e.AspNetUsers)
            //   .HasForeignKey(e => e.UserID);

            //modelBuilder.Entity<FWYSupplierCooperation>()
            //  .HasMany(e => e.FWYFavSupplier)
            //  .WithRequired(e => e.FWYSupplierCooperation)
            //  .HasForeignKey(e => e.SupplierID);
        }
    }
}
