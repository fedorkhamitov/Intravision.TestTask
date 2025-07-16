namespace Intravision.TestTask.Application.DTOs;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    Guid BrandId,
    string BrandName,
    int Quantity,
    decimal TotalPrice);