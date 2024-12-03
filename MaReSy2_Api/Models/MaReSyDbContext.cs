using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

    public virtual DbSet<SingleProduct> SingleProducts { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\Users\\gindl\\OneDrive - BHAK BHAS Feldbach\\MaReSy2\\Git\\MaReSy2\\Datenbank\\database_maresy2\\database1_maresy2.mdf\";Integrated Security=True;Connect Timeout=30;Encrypt=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var intToBoolConvert = new ValueConverter<bool, int>(
v => v ? 1 : 0,
v => v == 1
);



        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__products__2D10D14A0E64F62F");

            entity.ToTable("products");

            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.ProductActive).HasColumnName("productActive").HasConversion(intToBoolConvert);
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
            entity.Property(e => e.SetId).HasColumnName("setID");
            entity.Property(e => e.SingleProductAmount).HasColumnName("singleProductAmount");

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
            entity.HasKey(e => e.RentalId).HasName("PK__tmp_ms_x__016470CED75488D3");

            entity.ToTable("rentals");

            entity.Property(e => e.RentalId).HasColumnName("rentalID");
            entity.Property(e => e.RentalAblehnung)
                .HasColumnType("datetime")
                .HasColumnName("rentalAblehnung");
            entity.Property(e => e.RentalAblehnungUser).HasColumnName("rentalAblehnungUser");
            entity.Property(e => e.RentalAmount).HasColumnName("rentalAmount");
            entity.Property(e => e.RentalAnforderung)
                .HasColumnType("datetime")
                .HasColumnName("rentalAnforderung");
            entity.Property(e => e.RentalAuslieferung)
                .HasColumnType("datetime")
                .HasColumnName("rentalAuslieferung");
            entity.Property(e => e.RentalAuslieferungUser).HasColumnName("rentalAuslieferungUser");
            entity.Property(e => e.RentalEnd)
                .HasColumnType("datetime")
                .HasColumnName("rentalEnd");
            entity.Property(e => e.RentalFreigabe)
                .HasColumnType("datetime")
                .HasColumnName("rentalFreigabe");
            entity.Property(e => e.RentalFreigabeUser).HasColumnName("rentalFreigabeUser");
            entity.Property(e => e.RentalNote)
                .HasMaxLength(50)
                .HasColumnName("rentalNote");
            entity.Property(e => e.RentalStart)
                .HasColumnType("datetime")
                .HasColumnName("rentalStart");
            entity.Property(e => e.RentalStornierung)
                .HasColumnType("datetime")
                .HasColumnName("rentalStornierung");
            entity.Property(e => e.RentalZurückgabe)
                .HasColumnType("datetime")
                .HasColumnName("rentalZurückgabe");
            entity.Property(e => e.RentalZurückgabeUser).HasColumnName("rentalZurückgabeUser");
            entity.Property(e => e.SingleProductId).HasColumnName("singleProductID");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.RentalAblehnungUserNavigation).WithMany(p => p.RentalRentalAblehnungUserNavigations)
                .HasForeignKey(d => d.RentalAblehnungUser)
                .HasConstraintName("FK__rentals__rentalA__70DDC3D8");

            entity.HasOne(d => d.RentalAuslieferungUserNavigation).WithMany(p => p.RentalRentalAuslieferungUserNavigations)
                .HasForeignKey(d => d.RentalAuslieferungUser)
                .HasConstraintName("FK__rentals__rentalA__71D1E811");

            entity.HasOne(d => d.RentalFreigabeUserNavigation).WithMany(p => p.RentalRentalFreigabeUserNavigations)
                .HasForeignKey(d => d.RentalFreigabeUser)
                .HasConstraintName("FK__rentals__rentalF__6FE99F9F");

            entity.HasOne(d => d.RentalZurückgabeUserNavigation).WithMany(p => p.RentalRentalZurückgabeUserNavigations)
                .HasForeignKey(d => d.RentalZurückgabeUser)
                .HasConstraintName("FK__rentals__rentalZ__72C60C4A");

            entity.HasOne(d => d.SingleProduct).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.SingleProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rentals__singleP__14270015");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rentals__status__73BA3083");

            entity.HasOne(d => d.User).WithMany(p => p.RentalUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__rentals__userID__6EF57B66");
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
            entity.Property(e => e.Setactive).HasColumnName("setactive")
            .HasConversion(intToBoolConvert);
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

        modelBuilder.Entity<SingleProduct>(entity =>
        {
            entity.HasKey(e => e.SingleProductId).HasName("PK__singlePr__DE7B6268174A8667");

            entity.ToTable("singleProducts");

            entity.Property(e => e.SingleProductId).HasColumnName("singleProductID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.SingleProductActive).HasColumnName("singleProductActive").HasConversion(intToBoolConvert);
            entity.Property(e => e.SingleProductName)
                .HasMaxLength(50)
                .HasColumnName("singleProductName");
            entity.Property(e => e.SingleProductNumber)
                .HasMaxLength(50)
                .HasColumnName("singleProductNumber");

            entity.HasOne(d => d.Product).WithMany(p => p.SingleProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__singlePro__produ__02FC7413");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC07BC5C7D48");

            entity.ToTable("status");

            entity.Property(e => e.Bezeichnung)
                .HasMaxLength(25)
                .HasColumnName("bezeichnung");
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
