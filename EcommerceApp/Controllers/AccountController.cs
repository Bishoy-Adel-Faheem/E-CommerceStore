using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using EcommerceApp.Models;
using EcommerceApp.ViewModels;
using EcommerceApp.Services;

namespace EcommerceApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICartService _cartService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ICartService cartService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cartService = cartService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["ReturnUrl"] = model.ReturnUrl;
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    
                if (result.Succeeded)
                {                    // Migrate cart items from anonymous session to user account
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var anonymousCartId = Microsoft.AspNetCore.Http.SessionExtensions.GetString(HttpContext.Session, "CartId");
                        if (!string.IsNullOrEmpty(anonymousCartId))
                        {
                            await _cartService.MigrateCartAsync(anonymousCartId, user.Id);
                        }
                    }
                    
                    if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return LocalRedirect(model.ReturnUrl);
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            
            return View(model);
        }        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new RegisterViewModel());
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["ReturnUrl"] = string.Empty;
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    City = model.City,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber
                };
                
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);                    // Migrate cart items from anonymous session to user account
                    var anonymousCartId = Microsoft.AspNetCore.Http.SessionExtensions.GetString(HttpContext.Session, "CartId");
                    if (!string.IsNullOrEmpty(anonymousCartId))
                    {
                        await _cartService.MigrateCartAsync(anonymousCartId, user.Id);
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
