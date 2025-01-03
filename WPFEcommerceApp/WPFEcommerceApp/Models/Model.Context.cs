//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WPFEcommerceApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EcommerceAppEntities : DbContext
    {
        public EcommerceAppEntities()
            : base("name=EcommerceAppEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<AdInUse> AdInUses { get; set; }
        public virtual DbSet<Advertisement> Advertisements { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<BrandRequest> BrandRequests { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryRequest> CategoryRequests { get; set; }
        public virtual DbSet<ImageProduct> ImageProducts { get; set; }
        public virtual DbSet<MOrder> MOrders { get; set; }
        public virtual DbSet<MUser> MUsers { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<OrderInfo> OrderInfoes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Promo> Promoes { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<RatingInfo> RatingInfoes { get; set; }
        public virtual DbSet<ShopRequest> ShopRequests { get; set; }
    }
}
