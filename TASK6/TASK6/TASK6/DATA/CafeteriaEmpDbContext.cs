
using Microsoft.EntityFrameworkCore;
using TASK6.Models;

namespace TASK6.DATA
{
    public class CafeteriaEmpDbContext : DbContext
    {
        public CafeteriaEmpDbContext(
            DbContextOptions<CafeteriaEmpDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(255);
            });

            // Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.EmailAddress)
                    .HasMaxLength(150);

                entity.HasIndex(x => x.EmailAddress)
                    .IsUnique();

                entity.Property(x => x.phoneNumber)
                    .HasMaxLength(20)
                    .IsRequired();
            });

            // Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Price)
                    .HasPrecision(10, 2);

                entity.Property(x => x.Quantity)
                    .HasDefaultValue(0);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Products_Price",
                        "[Price] >= 0");

                    t.HasCheckConstraint(
                        "CK_Products_Quantity",
                        "[Quantity] >= 0");
                });
            });
        }
    }
}
