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
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductImage> ProductImage { get; set; } = null!;

    }
}
