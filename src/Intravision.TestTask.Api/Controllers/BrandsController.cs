using Intravision.TestTask.Application.DTOs.Brands;
using Intravision.TestTask.Application.DTOs.CommonDtos;
using Intravision.TestTask.Application.Interfaces.Services;
using Intravision.TestTask.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Intravision.TestTask.Api.Controllers;

/// <summary>
/// Контроллер для управления брендами в системе торгового автомата.
/// </summary>
/// <remarks>
/// Предоставляет полный набор CRUD операций для работы с брендами:
/// получение списка брендов, создание новых брендов, обновление и удаление существующих.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    /// <summary>
    /// Инициализация контроллера BrandsController.
    /// </summary>
    /// <param name="brandService">Сервис для работы с брендами.</param>
    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /// <summary>
    /// Получает список всех доступных брендов.
    /// </summary>
    /// <returns>Коллекция брендов с базовой информацией.</returns>
    /// <response code="200">Возвращает список всех брендов.</response>
    /// <response code="400">Возникла ошибка при получении списка брендов.</response>
    /// <example>
    /// GET /api/brands
    /// </example>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<BrandDto>>>> GetBrands()
    {
        try
        {
            var brands = await _brandService.GetBrandsAsync();
            
            return Ok(new ApiResponse<IEnumerable<BrandDto>>(
                true, brands, "Бренды получены"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<IEnumerable<BrandDto>>(
                false, null, ex.Message));
        }
    }

    /// <summary>
    /// Получает информацию о конкретном бренде по его идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор бренда в формате GUID.</param>
    /// <returns>Детальная информация о бренде.</returns>
    /// <response code="200">Возвращает информацию о найденном бренде.</response>
    /// <response code="404">Бренд с указанным идентификатором не найден.</response>
    /// <response code="400">Некорректный формат идентификатора или другая ошибка.</response>
    /// <example>
    /// GET /api/brands/12345678-1234-1234-1234-123456789012
    /// </example>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BrandDto>>> GetBrand(Guid id)
    {
        try
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            
            if (brand == null)
            {
                return NotFound(new ApiResponse<BrandDto>(
                    false, null, "Бренд не найден"));
            }

            return Ok(new ApiResponse<BrandDto>(
                true, brand, "Бренд найден"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<BrandDto>(
                false, null, ex.Message));
        }
    }

    /// <summary>
    /// Создает новый бренд в системе.
    /// </summary>
    /// <param name="dto">Данные для создания нового бренда.</param>
    /// <returns>Информация о созданном бренде.</returns>
    /// <response code="201">Бренд успешно создан.</response>
    /// <response code="400">Ошибка валидации данных или бизнес-логики.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    /// <example>
    /// POST /api/brands
    /// {
    ///   "name": "Coca-Cola",
    ///   "description": "Американская компания по производству напитков"
    /// }
    /// </example>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<BrandDto>>> CreateBrand(
        [FromBody] CreateBrandDto dto)
    {
        try
        {
            var brand = await _brandService.CreateBrandAsync(dto);
            
            return CreatedAtAction(
                nameof(GetBrand), 
                new { id = brand.Id }, 
                new ApiResponse<BrandDto>(true, brand, "Бренд создан"));
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<BrandDto>(
                false, null, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<BrandDto>(
                false, null, "Внутренняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Обновляет информацию о существующем бренде.
    /// </summary>
    /// <param name="id">Идентификатор бренда для обновления.</param>
    /// <param name="dto">Новые данные бренда.</param>
    /// <returns>Обновленная информация о бренде.</returns>
    /// <response code="200">Бренд успешно обновлен.</response>
    /// <response code="400">Ошибка валидации данных или бизнес-логики.</response>
    /// <response code="404">Бренд с указанным идентификатором не найден.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    /// <example>
    /// PUT /api/brands/12345678-1234-1234-1234-123456789012
    /// {
    ///   "name": "Pepsi",
    ///   "description": "Обновленное описание бренда"
    /// }
    /// </example>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<BrandDto>>> UpdateBrand(
        Guid id, [FromBody] UpdateBrandDto dto)
    {
        try
        {
            var brand = await _brandService.UpdateBrandAsync(id, dto);
            
            return Ok(new ApiResponse<BrandDto>(
                true, brand, "Бренд обновлен"));
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<BrandDto>(
                false, null, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<BrandDto>(
                false, null, "Внутренняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Удаляет бренд из системы.
    /// </summary>
    /// <param name="id">Идентификатор бренда для удаления.</param>
    /// <returns>Подтверждение удаления бренда.</returns>
    /// <response code="200">Бренд успешно удален.</response>
    /// <response code="400">Ошибка при удалении бренда (например, есть связанные товары).</response>
    /// <response code="404">Бренд с указанным идентификатором не найден.</response>
    /// <example>
    /// DELETE /api/brands/12345678-1234-1234-1234-123456789012
    /// </example>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBrand(Guid id)
    {
        try
        {
            await _brandService.DeleteBrandAsync(id);
            
            return Ok(new ApiResponse<object>(
                true, null, "Бренд удален"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(
                false, null, ex.Message));
        }
    }
}