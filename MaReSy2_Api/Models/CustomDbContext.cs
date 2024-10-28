using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MaReSy2_Api.Models;

public partial class MaReSyDbContext : DbContext
{
    public MaReSyDbContext()
    {
    }

    public MaReSyDbContext(DbContextOptions<MaReSyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductsSet> ProductsSets { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Set> Sets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename='C:\\Users\\Tobias\\OneDrive - BHAK BHAS Feldbach\\04_SJ_ HAK 2023_2024\\Diplomarbeit\\MaReSy2\\Datenbank\\database_maresy2\\database1_maresy2.mdf';Integrated Security=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__products__2D10D14A0E64F62F");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Productactive).HasColumnName("productactive");
            entity.Property(e => e.Productamount).HasColumnName("productamount");
            entity.Property(e => e.Productdescription)
                .HasMaxLength(200)
                .HasColumnName("productdescription");
            entity.Property(e => e.Productimage)
                .HasColumnType("image")
                .HasColumnName("productimage");
            entity.Property(e => e.Productname)
                .HasMaxLength(50)
                .HasColumnName("productname");
        });

        modelBuilder.Entity<ProductsSet>(entity =>
        {
            entity.HasKey(e => e.ProductSetId).HasName("PK__products__549422F29549BE71");

            entity.ToTable("productsSets");

            entity.Property(e => e.ProductSetId).HasColumnName("productSetID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Productamount).HasColumnName("productamount");
            entity.Property(e => e.SetId).HasColumnName("setID");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductsSets)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__productsS__produ__30F848ED");

            entity.HasOne(d => d.Set).WithMany(p => p.ProductsSets)
                .HasForeignKey(d => d.SetId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__productsS__setID__300424B4");
        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.RentalId).HasName("PK__rentals__016470CE8F37FF7A");

            entity.ToTable("rentals");

            entity.Property(e => e.RentalId).HasColumnName("rentalID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.RentalAmount).HasColumnName("rentalAmount");
            entity.Property(e => e.RentalCanceled)
                .HasColumnType("datetime")
                .HasColumnName("rentalCanceled");
            entity.Property(e => e.RentalCanceledUserName)
                .HasMaxLength(30)
                .HasColumnName("rentalCanceledUserName");
            entity.Property(e => e.RentalCreated)
                .HasColumnType("datetime")
                .HasColumnName("rentalCreated");
            entity.Property(e => e.RentalDelivery)
                .HasColumnType("datetime")
                .HasColumnName("rentalDelivery");
            entity.Property(e => e.RentalEnd)
                .HasColumnType("datetime")
                .HasColumnName("rentalEnd");
            entity.Property(e => e.RentalFree)
                .HasColumnType("datetime")
                .HasColumnName("rentalFree");
            entity.Property(e => e.RentalNote)
                .HasMaxLength(50)
                .HasColumnName("rentalNote");
            entity.Property(e => e.RentalReturned)
                .HasColumnType("datetime")
                .HasColumnName("rentalReturned");
            entity.Property(e => e.RentalStart)
                .HasColumnType("datetime")
                .HasColumnName("rentalStart");
            entity.Property(e => e.SetId).HasColumnName("setID");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Product).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__rentals__product__35BCFE0A");

            entity.HasOne(d => d.Set).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.SetId)
                .HasConstraintName("FK__rentals__setID__34C8D9D1");

            entity.HasOne(d => d.User).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rentals__userID__47DBAE45");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__CD98460AEB72E75E");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Rolename, "UQ__roles__4685A062D1142667").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(25)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Set>(entity =>
        {
            entity.HasKey(e => e.SetId).HasName("PK__sets__DA8A697AC2E50AAA");

            entity.ToTable("sets");

            entity.Property(e => e.SetId).HasColumnName("setID");
            entity.Property(e => e.Setactive).HasColumnName("setactive");
            entity.Property(e => e.Setdescription)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("setdescription");
            entity.Property(e => e.Setimage)
                .HasColumnType("image")
                .HasColumnName("setimage");
            entity.Property(e => e.Setname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("setname");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__CB9A1CDF860148EC");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E6164706FFDC8").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__users__F3DBC572FF69E555").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(25)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(25)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.Username)
                .HasMaxLength(30)
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__users__roleID__2B3F6F97");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
