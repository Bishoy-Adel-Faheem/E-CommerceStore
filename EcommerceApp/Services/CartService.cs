using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;

namespace EcommerceApp.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public Task<string> GetCartIdAsync(string? userId = null)
        {
            // If userId is provided (user is logged in), use it as cart ID
            if (!string.IsNullOrEmpty(userId))
                return Task.FromResult(userId);

            // For anonymous users, use session-based cart ID
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
            {
                return Task.FromResult(Guid.NewGuid().ToString());
            }            string? cartId = Microsoft.AspNetCore.Http.SessionExtensions.GetString(session, "CartId");

            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                Microsoft.AspNetCore.Http.SessionExtensions.SetString(session, "CartId", cartId);
            }

            return Task.FromResult(cartId);
        }

        public async Task<int> AddToCartAsync(int productId, int quantity, string cartId)
        {
            // Check if product exists
            var product = await _context.Products.FindAsync(productId);
            if (product == null || quantity <= 0)
                return 0;

            // Check if item already in cart
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

            if (cartItem == null)
            {
                // Add new item to cart
                cartItem = new CartItem
                {
                    CartId = cartId,
                    ProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }
            else
            {
                // Update quantity
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            return await GetCartItemCountAsync(cartId);
        }

        public async Task<int> RemoveFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return 0;

            string cartId = cartItem.CartId;
            
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return await GetCartItemCountAsync(cartId);
        }

        public async Task<int> UpdateCartItemAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null || quantity <= 0)
                return 0;

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            return await GetCartItemCountAsync(cartItem.CartId);
        }

        public async Task<CartViewModel> GetCartAsync(string cartId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.CartId == cartId)
                .Include(c => c.Product)
                .ToListAsync();

            var cartItemViewModels = cartItems.Select(item => new CartItemViewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductImageUrl = item.Product.ImageUrl,
                Price = item.Product.Price,
                Quantity = item.Quantity,
                Subtotal = item.Product.Price * item.Quantity
            }).ToList();

            var cartViewModel = new CartViewModel
            {
                Items = cartItemViewModels,
                Total = cartItemViewModels.Sum(i => i.Subtotal)
            };

            return cartViewModel;
        }

        public async Task ClearCartAsync(string cartId)
        {
            var cartItems = await _context.CartItems
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }

        public async Task MigrateCartAsync(string anonymousCartId, string userId)
        {
            var anonymousCartItems = await _context.CartItems
                .Where(c => c.CartId == anonymousCartId)
                .ToListAsync();

            foreach (var item in anonymousCartItems)
            {
                var userCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.CartId == userId && c.ProductId == item.ProductId);

                if (userCartItem == null)
                {
                    // Add the item to the user's cart
                    var newUserCartItem = new CartItem
                    {
                        CartId = userId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    _context.CartItems.Add(newUserCartItem);
                }
                else
                {
                    // Update quantity if item already exists in user cart
                    userCartItem.Quantity += item.Quantity;
                }
            }

            // Remove anonymous cart items
            _context.CartItems.RemoveRange(anonymousCartItems);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemCountAsync(string cartId)
        {
            return await _context.CartItems
                .Where(c => c.CartId == cartId)
                .SumAsync(c => c.Quantity);
        }
    }
}
