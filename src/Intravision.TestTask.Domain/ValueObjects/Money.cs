using Intravision.TestTask.Domain.Shared;

namespace Intravision.TestTask.Domain.ValueObjects;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    private Money() { } // Для EF Core
    
    public Money(decimal amount, string currency = "RUB")
    {
        if (amount < 0)
            throw new ArgumentException("Сумма не может быть отрицательной");
        
        Amount = amount;
        Currency = currency ?? "RUB";
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}