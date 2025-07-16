using Intravision.TestTask.Application.DTOs.Brands;
using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Application.Interfaces.Services;
using Intravision.TestTask.Domain.Entities;
using Intravision.TestTask.Domain.Exceptions;

namespace Intravision.TestTask.Application.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }
    
    public async Task<IEnumerable<BrandDto>> GetBrandsAsync()
    {
        var brands = await _brandRepository.GetAllAsync();
        return brands.Select(b => new BrandDto(b.Id, b.Name, b.Description));
    }

    public async Task<BrandDto?> GetBrandByIdAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        return brand == null ? null : new BrandDto(brand.Id, brand.Name, brand.Description);
    }

    public async Task<BrandDto> CreateBrandAsync(CreateBrandDto dto)
    {
        var brand = new Brand(dto.Name, dto.Description);
        await _brandRepository.AddAsync(brand);
        
        return new BrandDto(brand.Id, brand.Name, brand.Description);
    }

    public async Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandDto dto)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
            throw new DomainException("Бренд не найден");

        brand.UpdateInfo(dto.Name, dto.Description);
        await _brandRepository.UpdateAsync(brand);

        return new BrandDto(brand.Id, brand.Name, brand.Description);
    }

    public async Task DeleteBrandAsync(Guid id)
    {
        await _brandRepository.DeleteAsync(id);
    }
}