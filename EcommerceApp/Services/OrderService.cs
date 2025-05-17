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
        }        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string? userId = null)
        {
            var query = _context.Orders.AsQueryable();
            
            if (userId != null)
            {
                query = query.Where(o => o.UserId == userId);
            }
            
            return await query
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }public async Task<Order?> GetOrderByIdAsync(int orderId, string? userId = null)
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
        }        public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status, string? adminUsername = null)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;

            // Save the previous status for note
            var previousStatus = order.Status;
            
            // Update status and related date fields
            order.Status = status;
            
            // Update date fields based on status
            switch (status)
            {
                case OrderStatus.Processing:
                    order.ProcessedDate = DateTime.Now;
                    break;
                case OrderStatus.Shipped:
                    order.ShippedDate = DateTime.Now;
                    break;
                case OrderStatus.Delivered:
                    order.DeliveredDate = DateTime.Now;
                    break;
            }
            
            _context.Entry(order).State = EntityState.Modified;
            
            // Add a note about the status change
            if (adminUsername != null)
            {
                var note = new OrderNote
                {
                    OrderId = orderId,
                    Note = $"Order status changed from {previousStatus} to {status}",
                    CreatedBy = adminUsername,
                    IsInternal = true
                };
                _context.OrderNotes.Add(note);
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<bool> UpdateTrackingInfoAsync(int orderId, string trackingNumber, string shippingProvider)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return false;
                
            order.TrackingNumber = trackingNumber;
            order.ShippingProvider = shippingProvider;
            
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<OrderNote> AddOrderNoteAsync(int orderId, string note, string createdBy, bool isInternal = true)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found", nameof(orderId));
                
            var orderNote = new OrderNote
            {
                OrderId = orderId,
                Note = note,
                CreatedBy = createdBy,
                IsInternal = isInternal
            };
            
            _context.OrderNotes.Add(orderNote);
            await _context.SaveChangesAsync();
            return orderNote;
        }
        
        public async Task<IEnumerable<OrderNote>> GetOrderNotesAsync(int orderId, bool includeInternal = true)
        {
            var query = _context.OrderNotes.Where(n => n.OrderId == orderId);
            
            if (!includeInternal)
                query = query.Where(n => !n.IsInternal);
                
            return await query.OrderByDescending(n => n.CreatedAt).ToListAsync();
        }
    }
}
