using Intravision.TestTask.Application.DTOs.Orders;
using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Application.Interfaces.Services;
using Intravision.TestTask.Domain.Entities;
using Intravision.TestTask.Domain.Exceptions;

namespace Intravision.TestTask.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICoinRepository _coinRepository;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICoinRepository coinRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _coinRepository = coinRepository;
    }
    
    public async Task<OrderResultDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // Валидация товаров
        var products = new List<Product>();
        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new DomainException($"Товар с ID {item.ProductId} не найден");

            if (!product.HasSufficientStock(item.Quantity))
                throw new DomainException($"Недостаточно товара {product.Name} на складе");

            products.Add(product);
        }

        // Создание заказа
        var order = new Order(DateTime.UtcNow);
        
        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            order.AddItem(product.Id, product.BrandId, item.Quantity, product.Price);
        }

        // Валидация оплаты
        var insertedAmount = dto.InsertedCoins.Sum(c => c.Key * c.Value);
        if (insertedAmount < order.TotalAmount.Amount)
            throw new DomainException("Недостаточно средств для оплаты");

        // Расчет сдачи
        var changeAmount = insertedAmount - order.TotalAmount.Amount;
        var changeCoins = new Dictionary<decimal, int>();
        
        if (changeAmount > 0)
        {
            if (!await _coinRepository.IsCanMakeChangeAsync(changeAmount))
                throw new DomainException("Автомат не может выдать сдачу");

            changeCoins = await _coinRepository.CalculateChangeAsync(changeAmount);
        }

        // Обновление количества товаров
        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.UpdateStock(product.StockQuantity - item.Quantity);
            await _productRepository.UpdateAsync(product);
        }

        // Обновление монет
        await _coinRepository.ProcessPaymentAsync(dto.InsertedCoins.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), changeCoins);

        // Сохранение заказа
        await _orderRepository.AddAsync(order);

        // Возврат результата
        var orderDto = await ConvertToOrderDto(order);
        
        return new OrderResultDto(
            orderDto,
            changeCoins,
            "Спасибо за покупку! Заберите товар и сдачу"
        );
    }

    public async Task<IEnumerable<OrderDto>> GetOrdersAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var orders = startDate.HasValue && endDate.HasValue
            ? await _orderRepository.GetOrdersByDateRangeAsync(startDate.Value, endDate.Value)
            : await _orderRepository.GetAllAsync();

        var result = new List<OrderDto>();
        foreach (var order in orders)
        {
            result.Add(await ConvertToOrderDto(order));
        }

        return result;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        return order == null ? null : await ConvertToOrderDto(order);
    }
    
    private async Task<OrderDto> ConvertToOrderDto(Order order)
    {
        var products = await _productRepository.GetAllAsync();
        var brands = await _brandRepository.GetAllAsync();
        
        var productDict = products.ToDictionary(p => p.Id, p => p.Name);
        var brandDict = brands.ToDictionary(b => b.Id, b => b.Name);

        var items = order.Items.Select(item => new OrderItemDto(
            item.ProductId,
            productDict.GetValueOrDefault(item.ProductId, "Unknown"),
            item.BrandId,
            brandDict.GetValueOrDefault(item.BrandId, "Unknown"),
            item.Quantity,
            item.TotalPrice.Amount
        )).ToList();

        return new OrderDto(
            order.Id,
            order.OrderDate,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            items
        );
    }
}