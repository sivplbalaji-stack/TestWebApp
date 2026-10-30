using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestWebApp.DataAccess.Data;
using TestWebApp.Models;
using TestWebApp.Utility;

namespace TestWebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _context.CartItems.Where(c => c.UserId == userId).Count());
            }
            var products = _context.Products.ToList();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            return View(product);
        }

        [Authorize, HttpPost]
        public async Task<IActionResult> Details(CartItem cartItem)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var cartProduct = await _context.CartItems.FirstOrDefaultAsync(e => e.ProductId == cartItem.ProductId && e.UserId == userId);
            if (cartProduct != null)
            {
                cartProduct.Quantity += cartItem.Quantity;
                _context.CartItems.Update(cartProduct);
            }
            else
            {
                cartItem.Id = 0;
                cartItem.UserId = userId!;
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Product added to cart successfully!";
            //var product = await _context.Products.FindAsync(cartItem.ProductId);
            //if (product == null)
            //{
            //    return NotFound();
            //}
            // Logic to add the product to the user's cart
            // This could involve creating a Cart entity and saving it to the database
            return RedirectToAction("Index", "Product");
        }
    }
}
