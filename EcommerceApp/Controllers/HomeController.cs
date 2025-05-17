using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Models;
using EcommerceApp.Services;

namespace EcommerceApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public HomeController(
        ILogger<HomeController> logger,
        IProductService productService,
        ICategoryService categoryService)
    {
        _logger = logger;
        _productService = productService;
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index()
    {
        var featuredProducts = await _productService.FilterProductsAsync(
            categoryId: null,
            minPrice: null,
            maxPrice: null,
            inStock: true,
            sortBy: "newest");

        // Take only a few featured products for the home page
        return View(featuredProducts.Take(8).ToList());
    }    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult AboutUs()
    {
        return View();
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult FAQ()
    {
        return View();
    }

    public IActionResult Terms()
    {
        return View();
    }

    public IActionResult ShippingInfo()
    {
        return View("ShippingInfo");
    }

    public IActionResult Returns()
    {
        return View("Returns");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
