namespace Intravision.TestTask.Application.DTOs.Products;

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    Guid BrandId,
    int StockQuantity);