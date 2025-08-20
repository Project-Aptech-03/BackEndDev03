using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectDemoWebApi.Models;

namespace ProjectDemoWebApi.Data
{
    public class AuthDbContext: IdentityDbContext<Users>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {

        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    var entities = modelBuilder.Model.GetEntityTypes();
        //    foreach (var entityType in entities)
        //    {
        //        string? name = entityType.GetTableName();
        //        if (name != null && name.Length > 6) 
        //        {
        //            entityType.SetTableName(name.Substring(6));
        //        }
        //    }
        //}
    }
}
