namespace Intravision.TestTask.Application.DTOs.Orders;

public record OrderResultDto(
    OrderDto Order,
    IReadOnlyDictionary<decimal, int> Change,
    string Message);