using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BootS.Data;
using BootS.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace BootS.Controllers
{
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ApplicationDbContext context, ILogger<WishlistController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItems = await _context.Wishlist
                .Include(w => w.Product)
                .Where(w => w.UserId == userId)
                .ToListAsync();
            return View(wishlistItems);
        }

        [HttpPost]
        [Route("Wishlist/AddToWishlist")]
        public async Task<IActionResult> AddToWishlist([FromBody]int productId)
        {
            try
            {
                _logger.LogInformation($"Attempting to add product {productId} to wishlist");
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found");
                    return Json(new { success = false, message = "Пожалуйста, войдите в систему" });
                }

                var existingItem = await _context.Wishlist
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

                if (existingItem == null)
                {
                    var wishlistItem = new Wishlist
                    {
                        UserId = userId,
                        ProductId = productId,
                        DateAdded = DateTime.Now
                    };

                    _context.Wishlist.Add(wishlistItem);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Product {productId} added to wishlist for user {userId}");
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error adding product to wishlist: {ex.Message}");
                return Json(new { success = false, message = "Произошла ошибка при добавлении товара" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist([FromBody]int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlistItem = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlistItem != null)
            {
                _context.Wishlist.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> IsInWishlist(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isInWishlist = await _context.Wishlist
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
            return Json(new { isInWishlist });
        }
    }
} 