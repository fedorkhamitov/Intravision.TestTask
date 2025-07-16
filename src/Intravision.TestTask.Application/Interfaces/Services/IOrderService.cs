using Intravision.TestTask.Application.DTOs;
using Intravision.TestTask.Application.DTOs.Orders;

namespace Intravision.TestTask.Application.Interfaces.Services;

public interface IOrderService
{
    Task<OrderResultDto> CreateOrderAsync(CreateOrderDto dto);
    Task<IEnumerable<OrderDto>> GetOrdersAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<OrderDto?> GetOrderByIdAsync(Guid id);
}