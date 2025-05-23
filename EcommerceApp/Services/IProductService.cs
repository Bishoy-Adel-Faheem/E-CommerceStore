using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceApp.Models;

namespace EcommerceApp.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> FilterProductsAsync(int? categoryId, decimal? minPrice, decimal? maxPrice, bool? inStock, string sortBy);
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
        
        // Inventory management methods
        Task<Product> UpdateProductStockAsync(int productId, int newStock, string username, string reason = "", string transactionType = "Manual");
        Task<IEnumerable<StockHistory>> GetStockHistoryByProductIdAsync(int productId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 5);
        Task<bool> BulkUpdateStockAsync(Dictionary<int, int> productStockUpdates, string username, string reason = "");
    }
}
