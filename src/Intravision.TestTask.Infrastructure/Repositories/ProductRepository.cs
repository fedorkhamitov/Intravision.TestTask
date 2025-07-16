using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intravision.TestTask.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(Guid brandId)
    {
        return await _context.Products
            .Where(p => p.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFilteredAsync(Guid? brandId, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Products.AsQueryable();

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId.Value);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price.Amount >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price.Amount <= maxPrice.Value);

        return await query.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.Name.ToLower() == product.Name.ToLower());

        if (existingProduct != null)
        {
            existingProduct.AddStockQuantity(product.StockQuantity);
            _context.Products.Update(existingProduct);
        }
        else
        {
            await _context.Products.AddAsync(product);
        }
        await _context.SaveChangesAsync();
    }


    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await GetByIdAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}