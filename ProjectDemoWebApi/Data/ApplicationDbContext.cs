using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users, Roles, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // New database schema models
        public DbSet<Categories> Categories { get; set; } = null!;
        public DbSet<SubCategories> SubCategories { get; set; } = null!;
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


        public DbSet<Blogs> Blogs { get; set; } = null!;
        public DbSet<BlogComments> BlogComments { get; set; } = null!;
        public DbSet<BlogLikes> BlogLikes { get; set; } = null!;
        public DbSet<CommentLikes> CommentLikes { get; set; } = null!;
        public DbSet<AuthorFollows> AuthorFollows { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserRole relationships properly
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne<Users>()
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();

                entity.HasOne<Roles>()
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
            });

            // Configure ShoppingCart relationship with Users
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

                // Configure Users relationship properly
                entity.HasOne(d => d.User)
                    .WithMany(u => u.ShoppingCartItems)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

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

                // Configure 1-n relationship with SubCategories
                entity.HasMany(c => c.SubCategories)
                      .WithOne(sc => sc.Category)
                      .HasForeignKey(sc => sc.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);


            });

            // Configure SubCategories
            modelBuilder.Entity<SubCategories>(entity =>
            {
                entity.HasIndex(e => e.SubCategoryCode)
                      .IsUnique()
                      .HasDatabaseName("idx_subcategories_code");

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

                // Configure Users relationship
                entity.HasOne(d => d.User)
                    .WithMany(u => u.CustomerAddresses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
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
                   .WithMany()
                   .HasForeignKey(d => d.DeliveryAddressId)
                   .OnDelete(DeleteBehavior.Restrict);


                // Configure Customer relationship
                entity.HasOne(o => o.Customer)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.CustomerId)
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
                    .OnDelete(DeleteBehavior.Cascade);
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

                // Configure Customer relationship
                entity.HasOne(d => d.Customer)
                    .WithMany(u => u.CustomerQueries)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull);
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

                // Configure Admin relationship
                entity.HasOne(d => d.Admin)
                    .WithMany(u => u.AdminReplies)
                    .HasForeignKey(d => d.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);
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


            // Configure Blogs
            modelBuilder.Entity<Blogs>(entity =>
            {
                entity.HasIndex(e => e.Slug)
                    .IsUnique()
                    .HasDatabaseName("idx_blogs_slug");

                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("idx_blogs_author");

                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("idx_blogs_category");

                entity.HasIndex(e => new { e.IsPublished, e.PublishedDate })
                    .HasDatabaseName("idx_blogs_published");

                entity.HasIndex(e => e.IsFeatured)
                    .HasDatabaseName("idx_blogs_featured");

                entity.Property(e => e.ViewCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.LikeCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.CommentCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.IsPublished)
                    .HasDefaultValue(false);

                entity.Property(e => e.IsFeatured)
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Category)
                     .WithMany(c => c.Blogs)
                     .HasForeignKey(b => b.CategoryId)
                     .IsRequired(false)                 
                     .OnDelete(DeleteBehavior.SetNull); 
                        });

            // Configure BlogComments
            modelBuilder.Entity<BlogComments>(entity =>
            {
                entity.HasIndex(e => e.BlogId)
                    .HasDatabaseName("idx_blog_comments_blog");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_blog_comments_user");

                entity.HasIndex(e => e.ParentCommentId)
                    .HasDatabaseName("idx_blog_comments_parent");

                entity.Property(e => e.IsApproved)
                    .HasDefaultValue(true);

                entity.Property(e => e.LikeCount)
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(e => e.UpdatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Blog)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlogComments)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ParentComment)
                    .WithMany(p => p.Replies)
                    .HasForeignKey(d => d.ParentCommentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure BlogLikes
            modelBuilder.Entity<BlogLikes>(entity =>
            {
                entity.HasIndex(e => new { e.BlogId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("idx_blog_likes_unique");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Blog)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.BlogId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BlogLikes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CommentLikes
            modelBuilder.Entity<CommentLikes>(entity =>
            {
                entity.HasIndex(e => new { e.CommentId, e.UserId })
                    .IsUnique()
                    .HasDatabaseName("idx_comment_likes_unique");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.Likes)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CommentLikes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure AuthorFollows
            modelBuilder.Entity<AuthorFollows>(entity =>
            {
                entity.HasIndex(e => new { e.FollowerId, e.AuthorId })
                    .IsUnique()
                    .HasDatabaseName("idx_author_follows_unique");

                entity.HasIndex(e => e.FollowerId)
                    .HasDatabaseName("idx_author_follows_follower");

                entity.HasIndex(e => e.AuthorId)
                    .HasDatabaseName("idx_author_follows_author");

                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(d => d.Follower)
                    .WithMany(p => p.Following)
                    .HasForeignKey(d => d.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.Followers)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
