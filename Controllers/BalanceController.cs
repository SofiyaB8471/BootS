using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BootS.Data;
using Microsoft.EntityFrameworkCore;
using BootS.Models;

namespace BootS.Controllers
{
    [Authorize]
    public class BalanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BalanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBalance([FromBody] AddBalanceModel model)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Некорректные данные" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "Пользователь не найден" });
                }

                user.Balance += model.Amount;
                await _context.SaveChangesAsync();

                return Json(new { success = true, newBalance = user.Balance });
            }
            catch (Exception)
            {
                return StatusCode(500, new { success = false, message = "Произошла ошибка при обновлении баланса" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { balance = 0 });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            return Json(new { balance = user?.Balance ?? 0 });
        }
    }
} 