namespace Intravision.TestTask.Application.DTOs.Orders;

public record OrderItemRequestDto(Guid ProductId, int Quantity);