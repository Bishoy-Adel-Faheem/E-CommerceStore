using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EcommerceApp.Services;
using EcommerceApp.Models;
using System.Threading.Tasks;
using EcommerceApp.ViewModels;
using System.Linq;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }        
        public async Task<IActionResult> Index(OrderStatus? status = null)
        {
            // Get all orders without specifying user ID (admin can see all orders)
            var orders = await _orderService.GetOrdersByUserIdAsync(null);
            
            // Filter by status if specified
            if (status.HasValue)
            {
                orders = orders.Where(o => o.Status == status.Value).ToList();
                ViewData["CurrentFilter"] = status.Value;
            }
            
            return View(orders);
        }        
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Get order notes
            var notes = await _orderService.GetOrderNotesAsync(id);

            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderNotes = notes.ToList()
            };
            
            return View(viewModel);
        }        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            // Get the admin username for tracking
            var username = User.Identity?.Name ?? "Admin";
            
            var success = await _orderService.UpdateOrderStatusAsync(id, status, username);
            if (!success)
            {
                return NotFound();
            }

            TempData["StatusMessage"] = $"Order status has been updated to {status}.";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(int id, string note, bool isInternal = true)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                TempData["ErrorMessage"] = "Note cannot be empty.";
                return RedirectToAction(nameof(Details), new { id });
            }
            
            var username = User.Identity?.Name ?? "Admin";
            
            try
            {
                await _orderService.AddOrderNoteAsync(id, note, username, isInternal);
                TempData["StatusMessage"] = "Note added successfully.";
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            
            return RedirectToAction(nameof(Details), new { id });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTracking(int id, string trackingNumber, string shippingProvider)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber) || string.IsNullOrWhiteSpace(shippingProvider))
            {
                TempData["ErrorMessage"] = "Tracking number and shipping provider are required.";
                return RedirectToAction(nameof(Details), new { id });
            }
            
            var success = await _orderService.UpdateTrackingInfoAsync(id, trackingNumber, shippingProvider);
            if (!success)
            {
                return NotFound();
            }
            
            // Add a note about the tracking update
            var username = User.Identity?.Name ?? "Admin";
            await _orderService.AddOrderNoteAsync(id, $"Tracking information added: {trackingNumber} ({shippingProvider})", username, true);
            
            TempData["StatusMessage"] = "Tracking information updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
