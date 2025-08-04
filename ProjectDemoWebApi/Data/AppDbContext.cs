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
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        //    // Configure entity relationships and constraints here if needed
        //}
        //// DbSet properties for your entities
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImage { get; set; } = null!;

    }
}
