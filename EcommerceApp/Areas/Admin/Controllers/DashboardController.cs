using EcommerceApp.Data;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Set the time periods for comparison (current month vs previous month)
            var today = DateTime.Today;
            var startOfCurrentMonth = new DateTime(today.Year, today.Month, 1);
            var startOfPreviousMonth = startOfCurrentMonth.AddMonths(-1);
            
            // Get all completed orders (not cancelled or returned)
            var completedOrders = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled && o.Status != OrderStatus.Returned)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();
            
            var model = new DashboardViewModel
            {
                // Sales Summary
                TotalSales = completedOrders.Sum(o => o.TotalAmount),
                TotalOrders = completedOrders.Count,
                AvgOrderValue = completedOrders.Any() ? completedOrders.Average(o => o.TotalAmount) : 0,
                
                // Products & Categories
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                LowStockProductsCount = await _context.Products.CountAsync(p => p.Stock > 0 && p.Stock <= 5),
                OutOfStockProductsCount = await _context.Products.CountAsync(p => p.Stock == 0),
                
                // Recent Orders
                RecentOrders = await _context.Orders
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Include(o => o.User)
                    .ToListAsync(),
                
                // Order Status Breakdown
                OrderStatusBreakdown = await _context.Orders
                    .GroupBy(o => o.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count)
            };
            
            // Calculate avg daily revenue (from the last 30 days)
            var last30DaysOrders = completedOrders.Where(o => o.OrderDate >= today.AddDays(-30)).ToList();
            model.AverageDailyRevenue = last30DaysOrders.Any() 
                ? last30DaysOrders.Sum(o => o.TotalAmount) / 30 
                : 0;
            
            // Period comparison (current month vs previous month)
            var currentMonthOrders = completedOrders.Where(o => o.OrderDate >= startOfCurrentMonth).ToList();
            var previousMonthOrders = completedOrders.Where(o => o.OrderDate >= startOfPreviousMonth && o.OrderDate < startOfCurrentMonth).ToList();
            
            model.SalesCurrentPeriod = currentMonthOrders.Sum(o => o.TotalAmount);
            model.SalesPreviousPeriod = previousMonthOrders.Sum(o => o.TotalAmount);
            
            // Calculate growth rate
            if (model.SalesPreviousPeriod > 0)
            {
                model.SalesGrowthRate = ((model.SalesCurrentPeriod - model.SalesPreviousPeriod) / model.SalesPreviousPeriod) * 100;
            }
            
            // Top Selling Products
            model.TopSellingProducts = await GetTopSellingProducts();
            
            // Sales Over Time - Daily (last 30 days)
            model.DailySalesData = GetDailySalesData(completedOrders, today.AddDays(-30), today);
            
            // Sales Over Time - Monthly (last 12 months)
            model.MonthlySalesData = GetMonthlySalesData(completedOrders, today.AddMonths(-11), today);
            
            return View(model);
        }

        private async Task<List<ProductPerformanceViewModel>> GetTopSellingProducts()
        {
            // Get all order items from completed orders
            var orderItems = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order.Status != OrderStatus.Cancelled && oi.Order.Status != OrderStatus.Returned)
                .ToListAsync();

            // Group by product and calculate total quantity and revenue
            return orderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new ProductPerformanceViewModel
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    ImageUrl = g.First().Product.ImageUrl,
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Price * oi.Quantity),
                    CurrentStock = g.First().Product.Stock
                })
                .OrderByDescending(p => p.TotalRevenue)
                .Take(5)
                .ToList();
        }

        private Dictionary<DateTime, decimal> GetDailySalesData(List<Order> orders, DateTime startDate, DateTime endDate)
        {
            var result = new Dictionary<DateTime, decimal>();
            
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dailySales = orders
                    .Where(o => o.OrderDate.Date == date)
                    .Sum(o => o.TotalAmount);
                
                result.Add(date, dailySales);
            }
            
            return result;
        }

        private Dictionary<string, decimal> GetMonthlySalesData(List<Order> orders, DateTime startDate, DateTime endDate)
        {
            var result = new Dictionary<string, decimal>();
            
            for (var date = new DateTime(startDate.Year, startDate.Month, 1); 
                 date <= new DateTime(endDate.Year, endDate.Month, 1); 
                 date = date.AddMonths(1))
            {
                var monthlyOrders = orders
                    .Where(o => o.OrderDate.Year == date.Year && o.OrderDate.Month == date.Month);
                
                var monthlySales = monthlyOrders.Sum(o => o.TotalAmount);
                var monthName = date.ToString("MMM yyyy");
                
                result.Add(monthName, monthlySales);
            }
            
            return result;
        }

        // Action method for product analysis
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
                
            return View(products);
        }

        // Action method for order analysis
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
                
            return View(orders);
        }

        // Action for inventory status
        public async Task<IActionResult> Inventory()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Stock)
                .ToListAsync();
            
            return View(products);
        }
    }
}