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
    private readonly ChangeCalculator _changeCalculator;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IBrandRepository brandRepository,
        ICoinRepository coinRepository,
        ChangeCalculator changeCalculator)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _coinRepository = coinRepository;
        _changeCalculator = changeCalculator;
    }
    
    public async Task<OrderResultDto> CreateOrderAsync(CreateOrderDto dto)
    {
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

        var order = new Order(DateTime.UtcNow);
        
        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var brand = await _brandRepository.GetByIdAsync(product.BrandId);
    
            order.AddItem(
                product.Id,
                product.Name,
                product.BrandId,
                brand?.Name ?? "Unknown Brand",
                item.Quantity,
                product.Price);
        }

        var insertedAmount = dto.InsertedCoins.Sum(c => c.Key * c.Value);
        if (insertedAmount < order.TotalAmount.Amount)
            throw new DomainException("Недостаточно средств для оплаты");

        var changeAmount = insertedAmount - order.TotalAmount.Amount;
        var changeCoins = new Dictionary<decimal, int>();
        
        if (changeAmount > 0)
        {
            var coins = await _coinRepository.GetAllAsync();

            if (!_changeCalculator.IsCanMakeChange(coins, changeAmount))
                throw new DomainException("Автомат не может выдать сдачу");

            changeCoins = _changeCalculator.CalculateChange(coins, changeAmount);

        }

        foreach (var item in dto.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.UpdateStock(product.StockQuantity - item.Quantity);
            await _productRepository.UpdateAsync(product);
        }

        await _coinRepository.ProcessPaymentAsync(dto.InsertedCoins.ToDictionary(kvp => kvp.Key, kvp => kvp.Value), changeCoins);

        await _orderRepository.AddAsync(order);

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
    
    private Task<OrderDto> ConvertToOrderDto(Order order)
    {
        var items = order.Items.Select(item => new OrderItemDto(
            item.ProductId,
            item.ProductName,
            item.BrandId,
            item.BrandName,
            item.Quantity,
            item.TotalPrice.Amount
        )).ToList();

        var orderDto = new OrderDto(
            order.Id,
            order.OrderDate,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency,
            items
        );

        return Task.FromResult(orderDto);
    }


}