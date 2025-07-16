using Intravision.TestTask.Domain.Entities;
using Intravision.TestTask.Domain.ValueObjects;

namespace Intravision.TestTask.Infrastructure;

public static class DataSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        
        // Инициализация брендов
        if (!context.Brands.Any())
        {
            var brands = new[]
            {
                new Brand("Coca-Cola", "Газированный напиток с карамельным вкусом и вековой историей"),
                new Brand("Sprite", "Освежающая безалкогольная лимонадная классика"),
                new Brand("Pepsi", "Популярный бренд колы"),
                new Brand("Fanta", "Апельсиновый газированные напитки"),
                new Brand("Dr. Pepper", "Газированный напиток с пряными и фруктовыми нотами"),
            };

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync();
        }

        // Инициализация напитков
        if (!context.Products.Any())
        {
            var brands = context.Brands.ToList();
            var cocaCola = brands.First(b => b.Name == "Coca-Cola");
            var pepsi = brands.First(b => b.Name == "Pepsi");
            var fanta = brands.First(b => b.Name == "Fanta");
            var sprite = brands.First(b => b.Name == "Sprite");
            var drpepper = brands.First(b => b.Name == "Dr. Pepper");
            
            var products = new[]
            {
                new Product("Coca-Cola", "Классическая кола", new Money(85), cocaCola.Id, 10),
                new Product("Coca-Cola Zero", "Кола без сахара", new Money(85), cocaCola.Id, 8),
                new Product("Pepsi Cola", "Пепси кола", new Money(70), pepsi.Id, 12),
                new Product("Pepsi Max", "Пепси без сахара", new Money(70), pepsi.Id, 7),
                new Product("Fanta Orange", "Апельсиновая фанта", new Money(75), fanta.Id, 15),
                new Product("Fanta Grape", "Виноградная фанта", new Money(75), fanta.Id, 9),
                new Product("Sprite", "Лимонно-лаймовая газировка", new Money(70), sprite.Id, 11),
                new Product("Sprite Zero", "Спрайт без сахара", new Money(70), sprite.Id, 6),
                new Product("Dr. Pepper", "Доктор Пеппер", new Money(100), drpepper.Id, 5)
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Инициализация монет
        if (!context.Coins.Any())
        {
            var coins = new[]
            {
                new Coin(new Money(1), 80),
                new Coin(new Money(2), 30),
                new Coin(new Money(5), 10),
                new Coin(new Money(10), 10)
            };

            context.Coins.AddRange(coins);
            await context.SaveChangesAsync();
        }
    }
}