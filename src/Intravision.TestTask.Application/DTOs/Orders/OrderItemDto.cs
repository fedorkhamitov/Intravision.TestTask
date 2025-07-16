namespace Intravision.TestTask.Application.DTOs.Orders;

public record OrderItemDto(
    Guid ProductId,
    string ProductName,
    Guid BrandId,
    string BrandName,
    int Quantity,
    decimal TotalPrice);