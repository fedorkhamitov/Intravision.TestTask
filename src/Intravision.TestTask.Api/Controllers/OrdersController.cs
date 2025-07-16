using Intravision.TestTask.Application.DTOs.CommonDtos;
using Intravision.TestTask.Application.DTOs.Orders;
using Intravision.TestTask.Application.Interfaces.Services;
using Intravision.TestTask.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Intravision.TestTask.Api.Controllers;

/// <summary>
/// Контроллер для управления заказами торгового автомата.
/// </summary>
/// <remarks>
/// Предоставляет функциональность для создания заказов, получения информации о заказах
/// и проверки статуса торгового автомата. Включает проверку блокировки автомата
/// через сессии для обеспечения эксклюзивного доступа.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера OrdersController.
    /// </summary>
    /// <param name="orderService">Сервис для работы с заказами.</param>
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Создает новый заказ в системе торгового автомата.
    /// </summary>
    /// <param name="dto">Данные для создания заказа, включая список товаров и количество.</param>
    /// <returns>Результат создания заказа с информацией о необходимой оплате и сдаче.</returns>
    /// <response code="200">Заказ успешно создан.</response>
    /// <response code="400">Ошибка валидации данных заказа или сессия не удерживает автомат.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    /// <remarks>
    /// Метод проверяет, что текущая сессия удерживает блокировку автомата.
    /// После успешного создания заказа автомат автоматически освобождается.
    /// </remarks>
    /// <example>
    /// POST /api/orders
    /// {
    ///   "items": [
    ///     {
    ///       "productId": "12345678-1234-1234-1234-123456789012",
    ///       "quantity": 2
    ///     }
    ///   ],
    ///   "paymentAmount": 100.00
    /// }
    /// </example>
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto dto)
    {
        try
        {
            var orderResult = await _orderService.CreateOrderAsync(dto);
        
            return Ok(new ApiResponse<OrderResultDto>(
                true, orderResult, "Заказ успешно создан"));
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<OrderResultDto>(
                false, null, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<OrderResultDto>(
                false, null, "Внутренняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Получает список всех заказов с возможностью фильтрации по датам.
    /// </summary>
    /// <param name="startDate">Начальная дата для фильтрации заказов (включительно).</param>
    /// <param name="endDate">Конечная дата для фильтрации заказов (включительно).</param>
    /// <returns>Список заказов, соответствующих критериям фильтрации.</returns>
    /// <response code="200">Возвращает список заказов.</response>
    /// <response code="400">Ошибка при получении заказов.</response>
    /// <remarks>
    /// Если параметры дат не указаны, возвращаются все заказы.
    /// Даты должны быть в формате ISO 8601 (например, 2024-01-15T10:30:00).
    /// </remarks>
    /// <example>
    /// GET /api/orders?startDate=2024-01-01T00:00:00&amp;endDate=2024-01-31T23:59:59
    /// </example>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            var orders = await _orderService.GetOrdersAsync(startDate, endDate);
            
            return Ok(new ApiResponse<IEnumerable<OrderDto>>(
                true, orders, "Заказы получены"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<IEnumerable<OrderDto>>(
                false, null, ex.Message));
        }
    }

    /// <summary>
    /// Получает информацию о конкретном заказе по его идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор заказа в формате GUID.</param>
    /// <returns>Детальная информация о заказе.</returns>
    /// <response code="200">Возвращает информацию о найденном заказе.</response>
    /// <response code="404">Заказ с указанным идентификатором не найден.</response>
    /// <response code="400">Некорректный формат идентификатора или другая ошибка.</response>
    /// <example>
    /// GET /api/orders/12345678-1234-1234-1234-123456789012
    /// </example>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(Guid id)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null)
            {
                return NotFound(new ApiResponse<OrderDto>(
                    false, null, "Заказ не найден"));
            }

            return Ok(new ApiResponse<OrderDto>(
                true, order, "Заказ найден"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<OrderDto>(
                false, null, ex.Message));
        }
    }
}