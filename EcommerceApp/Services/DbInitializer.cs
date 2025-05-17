using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Identity;                       
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Services
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created and migrations are applied
            context.Database.Migrate();

            // Create roles if they don't exist
            var roles = new[] { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user if it doesn't exist
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "123 Admin St",
                    City = "Admin City",
                    PostalCode = "12345",
                    Country = "USA",
                    EmailConfirmed = true
                };

                var password = "Admin@123";
                var result = await userManager.CreateAsync(adminUser, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create a custom admin user (you can modify these details)
            var customAdminEmail = "youremail@example.com";
            var customAdminUser = await userManager.FindByEmailAsync(customAdminEmail);

            if (customAdminUser == null)
            {
                customAdminUser = new ApplicationUser
                {
                    UserName = customAdminEmail,
                    Email = customAdminEmail,
                    FirstName = "Your",
                    LastName = "Name",
                    Address = "Your Address",
                    City = "Your City",
                    PostalCode = "Your Postal Code",
                    Country = "Your Country",
                    EmailConfirmed = true
                };

                var customPassword = "YourPassword@123"; // Choose a strong password
                var customResult = await userManager.CreateAsync(customAdminUser, customPassword);

                if (customResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(customAdminUser, "Admin");
                }
            }

            // Seed categories if none exist
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
                    new Category { Name = "Clothing", Description = "Apparel and fashion items" },
                    new Category { Name = "Home & Garden", Description = "Items for home and garden" },
                    new Category { Name = "Sports", Description = "Sports equipment and accessories" },
                    new Category { Name = "Books", Description = "Books and literature" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed products if none exist
            if (!await context.Products.AnyAsync())
            {
                var categories = await context.Categories.ToListAsync();
                var products = new List<Product>();

                // Electronics Products
                var electronics = categories.FirstOrDefault(c => c.Name == "Electronics");
                if (electronics != null)
                {
                    products.AddRange(new List<Product>
                    {                        new Product                        {                            Name = "Smartphone",
                            Description = "Latest model smartphone with advanced features",
                            Price = 799.99m,
                            Stock = 50,
                            ImageUrl = "/images/smartphone.jpg",
                            IsFeatured = true,
                            CategoryId = electronics.Id
                        },
                        new Product
                        {
                            Name = "Laptop",
                            Description = "Powerful laptop for work and gaming",                            Price = 1299.99m,
                            OldPrice = 1499.99m,
                            Stock = 25,
                            ImageUrl = "/images/laptop.jpg",
                            IsFeatured = true,
                            CategoryId = electronics.Id
                        },
                        new Product
                        {
                            Name = "Headphones",
                            Description = "Wireless noise-cancelling headphones",                            Price = 199.99m,
                            Stock = 100,
                            ImageUrl = "/images/headphones.jpg",
                            IsFeatured = false,
                            CategoryId = electronics.Id
                        }
                    });
                }

                // Clothing Products
                var clothing = categories.FirstOrDefault(c => c.Name == "Clothing");
                if (clothing != null)
                {
                    products.AddRange(new List<Product>
                    {
                        new Product
                        {
                            Name = "T-Shirt",
                            Description = "Comfortable cotton t-shirt",                            Price = 19.99m,
                            Stock = 200,
                            ImageUrl = "/images/tshirt.jpg",
                            IsFeatured = false,
                            CategoryId = clothing.Id
                        },
                        new Product
                        {
                            Name = "Jeans",
                            Description = "Classic blue jeans",                            Price = 49.99m,
                            OldPrice = 59.99m,
                            Stock = 150,
                            ImageUrl = "/images/jeans.jpg",
                            IsFeatured = true,
                            CategoryId = clothing.Id
                        }
                    });
                }

                // Add products from other categories
                var homeGarden = categories.FirstOrDefault(c => c.Name == "Home & Garden");
                if (homeGarden != null)
                {
                    products.AddRange(new List<Product>
                    {
                        new Product
                        {
                            Name = "Coffee Table",
                            Description = "Modern coffee table for living room",                            Price = 129.99m,
                            Stock = 30,
                            ImageUrl = "/images/coffeetable.jpg",
                            IsFeatured = false,
                            CategoryId = homeGarden.Id
                        }
                    });
                }

                var sports = categories.FirstOrDefault(c => c.Name == "Sports");
                if (sports != null)
                {
                    products.AddRange(new List<Product>
                    {
                        new Product
                        {
                            Name = "Basketball",
                            Description = "Official size basketball",                            Price = 29.99m,
                            Stock = 50,
                            ImageUrl = "/images/basketball.jpg",
                            IsFeatured = false,
                            CategoryId = sports.Id
                        }
                    });
                }

                var books = categories.FirstOrDefault(c => c.Name == "Books");
                if (books != null)
                {
                    products.AddRange(new List<Product>
                    {
                        new Product
                        {
                            Name = "Programming Book",
                            Description = "Learn to code with this comprehensive guide",                            Price = 39.99m,
                            Stock = 75,
                            ImageUrl = "/images/programmingbook.jpg",
                            IsFeatured = true,
                            CategoryId = books.Id
                        }
                    });
                }

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
