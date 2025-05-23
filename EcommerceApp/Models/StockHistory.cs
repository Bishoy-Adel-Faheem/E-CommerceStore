using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    public class StockHistory
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int PreviousStock { get; set; }
        
        [Required]
        public int NewStock { get; set; }
        
        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        
        [Required]
        public string ChangedBy { get; set; } = string.Empty;
        
        public string Reason { get; set; } = string.Empty;
        
        // Transaction type: Manual, Order, Return, Adjustment, Initial, etc.
        public string TransactionType { get; set; } = "Manual";
        
        // Reference number (e.g., Order ID)
        public string? ReferenceNumber { get; set; }
        
        // Navigation property
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
