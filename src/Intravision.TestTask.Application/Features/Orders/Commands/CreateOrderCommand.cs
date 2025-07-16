namespace Intravision.TestTask.Application.Features.Orders.Commands;

public record CreateOrderCommand(
    IReadOnlyList<OrderItemRequest> Items,
    IReadOnlyDictionary<decimal, int> InsertedCoins);