using Intravision.TestTask.Domain.Entities;

namespace Intravision.TestTask.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId);
    Task<IEnumerable<Product>> GetFilteredAsync(Guid? brandId, decimal? minPrice, decimal? maxPrice);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}