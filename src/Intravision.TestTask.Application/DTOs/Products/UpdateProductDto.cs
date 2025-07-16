namespace Intravision.TestTask.Application.DTOs.Products;

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int StockQuantity);