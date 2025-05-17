using EcommerceApp.Models;
using System;
using System.Collections.Generic;

namespace EcommerceApp.ViewModels
{
    public class DashboardViewModel
    {
        // Sales Summary
        public decimal TotalSales { get; set; }
        public decimal AverageDailyRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AvgOrderValue { get; set; }
        
        // Period Comparisons
        public decimal SalesCurrentPeriod { get; set; }
        public decimal SalesPreviousPeriod { get; set; }
        public decimal SalesGrowthRate { get; set; }
        
        // Products & Categories
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int LowStockProductsCount { get; set; }
        public int OutOfStockProductsCount { get; set; }
        
        // Recent Data
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<ProductPerformanceViewModel> TopSellingProducts { get; set; } = new List<ProductPerformanceViewModel>();
        
        // Order Status Breakdown
        public Dictionary<OrderStatus, int> OrderStatusBreakdown { get; set; } = new Dictionary<OrderStatus, int>();
        
        // Sales Over Time
        public Dictionary<DateTime, decimal> DailySalesData { get; set; } = new Dictionary<DateTime, decimal>();
        public Dictionary<string, decimal> MonthlySalesData { get; set; } = new Dictionary<string, decimal>();
    }
    
    public class ProductPerformanceViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int CurrentStock { get; set; }
    }
    
    public class SalesByPeriodViewModel
    {
        public string Period { get; set; } = string.Empty;
        public decimal Sales { get; set; }
    }
    
    public class SalesByCategoryViewModel
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalSales { get; set; }
        public int ProductCount { get; set; }
        public double Percentage { get; set; }
    }
}