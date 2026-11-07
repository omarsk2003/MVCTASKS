using Microsoft.EntityFrameworkCore;
using TASK5CF.Models;

namespace TASK5CF.Data;

public class CafeteriaDbContext : DbContext
{
    public CafeteriaDbContext(
        DbContextOptions<CafeteriaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.Description)
                .HasMaxLength(255);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.Email)
                .HasMaxLength(150).IsRequired();

            entity.HasIndex(x => x.Email).IsUnique();

            entity.Property(x => x.Phone)
                .HasMaxLength(20);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(x => x.Name)
                .HasMaxLength(100).IsRequired();

            entity.Property(x => x.Price)
                .HasPrecision(10, 2);

            entity.Property(x => x.Quantity)
                .HasDefaultValue(0);

            entity.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Products_Price",
                    "[Price] >= 0");
                t.HasCheckConstraint("CK_Products_Quantity",
                    "[Quantity] >= 0");
            });
        });

        modelBuilder.Entity<PurchaseOrder>(entity =>
        {
            entity.Property(x => x.OrderDate)
                .HasDefaultValueSql("GETDATE()");

            entity.Property(x => x.Status)
                .HasMaxLength(30)
                .HasDefaultValue("Pending")
                .IsRequired();

            entity.ToTable(t =>
                t.HasCheckConstraint(
                    "CK_PurchaseOrders_Status",
                    "[Status] IN ('Pending','Approved','Received','Cancelled')"));
        });

        modelBuilder.Entity<PurchaseOrderItem>(entity =>
        {
            entity.Property(x => x.UnitPrice)
                .HasPrecision(10, 2);

            entity.HasIndex(x => new
            {
                x.PurchaseOrderId,
                x.ProductId
            }).IsUnique();

            entity.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_PurchaseOrderItems_Quantity",
                    "[Quantity] > 0");
                t.HasCheckConstraint(
                    "CK_PurchaseOrderItems_UnitPrice",
                    "[UnitPrice] >= 0");
            });
        });
        modelBuilder.Entity<PurchaseOrderItem>()
    .HasOne(x => x.PurchaseOrder)
    .WithMany(x => x.PurchaseOrderItems)
    .HasForeignKey(x => x.PurchaseOrderId)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PurchaseOrderItem>()
            .HasOne(x => x.Product)
            .WithMany(x => x.PurchaseOrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}