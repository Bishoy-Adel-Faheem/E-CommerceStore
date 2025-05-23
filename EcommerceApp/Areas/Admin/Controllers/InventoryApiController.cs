using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Services;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class InventoryApiController : ControllerBase
    {
        private readonly IProductService _productService;

        public InventoryApiController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("updateStock")]
        public async Task<IActionResult> UpdateStock(StockUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var username = User.Identity?.Name ?? "Admin";
                var product = await _productService.UpdateProductStockAsync(
                    model.ProductId, 
                    model.NewStock, 
                    username, 
                    model.Reason
                );

                return Ok(new { 
                    success = true, 
                    productId = product.Id, 
                    newStock = product.Stock,
                    stockStatus = GetStockStatusInfo(product.Stock)
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost("bulkUpdate")]
        public async Task<IActionResult> BulkUpdateStock(BulkStockUpdateViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var username = User.Identity?.Name ?? "Admin";
                var success = await _productService.BulkUpdateStockAsync(
                    model.ProductStockUpdates, 
                    username, 
                    model.Reason
                );

                if (success)
                {
                    return Ok(new { success = true });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Failed to update some stock levels." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }        [HttpGet("stockHistory/{productId}")]
        public async Task<IActionResult> GetStockHistory(int productId)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(productId);
                var history = await _productService.GetStockHistoryByProductIdAsync(productId);
                
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                // Map to simplified model to avoid circular references
                var historyItems = history.Select(h => new {
                    id = h.Id,
                    productId = h.ProductId,
                    previousStock = h.PreviousStock,
                    newStock = h.NewStock,
                    changedAt = h.ChangedAt,
                    changedBy = h.ChangedBy,
                    reason = h.Reason,
                    transactionType = h.TransactionType,
                    referenceNumber = h.ReferenceNumber
                }).ToList();
                
                var viewModel = new StockHistoryViewModel
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    ProductImageUrl = product.ImageUrl,
                    History = history.ToList() // Original is still needed for the view model
                };
                
                // Return a cleaned object that won't cause circular references
                return Ok(new {
                    productId = viewModel.ProductId,
                    productName = viewModel.ProductName,
                    productImageUrl = viewModel.ProductImageUrl,
                    history = historyItems
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private object GetStockStatusInfo(int stock)
        {
            string status;
            string badgeClass;

            if (stock <= 0)
            {
                status = "Out of Stock";
                badgeClass = "bg-danger";
            }
            else if (stock <= 5)
            {
                status = "Low Stock";
                badgeClass = "bg-warning text-dark";
            }
            else
            {
                status = "In Stock";
                badgeClass = "bg-success";
            }

            return new
            {
                text = status,
                badgeClass = badgeClass,
                stockLevel = stock <= 0 ? "out-of-stock" : (stock <= 5 ? "low-stock" : "in-stock")
            };
        }
    }
}
