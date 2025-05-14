using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{    public class CartItem
    {
        public int Id { get; set; }
        
        [Required]
        public string CartId { get; set; } = string.Empty; // This will be the session ID
        
        [Required]
        public int ProductId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property for product
        public virtual Product Product { get; set; } = null!;
    }
}
