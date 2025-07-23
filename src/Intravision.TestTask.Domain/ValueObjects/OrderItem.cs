using Intravision.TestTask.Domain.Shared;

namespace Intravision.TestTask.Domain.ValueObjects;

public class OrderItem : ValueObject
{
    public Guid ProductId { get; private set; }
    
    public string ProductName { get; private set; }
    public Guid BrandId { get; private set; }
    
    public string BrandName { get; private set; }
    public int Quantity { get; private set; }
    
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }
    
    private OrderItem() { } // Для EF Core
    
    public OrderItem(
        Guid productId, 
        string productName, 
        Guid brandId, 
        string brandName, 
        int quantity, 
        Money unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        BrandId = brandId;
        BrandName = brandName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = new Money(unitPrice.Amount * quantity, unitPrice.Currency);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return ProductId;   
        yield return ProductName;
        yield return BrandId;     
        yield return BrandName;
        yield return Quantity;    
        yield return UnitPrice;   
        yield return TotalPrice;
    }
}