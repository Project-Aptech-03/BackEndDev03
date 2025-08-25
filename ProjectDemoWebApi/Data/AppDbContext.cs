using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // New database schema models
        public DbSet<Categories> Categories { get; set; } = null!;
        public DbSet<Manufacturers> Manufacturers { get; set; } = null!;
        public DbSet<Publishers> Publishers { get; set; } = null!;
        public DbSet<Products> Products { get; set; } = null!;
        public DbSet<ProductPhotos> ProductPhotos { get; set; } = null!;
        public DbSet<ShoppingCart> ShoppingCart { get; set; } = null!;
        public DbSet<CustomerAddresses> CustomerAddresses { get; set; } = null!;
        public DbSet<Orders> Orders { get; set; } = null!;
        public DbSet<OrderItems> OrderItems { get; set; } = null!;
        public DbSet<Payments> Payments { get; set; } = null!;
        public DbSet<ProductReturns> ProductReturns { get; set; } = null!;
        public DbSet<StockMovements> StockMovements { get; set; } = null!;
        public DbSet<Coupons> Coupons { get; set; } = null!;
        public DbSet<CustomerQueries> CustomerQueries { get; set; } = null!;
        public DbSet<AdminReplies> AdminReplies { get; set; } = null!;
        public DbSet<FAQ> FAQ { get; set; } = null!;
        public DbSet<SystemSettings> SystemSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ignore Users entity since it's managed by AuthDbContext
            modelBuilder.Ignore<Users>();

            // Configure Categories
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasIndex(e => e.CategoryCode)
                    .IsUnique()
                    .HasDatabaseName("idx_categories_code");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Manufacturers
            modelBuilder.Entity<Manufacturers>(entity =>
            {
                entity.HasIndex(e => e.ManufacturerCode)
                    .IsUnique()
                    .HasDatabaseName("idx_manufacturers_code");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Publishers
            modelBuilder.Entity<Publishers>(entity =>
            {
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Products (new schema) - this will use the "Products" table
            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasIndex(e => e.ProductCode)
                    .IsUnique()
                    .HasDatabaseName("idx_products_code");

                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("idx_products_category");

                entity.HasIndex(e => e.ManufacturerId)
                    .HasDatabaseName("idx_products_manufacturer");

                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("idx_products_active");

                entity.Property(e => e.StockQuantity)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configure relationships
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Manufacturer)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ManufacturerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.PublisherId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure ProductPhotos
            modelBuilder.Entity<ProductPhotos>(entity =>
            {
                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_product_photos_product");

                entity.HasIndex(e => e.IsActive)
                    .HasDatabaseName("idx_product_photos_active");

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductPhotos)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ShoppingCart
            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_cart_user");

                entity.HasIndex(e => new { e.UserId, e.ProductId })
                    .IsUnique()
                    .HasDatabaseName("idx_cart_user_product");

                entity.HasIndex(e => e.AddedDate)
                    .HasDatabaseName("idx_cart_date");

                entity.Property(e => e.Quantity)
                    .HasDefaultValue(1);

                entity.Property(e => e.AddedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCartItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Note: Users relationship will be handled by foreign key constraint only
                // since Users entity is managed by AuthDbContext
            });

            // Configure CustomerAddresses
            modelBuilder.Entity<CustomerAddresses>(entity =>
            {
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_addresses_user");

                entity.HasIndex(e => e.DistanceKm)
                    .HasDatabaseName("idx_addresses_distance");

                entity.Property(e => e.IsDefault)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure Orders
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber)
                    .IsUnique()
                    .HasDatabaseName("idx_orders_number");

                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("idx_orders_customer");

                entity.HasIndex(e => e.OrderStatus)
                    .HasDatabaseName("idx_orders_status");

                entity.HasIndex(e => e.PaymentType)
                    .HasDatabaseName("idx_orders_payment_type");

                entity.HasIndex(e => e.OrderDate)
                    .HasDatabaseName("idx_orders_date");

                entity.Property(e => e.OrderStatus)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.PaymentStatus)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.CouponDiscountAmount)
                    .HasDefaultValue(0);

                entity.Property(e => e.DeliveryCharges)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.DeliveryAddress)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DeliveryAddressId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure OrderItems
            modelBuilder.Entity<OrderItems>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("idx_order_items_order");

                entity.HasIndex(e => e.ProductId)
                    .HasDatabaseName("idx_order_items_product");

                entity.Property(e => e.Quantity)
                    .HasDefaultValue(1);

                entity.Property(e => e.DiscountPercent)
                    .HasDefaultValue(0);

                entity.Property(e => e.DiscountAmount)
                    .HasDefaultValue(0);

                // Configure relationships
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payments
            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("idx_payments_order");

                entity.Property(e => e.PaymentStatus)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ProductReturns
            modelBuilder.Entity<ProductReturns>(entity =>
            {
                entity.HasIndex(e => e.ReturnNumber)
                    .IsUnique()
                    .HasDatabaseName("idx_returns_number");

                entity.HasIndex(e => e.OrderId)
                    .HasDatabaseName("idx_returns_order");

                entity.Property(e => e.Status)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.RefundAmount)
                    .HasDefaultValue(0);

                entity.Property(e => e.ReturnDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ProductReturns)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure StockMovements
            modelBuilder.Entity<StockMovements>(entity =>
            {
                entity.HasIndex(e => new { e.ProductId, e.CreatedDate })
                    .HasDatabaseName("idx_stock_product_date");

                entity.HasIndex(e => new { e.ReferenceType, e.ReferenceId })
                    .HasDatabaseName("idx_stock_reference");

                entity.Property(e => e.UnitCost)
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.StockMovements)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Coupons
            modelBuilder.Entity<Coupons>(entity =>
            {
                entity.HasIndex(e => e.CouponCode)
                    .IsUnique()
                    .HasDatabaseName("idx_coupons_code");

                entity.HasIndex(e => new { e.IsActive, e.StartDate, e.EndDate })
                    .HasDatabaseName("idx_coupons_validity");

                entity.HasIndex(e => new { e.IsAutoApply, e.MinOrderAmount })
                    .HasDatabaseName("idx_coupons_auto");

                entity.Property(e => e.MinOrderAmount)
                    .HasDefaultValue(0);

                entity.Property(e => e.MaxDiscountAmount)
                    .HasDefaultValue(0);

                entity.Property(e => e.Quantity)
                    .HasDefaultValue(1);

                entity.Property(e => e.IsAutoApply)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure CustomerQueries
            modelBuilder.Entity<CustomerQueries>(entity =>
            {
                entity.HasIndex(e => e.CustomerId)
                    .HasDatabaseName("idx_queries_customer");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_queries_status");

                entity.Property(e => e.Status)
                    .HasDefaultValue("Open");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure AdminReplies
            modelBuilder.Entity<AdminReplies>(entity =>
            {
                entity.HasIndex(e => e.QueryId)
                    .HasDatabaseName("idx_replies_query");

                entity.Property(e => e.ReplyDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Query)
                    .WithMany(p => p.AdminReplies)
                    .HasForeignKey(d => d.QueryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure FAQ
            modelBuilder.Entity<FAQ>(entity =>
            {
                entity.HasIndex(e => e.SortOrder)
                    .HasDatabaseName("idx_faq_sort");

                entity.Property(e => e.SortOrder)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure SystemSettings
            modelBuilder.Entity<SystemSettings>(entity =>
            {
                entity.HasIndex(e => e.SettingKey)
                    .IsUnique()
                    .HasDatabaseName("idx_settings_key");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
