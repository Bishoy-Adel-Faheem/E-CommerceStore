using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcommerceApp.Models;

namespace EcommerceApp.ViewModels
{
    public class StockUpdateViewModel
    {
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock must be a non-negative number")]
        public int NewStock { get; set; }
        
        public string Reason { get; set; } = string.Empty;
    }
    
    public class BulkStockUpdateViewModel
    {
        public Dictionary<int, int> ProductStockUpdates { get; set; } = new Dictionary<int, int>();
        
        public string Reason { get; set; } = string.Empty;
    }
    
    public class StockHistoryViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImageUrl { get; set; } = string.Empty;
        public List<StockHistory> History { get; set; } = new List<StockHistory>();
    }
    
    public class InventoryReportViewModel
    {
        public int TotalProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int HealthyStockProducts { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public List<CategoryStockSummary> StockByCategory { get; set; } = new List<CategoryStockSummary>();
        public List<RecentStockChange> RecentChanges { get; set; } = new List<RecentStockChange>();
    }
    
    public class CategoryStockSummary
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int LowStockProducts { get; set; }
    }
    
    public class RecentStockChange
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int PreviousStock { get; set; }
        public int NewStock { get; set; }
        public DateTime ChangedAt { get; set; }
        public string ChangedBy { get; set; } = string.Empty;
    }
}
