using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCreator.Models
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Property> Properties { get; set; }

        public DbSet<FileModel> Files { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductProperty> ProductProperties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<ProductCategory>()
                .HasKey(t => new { t.ProductId, t.CategoryId });

            modelBuilder.Entity<ProductCategory>()
                .HasOne(sc => sc.Product)
                .WithMany(s => s.ProductCategories)
                .HasForeignKey(sc => sc.ProductId);

            modelBuilder.Entity<ProductCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.ProductCategories)
                .HasForeignKey(sc => sc.CategoryId);



            modelBuilder.Entity<ProductProperty>()
                .HasKey(t => new { t.ProductId, t.PropertyId });

            modelBuilder.Entity<ProductProperty>()
                .HasOne(sc => sc.Product)
                .WithMany(s => s.ProductProperties)
                .HasForeignKey(sc => sc.ProductId);

           

            modelBuilder.Entity<ProductProperty>()
                .HasOne(sc => sc.Property)
                .WithMany(c => c.ProductProperties)
                .HasForeignKey(sc => sc.PropertyId);

            modelBuilder.Entity<Product>()
              .HasOne(sc => sc.Image);

            modelBuilder.Entity<Property>()
             .Ignore(p=>p.GetPropertyTypes);

            modelBuilder.Entity<Category>()
                .HasOne(pc => pc.ParentCategory)
                .WithMany(p => p.ChildCategories);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=relationsdb;Trusted_Connection=True;");
        }


        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

