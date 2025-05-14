using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Models
{    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Navigation property for products in this category
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
