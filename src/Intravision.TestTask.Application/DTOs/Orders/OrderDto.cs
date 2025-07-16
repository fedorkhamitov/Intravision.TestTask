namespace Intravision.TestTask.Application.DTOs;

public record OrderDto(
    Guid Id,
    DateTime OrderDate,
    decimal TotalAmount,
    string Currency,
    IReadOnlyList<OrderItemDto> Items);