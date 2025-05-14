using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EcommerceApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public OrderService(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderAsync(string userId, CheckoutViewModel checkoutViewModel)
        {
            // Get cart
            var cartViewModel = await _cartService.GetCartAsync(userId);
            if (cartViewModel.Items.Count == 0)
                return null;

            // Create new order
            var order = new Order
            {
                UserId = userId,
                OrderDate = System.DateTime.Now,
                ShippingAddress = checkoutViewModel.ShippingAddress,
                City = checkoutViewModel.City,
                PostalCode = checkoutViewModel.PostalCode,
                Country = checkoutViewModel.Country,
                PhoneNumber = checkoutViewModel.PhoneNumber,
                Email = checkoutViewModel.Email,
                TotalAmount = cartViewModel.Total,
                Status = OrderStatus.Pending,
                OrderItems = new List<OrderItem>()
            };

            // Add order items
            foreach (var item in cartViewModel.Items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };
                    order.OrderItems.Add(orderItem);

                    // Update stock
                    product.Stock -= item.Quantity;
                    _context.Entry(product).State = EntityState.Modified;
                }
            }

            // Save order to database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Clear cart
            await _cartService.ClearCartAsync(userId);

            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }        public async Task<Order?> GetOrderByIdAsync(int orderId, string? userId = null)
        {
            if (userId != null)
            {
                // Regular user can only view their own orders
                return await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
            }
            else
            {
                // Admin can view all orders
                return await _context.Orders
                    .Include(o => o.User)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            order.Status = status;
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
