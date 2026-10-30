using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Data;
using TestWebApp.Models;

namespace TestWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var items = _context.Products.ToList();
            return View(items);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (product.Name.Trim().ToLower().Equals("test"))
            {
                ModelState.AddModelError("Name", "The product name 'Test' is not allowed.");
            }

            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                //Get the web root path from environment to save the uploaded image
                string webRootPath = _webHostEnvironment.WebRootPath;

                //Clean and generate a unique file name for the uploaded image
                string originalFileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName)
                                          .Replace(" ", "_");

                string extension = Path.GetExtension(product.ImageFile.FileName);
                string uniqueFileName = $"{originalFileName}_{Guid.NewGuid():N}{extension}";

                //ensure that the images directory exists
                string imagesDirectory = Path.Combine(webRootPath, "images");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                //path to save the image physically on the server
                var filePath = Path.Combine(imagesDirectory, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }

                product.ImagePath = "/images/" + uniqueFileName;

                //verify that the image was saved successfully
                string confirmationFilePath = Path.Combine(webRootPath, product.ImagePath.TrimStart('/'));
                if (!System.IO.File.Exists(confirmationFilePath))
                {
                    ModelState.AddModelError("ImageFile", "Failed to save the image. Please try again.");
                    return View(product);
                }
            }
            else
            {
                ModelState.AddModelError("ImageFile", "An image file is required.");
            }

            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }

        [HttpGet("Edit/{Id}")]
        public async Task<IActionResult> Edit(int Id)
        {
            if (Id == 0) return View();

            var item = await _context.Products.FindAsync(Id);
            return View(item);
        }

        [HttpPost("Edit/{Id}")]
        public async Task<IActionResult> Edit(Product product)
        {
            if (product.Id == 0) return View();

            var item = await _context.Products.FindAsync(product.Id);
            if (item != null)
            {
                item.Name = product.Name;
                item.Description = product.Description;
                item.Price = product.Price;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return View(item);
            }
        }

        public IActionResult Delete(int Id)
        {
            if (Id == 0) return View();

            var item = _context.Products.FindAsync(Id);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            if (product.Id == 0) return View();

            var item = await _context.Products.FindAsync(product.Id);
            if (item != null)
            {
                _context.Products.Remove(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(item);
        }
    }
}
