using Intravision.TestTask.Application.DTOs;
using Intravision.TestTask.Application.DTOs.CommonDtos;
using Intravision.TestTask.Application.DTOs.Products;

namespace Intravision.TestTask.Application.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetProductsAsync(ProductFilterDto filter);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto dto);
    Task DeleteProductAsync(Guid id);
    Task<PriceRangeDto> GetPriceRangeAsync(Guid? brandId = null);
}