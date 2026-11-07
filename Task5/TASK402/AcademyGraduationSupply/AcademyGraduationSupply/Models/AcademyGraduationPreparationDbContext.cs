using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AcademyGraduationSupply.Models;

public partial class AcademyGraduationPreparationDbContext : DbContext
{
    public AcademyGraduationPreparationDbContext(DbContextOptions<AcademyGraduationPreparationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<SelectedOffer> SelectedOffers { get; set; }

    public virtual DbSet<Supplier> Suppliers { get; set; }

    public virtual DbSet<SupplyRequest> SupplyRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A2B6346C800");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("PK__Offer__8EBCF0B115EA5E16");

            entity.ToTable("Offer");

            entity.HasIndex(e => e.SupplierId, "UQ__Offer__4BE66695DAA30F41").IsUnique();

            entity.Property(e => e.OfferId).HasColumnName("OfferID");
            entity.Property(e => e.OfferName)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Supplier).WithOne(p => p.Offer)
                .HasForeignKey<Offer>(d => d.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Offer__SupplierI__5070F446");
        });

        modelBuilder.Entity<SelectedOffer>(entity =>
        {
            entity.HasKey(e => e.SelectedOfferId).HasName("PK__Selected__EA39EDD6AC95F001");

            entity.ToTable("SelectedOffer");

            entity.HasIndex(e => new { e.RequestId, e.OfferId }, "UQ__Selected__3B439E90F6F1A3D3").IsUnique();

            entity.Property(e => e.SelectedOfferId).HasColumnName("SelectedOfferID");
            entity.Property(e => e.OfferId).HasColumnName("OfferID");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");

            entity.HasOne(d => d.Offer).WithMany(p => p.SelectedOffers)
                .HasForeignKey(d => d.OfferId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SelectedO__Offer__5812160E");

            entity.HasOne(d => d.Request).WithMany(p => p.SelectedOffers)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SelectedO__Reque__571DF1D5");
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId).HasName("PK__Supplier__4BE666947BED3271");

            entity.ToTable("Supplier");

            entity.Property(e => e.SupplierId).HasColumnName("SupplierID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Suppliers)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Supplier__Catego__4CA06362");
        });

        modelBuilder.Entity<SupplyRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__SupplyRe__33A8519A035061FC");

            entity.ToTable("SupplyRequest");

            entity.Property(e => e.RequestId).HasColumnName("RequestID");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
