using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using EcommerceApp.Models;

namespace EcommerceApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(
            ICartService cartService,
            IProductService productService,
            UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _productService = productService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartId = await _cartService.GetCartIdAsync(userId);
            var cart = await _cartService.GetCartAsync(cartId);
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0)
                quantity = 1;

            var userId = _userManager.GetUserId(User);
            var cartId = await _cartService.GetCartIdAsync(userId);
            
            await _cartService.AddToCartAsync(productId, quantity, cartId);

            // If this is an AJAX request, return a partial result
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var cartItemCount = await _cartService.GetCartItemCountAsync(cartId);
                return Json(new { success = true, count = cartItemCount });
            }

            // Otherwise redirect back to the product page
            return RedirectToAction("Details", "Shop", new { id = productId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id)
        {
            await _cartService.RemoveFromCartAsync(id);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var userId = _userManager.GetUserId(User);
                var cartId = await _cartService.GetCartIdAsync(userId);
                var cart = await _cartService.GetCartAsync(cartId);
                return Json(new { success = true, total = cart.Total });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            if (quantity <= 0)
                quantity = 1;

            await _cartService.UpdateCartItemAsync(id, quantity);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var userId = _userManager.GetUserId(User);
                var cartId = await _cartService.GetCartIdAsync(userId);
                var cart = await _cartService.GetCartAsync(cartId);
                var item = cart.Items.FirstOrDefault(i => i.Id == id);
                
                return Json(new { 
                    success = true, 
                    subtotal = item?.Subtotal,
                    total = cart.Total
                });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]        public async Task<IActionResult> ClearCart()
        {
            var userId = _userManager.GetUserId(User);
            var cartId = await _cartService.GetCartIdAsync(userId);
            await _cartService.ClearCartAsync(cartId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = _userManager.GetUserId(User);
            var cartId = await _cartService.GetCartIdAsync(userId);
            var count = await _cartService.GetCartItemCountAsync(cartId);
            return Json(new { count });
        }
    }
}
