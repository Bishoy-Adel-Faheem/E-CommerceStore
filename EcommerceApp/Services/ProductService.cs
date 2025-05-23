using System;
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
        }        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id) ?? throw new ArgumentException($"Product with ID {id} not found", nameof(id));
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

        // New inventory management methods
        public async Task<Product> UpdateProductStockAsync(int productId, int newStock, string username, string reason = "", string transactionType = "Manual")
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new ArgumentException($"Product with ID {productId} not found", nameof(productId));

            // Create stock history record
            var stockHistory = new StockHistory
            {
                ProductId = productId,
                PreviousStock = product.Stock,
                NewStock = newStock,
                ChangedAt = DateTime.Now,
                ChangedBy = username,
                Reason = reason,
                TransactionType = transactionType
            };

            // Update product stock
            product.Stock = newStock;

            // Save changes
            _context.StockHistory.Add(stockHistory);
            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<IEnumerable<StockHistory>> GetStockHistoryByProductIdAsync(int productId)
        {
            return await _context.StockHistory
                .Where(sh => sh.ProductId == productId)
                .OrderByDescending(sh => sh.ChangedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5)
        {
            return await _context.Products
                .Where(p => p.Stock <= threshold && p.Stock > 0)
                .Include(p => p.Category)
                .OrderBy(p => p.Stock)
                .ToListAsync();
        }

        public async Task<bool> BulkUpdateStockAsync(Dictionary<int, int> productStockUpdates, string username, string reason = "")
        {
            try
            {
                foreach (var update in productStockUpdates)
                {
                    int productId = update.Key;
                    int newStock = update.Value;

                    var product = await _context.Products.FindAsync(productId);
                    if (product != null)
                    {
                        var stockHistory = new StockHistory
                        {
                            ProductId = productId,
                            PreviousStock = product.Stock,
                            NewStock = newStock,
                            ChangedAt = DateTime.Now,
                            ChangedBy = username,
                            Reason = reason,
                            TransactionType = "Bulk Update"
                        };

                        product.Stock = newStock;
                        _context.StockHistory.Add(stockHistory);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
