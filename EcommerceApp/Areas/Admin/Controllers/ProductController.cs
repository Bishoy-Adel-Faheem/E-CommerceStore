using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using EcommerceApp.Services;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EcommerceApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(
            IProductService productService, 
            ICategoryService categoryService, 
            IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductViewModel
            {
                Categories = await _categoryService.GetAllCategoriesAsync()
            };
            return View(viewModel);
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Handle file upload if provided
                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }
                    
                    // Set the ImageUrl to the path of the uploaded file
                    viewModel.ImageUrl = "/images/" + uniqueFileName;
                }

                var product = new Product
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.Price,
                    OldPrice = viewModel.OldPrice,
                    ImageUrl = viewModel.ImageUrl,
                    Stock = viewModel.Stock,
                    IsFeatured = viewModel.IsFeatured,
                    CategoryId = viewModel.CategoryId
                };

                await _productService.CreateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed; redisplay form
            viewModel.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                OldPrice = product.OldPrice,
                ImageUrl = product.ImageUrl,
                Stock = product.Stock,
                IsFeatured = product.IsFeatured,
                CategoryId = product.CategoryId,
                Categories = await _categoryService.GetAllCategoriesAsync()
            };

            return View(viewModel);
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                // Handle file upload if provided
                if (viewModel.ImageFile != null && viewModel.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + viewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await viewModel.ImageFile.CopyToAsync(fileStream);
                    }
                    
                    // Set the ImageUrl to the path of the uploaded file
                    viewModel.ImageUrl = "/images/" + uniqueFileName;
                }

                product.Name = viewModel.Name;
                product.Description = viewModel.Description;
                product.Price = viewModel.Price;
                product.OldPrice = viewModel.OldPrice;
                product.ImageUrl = viewModel.ImageUrl;
                product.Stock = viewModel.Stock;
                product.IsFeatured = viewModel.IsFeatured;
                product.CategoryId = viewModel.CategoryId;

                await _productService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }

            viewModel.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
