using EcommerceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data if needed
            // SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Example of seeding categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic items and gadgets" },
                new Category { Id = 2, Name = "Clothing", Description = "Apparel and fashion items" },
                new Category { Id = 3, Name = "Books", Description = "Books of all genres" }
            );

            // Example of seeding products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Smartphone",
                    Description = "Latest smartphone with advanced features",
                    Price = 699.99m,
                    Stock = 50,
                    CategoryId = 1,
                    ImageUrl = "/images/products/smartphone.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Laptop",
                    Description = "High-performance laptop for productivity",
                    Price = 1299.99m,
                    Stock = 30,
                    CategoryId = 1,
                    ImageUrl = "/images/products/laptop.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "T-Shirt",
                    Description = "Comfortable cotton t-shirt",
                    Price = 19.99m,
                    Stock = 100,
                    CategoryId = 2,
                    ImageUrl = "/images/products/tshirt.jpg"
                }
            );
        }
    }
}
