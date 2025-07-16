namespace Intravision.TestTask.Application.DTOs;

public record ProductFilterDto(
    Guid? BrandId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null);