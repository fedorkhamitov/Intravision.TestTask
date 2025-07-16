using Intravision.TestTask.Domain.Shared;
using Intravision.TestTask.Domain.ValueObjects;

namespace Intravision.TestTask.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderItem> _items;
    
    public DateTime OrderDate { get; private set; }
    public Money TotalAmount { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    
    private Order() { } // Для EF Core
    
    public Order(DateTime orderDate)
    {
        Id = Guid.NewGuid();
        OrderDate = orderDate;
        TotalAmount = new Money(0);
        _items = [];
    }
    
    public void AddItem(Guid productId, Guid brandId, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Количество должно быть положительным");
        
        var orderItem = new OrderItem(productId, brandId, quantity, unitPrice);
        _items.Add(orderItem);
        
        RecalculateTotal();
    }
    
    private void RecalculateTotal()
    {
        var total = _items.Sum(item => item.TotalPrice.Amount);
        TotalAmount = new Money(total);
    }
}