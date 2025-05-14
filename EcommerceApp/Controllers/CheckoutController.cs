using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceApp.Services;
using EcommerceApp.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using EcommerceApp.Models;

namespace EcommerceApp.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(
            ICartService cartService,
            IOrderService orderService,
            UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _orderService = orderService;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var cartId = await _cartService.GetCartIdAsync(user.Id);
            var cart = await _cartService.GetCartAsync(cartId);

            if (cart == null || cart.Items.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var checkoutViewModel = new CheckoutViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ShippingAddress = user.Address,
                City = user.City,
                PostalCode = user.PostalCode,
                Country = user.Country,
                Cart = cart
            };

            return View(checkoutViewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var cartId = await _cartService.GetCartIdAsync(userId);
                model.Cart = await _cartService.GetCartAsync(cartId);
                return View("Index", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var order = await _orderService.CreateOrderAsync(user.Id, model);
            if (order == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            return RedirectToAction(nameof(OrderConfirmation), new { orderId = order.Id });
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var userId = _userManager.GetUserId(User);
            var order = await _orderService.GetOrderByIdAsync(orderId, userId);
            
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
