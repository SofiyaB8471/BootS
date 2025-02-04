using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BootS.Data;
using BootS.Models;

namespace BootS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Проверяем и создаем таблицу Wishlist если её нет
            if (!context.Wishlist.Any())
            {
                // Таблица создастся автоматически
            }

            // Проверяем, есть ли уже товары
            if (context.Products.Any())
            {
                return; // База данных уже заполнена
            }

            var products = new Product[]
            {
                new Product
                {
                    Name = "Nike Air Max 270",
                    Description = "Стильные кроссовки с амортизирующей подошвой",
                    Price = 12999,
                    ImageUrl = "/images/products/nike-air-max-270.jpg",
                    Category = "Nike",
                    InStock = true
                },
                new Product
                {
                    Name = "Adidas Ultraboost",
                    Description = "Беговые кроссовки с технологией Boost",
                    Price = 14999,
                    ImageUrl = "/images/products/adidas-ultraboost.jpg",
                    Category = "Adidas",
                    InStock = true
                },
                new Product
                {
                    Name = "Puma RS-X",
                    Description = "Кроссовки в стиле ретро",
                    Price = 8999,
                    ImageUrl = "/images/products/puma-rsx.jpg",
                    Category = "Puma",
                    InStock = true
                },
                new Product
                {
                    Name = "Nike Air Force 1",
                    Description = "Классические белые кроссовки",
                    Price = 9999,
                    ImageUrl = "/images/products/nike-air-force-1.jpg",
                    Category = "Nike",
                    InStock = true
                },
                new Product
                {
                    Name = "Adidas Stan Smith",
                    Description = "Культовые кроссовки для повседневной носки",
                    Price = 7999,
                    ImageUrl = "/images/products/adidas-stan-smith.jpg",
                    Category = "Adidas",
                    InStock = true
                },
                new Product
                {
                    Name = "New Balance 574",
                    Description = "Комфортные кроссовки для города",
                    Price = 8499,
                    ImageUrl = "/images/products/new-balance-574.jpg",
                    Category = "New Balance",
                    InStock = true
                }
            };

            context.Products.AddRange(products);

            // Добавляем пользователей если их нет
            if (!context.Users.Any())
            {
                var users = new User[]
                {
                    new User
                    {
                        Username = "admin",
                        Password = "admin123",
                        Email = "admin@boots.com",
                        Role = "Admin",
                        Balance = 0
                    },
                    new User
                    {
                        Username = "user",
                        Password = "user123",
                        Email = "user@boots.com",
                        Role = "User",
                        Balance = 0
                    }
                };

                context.Users.AddRange(users);
            }

            context.SaveChanges();
        }

        public static void ResetAdminBalance(ApplicationDbContext context)
        {
            var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
            if (admin != null)
            {
                admin.Balance = 0;
                context.SaveChanges();
            }
        }

        public static void UpdateProducts(ApplicationDbContext context)
        {
            // Очищаем существующие продукты
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
            
            // Вызываем Initialize для добавления всех продуктов заново
            Initialize(context);
        }
    }
}