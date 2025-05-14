using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;

namespace EcommerceApp.Services
{    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, CheckoutViewModel checkoutViewModel);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId, string? userId = null);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}
