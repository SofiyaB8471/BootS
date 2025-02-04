using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BootS.Data;
using BootS.Models;
using System.Security.Claims;

namespace BootS.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == $"user_{userId}")
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            ViewBag.TotalAmount = totalAmount;
            ViewBag.UserBalance = user?.Balance ?? 0;

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.DeliveryAddress))
                {
                    return BadRequest(new { message = "Адрес доставки обязателен" });
                }

                if (string.IsNullOrWhiteSpace(model.PhoneNumber))
                {
                    return BadRequest(new { message = "Номер телефона обязателен" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized();

                _logger.LogInformation($"Creating order for user {userId}");

                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null) return NotFound(new { message = "Пользователь не найден" });

                var cartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.CartId == $"user_{userId}")
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return BadRequest(new { message = "Корзина пуста" });
                }

                foreach (var item in cartItems)
                {
                    if (item.Product == null)
                    {
                        return BadRequest(new { message = $"Товар с ID {item.ProductId} не найден" });
                    }
                }

                var totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

                if (user.Balance < totalAmount)
                {
                    return BadRequest(new { message = "Недостаточно средств на балансе" });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var order = new Order
                    {
                        UserId = user.Id,
                        OrderDate = DateTime.Now,
                        TotalAmount = totalAmount,
                        DeliveryAddress = model.DeliveryAddress,
                        PhoneNumber = model.PhoneNumber,
                        Status = "Новый"
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    var orderItems = cartItems.Select(item => new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    }).ToList();

                    _context.OrderItems.AddRange(orderItems);

                    user.Balance -= totalAmount;
                    _context.CartItems.RemoveRange(cartItems);
                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Order {order.Id} created successfully");
                    return Json(new { 
                        success = true, 
                        message = "Заказ успешно оформлен!",
                        orderId = order.Id
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error creating order: {Message}", ex.Message);
                    _logger.LogError(ex.StackTrace);
                    return StatusCode(500, new { message = $"Ошибка при создании заказа: {ex.Message}" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in PlaceOrder: {Message}", ex.Message);
                _logger.LogError(ex.StackTrace);
                return StatusCode(500, new { message = $"Неожиданная ошибка при создании заказа: {ex.Message}" });
            }
        }

        [Authorize]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == int.Parse(userId))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }

    public class PlaceOrderModel
    {
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
} 