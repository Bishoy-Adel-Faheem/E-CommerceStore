using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceApp.Models
{    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Display(Name = "Old Price")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OldPrice { get; set; }
        
        [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;
        
        public int Stock { get; set; }
        
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }
        
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Foreign key for Category
        [Display(Name = "Category")]
        public int CategoryId { get; set; }
        
        // Navigation property for Category
        public virtual Category Category { get; set; } = null!;
    }
}
