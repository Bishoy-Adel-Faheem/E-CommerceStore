using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcommerceApp.Models;
using Microsoft.AspNetCore.Http;

namespace EcommerceApp.ViewModels
{    public class ProductViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        
        [Display(Name = "Old Price")]
        public decimal? OldPrice { get; set; }
          [Display(Name = "Image URL")]
        public string ImageUrl { get; set; } = string.Empty;
        
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
        
        [Display(Name = "Is Featured")]
        public bool IsFeatured { get; set; }
        
        [Display(Name = "Category")]
        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }    public class CategoryViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
    }
}
