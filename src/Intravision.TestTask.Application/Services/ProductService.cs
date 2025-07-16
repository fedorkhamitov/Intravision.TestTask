using Intravision.TestTask.Application.DTOs.CommonDtos;
using Intravision.TestTask.Application.DTOs.Products;
using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Application.Interfaces.Services;
using Intravision.TestTask.Domain.Entities;
using Intravision.TestTask.Domain.Exceptions;
using Intravision.TestTask.Domain.ValueObjects;

namespace Intravision.TestTask.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;
    
    public ProductService(IProductRepository productRepository, IBrandRepository brandRepository)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
    }
    
    public async Task<IEnumerable<ProductDto>> GetProductsAsync(ProductFilterDto filter)
    {
        var products = await _productRepository.GetFilteredAsync(
            filter.BrandId, 
            filter.MinPrice, 
            filter.MaxPrice
        );

        var brands = await _brandRepository.GetAllAsync();
        var brandDict = brands.ToDictionary(b => b.Id, b => b.Name);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price.Amount,
            p.Price.Currency,
            p.StockQuantity,
            p.BrandId,
            brandDict.GetValueOrDefault(p.BrandId, "Unknown")
        ));
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null) return null;

        var brand = await _brandRepository.GetByIdAsync(product.BrandId);
        
        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.BrandId,
            brand?.Name ?? "Unknown"
        );
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto)
    {
        var brand = await _brandRepository.GetByIdAsync(dto.BrandId);
        if (brand == null)
            throw new DomainException("Бренд не найден");

        var product = new Product(
            dto.Name,
            dto.Description,
            new Money(dto.Price),
            dto.BrandId,
            dto.StockQuantity
        );

        await _productRepository.AddAsync(product);

        return new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.BrandId,
            brand.Name
        );
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new DomainException("Товар не найден");

        product.UpdatePrice(new Money(dto.Price));
        product.UpdateStock(dto.StockQuantity);

        await _productRepository.UpdateAsync(product);

        var brand = await _brandRepository.GetByIdAsync(product.BrandId);
        
        return new ProductDto(
            product.Id,
            dto.Name,
            dto.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.BrandId,
            brand?.Name ?? "Unknown"
        );
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
    }

    public async Task<PriceRangeDto> GetPriceRangeAsync(Guid? brandId = null)
    {
        var products = brandId.HasValue 
            ? await _productRepository.GetByBrandAsync(brandId.Value)
            : await _productRepository.GetAllAsync();

        var availableProducts = products.Where(p => p.IsInStock).ToList();
        
        if (availableProducts.Count == 0)
            return new PriceRangeDto(0, 0);

        var minPrice = availableProducts.Min(p => p.Price.Amount);
        var maxPrice = availableProducts.Max(p => p.Price.Amount);

        return new PriceRangeDto(minPrice, maxPrice);
    }
}