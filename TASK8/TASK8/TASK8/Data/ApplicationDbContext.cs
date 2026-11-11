
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TASK8.Models;

namespace TASK8.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<SupplyRequest> SupplyRequests { get; set; }
        public DbSet<RequestItem> RequestItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Entity<SupplyRequest>()
                .Property(r => r.TotalPrice)
                .HasPrecision(10, 2);

            builder.Entity<RequestItem>()
                .Property(i => i.Price)
                .HasPrecision(10, 2);

            builder.Entity<SupplyRequest>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<RequestItem>()
                .HasOne(i => i.SupplyRequest)
                .WithMany(r => r.RequestItems)
                .HasForeignKey(i => i.SupplyRequestId);

            builder.Entity<RequestItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
