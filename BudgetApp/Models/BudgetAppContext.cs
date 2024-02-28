using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BudgetApp.Models;

public partial class BudgetAppContext : DbContext
{
    public BudgetAppContext()
    {
    }

    public BudgetAppContext(DbContextOptions<BudgetAppContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BudgetCategory> BudgetCategories { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("server=(localdb)\\MSSQLLocalDB;database=BudgetApp;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BudgetCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryName).HasName("PK__BudgetCa__8517B2E1B05588F2");

            entity.ToTable("BudgetCategory");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.TransId).HasName("PK__Transact__9E5DDB1C0DE6E6AD");

            entity.ToTable("Transaction");

            entity.Property(e => e.TransId).HasColumnName("TransID");
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TransName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.CategoryNameNavigation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.CategoryName)
                .HasConstraintName("FK_Transaction_BudgetCategory");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
