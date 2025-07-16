using Intravision.TestTask.Application.DTOs;
using Intravision.TestTask.Application.DTOs.Brands;

namespace Intravision.TestTask.Application.Interfaces.Services;

public interface IBrandService
{
    Task<IEnumerable<BrandDto>> GetBrandsAsync();
    Task<BrandDto?> GetBrandByIdAsync(Guid id);
    Task<BrandDto> CreateBrandAsync(CreateBrandDto dto);
    Task<BrandDto> UpdateBrandAsync(Guid id, UpdateBrandDto dto);
    Task DeleteBrandAsync(Guid id);
}