using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;

namespace EcommerceApp.Services
{    public interface ICartService
    {
        Task<string> GetCartIdAsync(string? userId = null);
        Task<int> AddToCartAsync(int productId, int quantity, string cartId);
        Task<int> RemoveFromCartAsync(int cartItemId);
        Task<int> UpdateCartItemAsync(int cartItemId, int quantity);
        Task<CartViewModel> GetCartAsync(string cartId);
        Task ClearCartAsync(string cartId);
        Task MigrateCartAsync(string anonymousCartId, string userId);
        Task<int> GetCartItemCountAsync(string cartId);
    }
}
