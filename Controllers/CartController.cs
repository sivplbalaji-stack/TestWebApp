using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestWebApp.DataAccess.Data;
using TestWebApp.Utility;

namespace TestWebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _context.CartItems.Where(c => c.UserId == userId)
                            .Include(p => p.Product).ToList();

            return View(cartItems);
        }

        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartFromDb == null) return NotFound();

            cartFromDb.Quantity += 1;
            _context.CartItems.Update(cartFromDb);
            await _context.SaveChangesAsync();
            TempData["Success"] = "CartItem Increamented successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cartFromDb = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartFromDb == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (cartFromDb.Quantity <= 1)
            {
                _context.CartItems.Remove(cartFromDb);

                var count = _context.CartItems.Count(c => c.UserId == userId) - 1;
                HttpContext.Session.SetInt32(TestWebApp.Utility.SD.SessionCart, count);
            }
            else
            {
                cartFromDb.Quantity -= 1;
                _context.CartItems.Update(cartFromDb);
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "CartItem Decremented successfully!";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartId);
            if (cartFromDb == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _context.CartItems.Remove(cartFromDb);

            var count = _context.CartItems.Count(c => c.UserId == userId) - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, count);
            
            await _context.SaveChangesAsync();
            TempData["Success"] = "CartItem Removed successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
