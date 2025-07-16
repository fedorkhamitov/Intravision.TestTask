namespace Intravision.TestTask.Application.DTOs.Products;

public record ImportProductDto(
    string Name,
    string Description,
    decimal Price,
    string BrandName,
    int StockQuantity);