using Intravision.TestTask.Domain.Shared;
using Intravision.TestTask.Domain.ValueObjects;

namespace Intravision.TestTask.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Money Price { get; private set; }
    public int StockQuantity { get; private set; }
    public Guid BrandId { get; private set; }
    
    public bool IsInStock => StockQuantity > 0;
    
    private Product() { } // Для EF Core
    
    public Product(string name, string description, Money price, Guid brandId, int stockQuantity)
    {
        Id = Guid.NewGuid();
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Price = price ?? throw new ArgumentNullException(nameof(price));
        BrandId = brandId;
        StockQuantity = stockQuantity;
    }
    
    public void UpdatePrice(Money newPrice)
    {
        Price = newPrice ?? throw new ArgumentNullException(nameof(newPrice));
    }
    
    public void UpdateStock(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        StockQuantity = quantity;
    }
    
    public bool HasSufficientStock(int requestedQuantity) => StockQuantity >= requestedQuantity;

    public void AddStockQuantity(int quantity)
    {
        if (quantity < 0)
            throw new ArgumentException("Количество добавленных не может быть отрицательным");
        
        StockQuantity += quantity;
    }
}