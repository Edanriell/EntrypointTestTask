using Microsoft.EntityFrameworkCore;

// JWT
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Server.Constants;
using System.Security.Principal;

namespace Server.Entities
{
    //public class ApplicationDbContext : DbContext
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fix for decimal
            // modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18, 2)");

            modelBuilder
                .Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductOrderLink>().HasKey(p => new { p.OrderId, p.ProductId });

            modelBuilder
                .Entity<ProductOrderLink>()
                .HasOne(p => p.Order)
                .WithMany(o => o.OrderProducts)
                .HasForeignKey(p => p.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .Entity<ProductOrderLink>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductOrders)
                .HasForeignKey(p => p.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductOrderLink> ProductOrderLink => Set<ProductOrderLink>();
    }
}
