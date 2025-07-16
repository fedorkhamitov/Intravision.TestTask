namespace Intravision.TestTask.Application.DTOs.Products;

public record ProductFilterDto(
    Guid? BrandId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null);