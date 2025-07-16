using Intravision.TestTask.Domain.Shared;
using Intravision.TestTask.Domain.ValueObjects;

namespace Intravision.TestTask.Domain.Entities;

public class Coin : Entity
{
    public Money Denomination { get; private set; }
    public int Quantity { get; private set; }
    
    private Coin() { } // Для EF Core
    
    public Coin(Money denomination, int quantity)
    {
        Id = Guid.NewGuid();
        Denomination = denomination ?? throw new ArgumentNullException(nameof(denomination));
        Quantity = quantity;
    }
    
    public void AddCoins(int count)
    {
        if (count < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        Quantity += count;
    }
    
    public void RemoveCoins(int count)
    {
        if (count < 0)
            throw new ArgumentException("Количество не может быть отрицательным");
        
        if (Quantity < count)
            throw new InvalidOperationException("Недостаточно монет");
        
        Quantity -= count;
    }
}