namespace Intravision.TestTask.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    int StockQuantity,
    Guid BrandId,
    string BrandName)
{
    public bool IsInStock => StockQuantity > 0;
}