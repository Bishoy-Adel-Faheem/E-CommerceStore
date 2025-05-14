using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
      public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        [Required]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        // Navigation property for user
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
        
        // Navigation property for order items
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
