using System;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{
    public class OrderNote
    {
        public int Id { get; set; }
        
        public int OrderId { get; set; }
        
        public Order Order { get; set; } = null!;
        
        [Required]
        public string Note { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [Required]
        public string CreatedBy { get; set; } = string.Empty;
        
        public bool IsInternal { get; set; } = true;
    }
}
