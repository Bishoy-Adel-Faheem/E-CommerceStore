using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EcommerceApp.Data;
using EcommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return await GetAllProductsAsync();

            return await _context.Products
                .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, bool? inStock, string sortBy)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Apply category filter
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            // Apply price range filters
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // Apply stock filter
            if (inStock.HasValue && inStock.Value)
                query = query.Where(p => p.Stock > 0);

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "price_asc" => query.OrderBy(p => p.Price),
                    "price_desc" => query.OrderByDescending(p => p.Price),
                    "name_asc" => query.OrderBy(p => p.Name),
                    "name_desc" => query.OrderByDescending(p => p.Name),
                    "newest" => query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Name)
                };
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            return await query.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
