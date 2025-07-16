using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intravision.TestTask.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly ApplicationDbContext _context;

    public BrandRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Brand>> GetAllAsync()
    {
        return await _context.Brands.ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(Guid id)
    {
        return await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task AddAsync(Brand brand)
    {
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var brand = await GetByIdAsync(id);
        if (brand != null)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }
    }
}