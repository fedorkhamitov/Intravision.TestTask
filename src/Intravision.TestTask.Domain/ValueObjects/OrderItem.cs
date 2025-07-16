using Intravision.TestTask.Domain.Shared;

namespace Intravision.TestTask.Domain.ValueObjects;

public class OrderItem : ValueObject
{
    public Guid ProductId { get; private set; }
    public Guid BrandId { get; private set; }
    public int Quantity { get; private set; }
    public Money TotalPrice { get; private set; }
    
    private OrderItem() { } // Для EF Core
    
    public OrderItem(Guid productId, Guid brandId, int quantity, Money unitPrice)
    {
        ProductId = productId;
        BrandId = brandId;
        Quantity = quantity;
        TotalPrice = new Money(unitPrice.Amount * quantity, unitPrice.Currency);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ProductId;
        yield return BrandId;
        yield return Quantity;
        yield return TotalPrice;
    }
}