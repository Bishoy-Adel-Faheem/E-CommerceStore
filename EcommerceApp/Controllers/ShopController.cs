using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Services;
using EcommerceApp.ViewModels;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace EcommerceApp.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ShopController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, string searchTerm, decimal? minPrice, decimal? maxPrice, 
            bool? inStock, string sortBy, int page = 1, int pageSize = 12)
        {
            // Filter products based on the provided criteria
            var products = await _productService.FilterProductsAsync(
                categoryId,
                minPrice,
                maxPrice,
                inStock,
                sortBy);

            // Apply search term if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => 
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Get all categories for the filter sidebar
            var categories = await _categoryService.GetAllCategoriesAsync();

            // Calculate pagination values
            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            // Apply pagination
            products = products.Skip((page - 1) * pageSize).Take(pageSize);

            // Determine the sort option
            var sortOption = SortOption.Newest;
            if (!string.IsNullOrEmpty(sortBy))
            {
                sortOption = sortBy.ToLower() switch
                {
                    "price_asc" => SortOption.PriceLowToHigh,
                    "price_desc" => SortOption.PriceHighToLow,
                    "name_asc" => SortOption.NameAZ,
                    "name_desc" => SortOption.NameZA,
                    "newest" => SortOption.Newest,
                    _ => SortOption.Newest
                };
            }

            // Create and populate view model
            var viewModel = new ShopViewModel
            {
                Products = products.ToList(),
                Categories = categories,
                CurrentCategory = categoryId.HasValue ? categories.FirstOrDefault(c => c.Id == categoryId.Value)?.Name : null,
                SearchTerm = searchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOption = sortOption,
                PageNumber = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Get related products from the same category
            var relatedProducts = await _productService.GetProductsByCategoryAsync(product.CategoryId);
            
            // Exclude the current product and take a few related products
            relatedProducts = relatedProducts
                .Where(p => p.Id != id)
                .Take(4)
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;
            
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index), new { searchTerm = term });
        }
    }
}
