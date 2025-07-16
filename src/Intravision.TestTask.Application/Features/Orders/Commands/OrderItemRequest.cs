namespace Intravision.TestTask.Application.Features.Orders.Commands;

public record OrderItemRequest(Guid ProductId, int Quantity);