using System.Collections.Generic;
using System.Threading.Tasks;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;

namespace EcommerceApp.Services
{    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, CheckoutViewModel checkoutViewModel);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string? userId = null);
        Task<Order?> GetOrderByIdAsync(int orderId, string? userId = null);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status, string? adminUsername = null);
        Task<bool> UpdateTrackingInfoAsync(int orderId, string trackingNumber, string shippingProvider);
        Task<OrderNote> AddOrderNoteAsync(int orderId, string note, string createdBy, bool isInternal = true);
        Task<IEnumerable<OrderNote>> GetOrderNotesAsync(int orderId, bool includeInternal = true);
    }
}
