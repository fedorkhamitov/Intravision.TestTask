namespace Intravision.TestTask.Application.DTOs;

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    Guid BrandId,
    int StockQuantity);