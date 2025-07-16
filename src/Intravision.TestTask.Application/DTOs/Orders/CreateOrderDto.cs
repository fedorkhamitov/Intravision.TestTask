namespace Intravision.TestTask.Application.DTOs.Orders;

public record CreateOrderDto(IReadOnlyList<OrderItemRequestDto> Items, IReadOnlyDictionary<decimal, int> InsertedCoins);