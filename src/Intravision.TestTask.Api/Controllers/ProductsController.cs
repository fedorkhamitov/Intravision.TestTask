using Intravision.TestTask.Application.DTOs.CommonDtos;
using Intravision.TestTask.Application.DTOs.Products;
using Intravision.TestTask.Application.Interfaces.Services;
using Intravision.TestTask.Application.Services;
using Intravision.TestTask.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Intravision.TestTask.Api.Controllers;

/// <summary>
/// Контроллер для управления продуктами торгового автомата.
/// </summary>
/// <remarks>
/// Предоставляет операции CRUD для товаров, включая получение списка,
/// создание, обновление и удаление товаров. Также поддерживает блокировку
/// автомата для эксклюзивного доступа к каталогу товаров.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    /// <summary>
    /// Инициализация контроллера ProductsController.
    /// </summary>
    /// <param name="productService">Сервис для работы с продуктами.</param>
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Получает список продуктов с возможностью фильтрации.
    /// </summary>
    /// <param name="filter">Параметры фильтрации продуктов.</param>
    /// <returns>Список продуктов, соответствующих критериям фильтрации.</returns>
    /// <response code="200">Возвращает список продуктов.</response>
    /// <response code="423">Автомат заблокирован другой сессией.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <example>
    /// GET /api/products?brandId=12345&minPrice=10&maxPrice=100
    /// </example>
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] ProductFilterDto filter)
    {
        try
        {
            var products = await _productService.GetProductsAsync(filter);
            return Ok(new ApiResponse<IEnumerable<ProductDto>>(
                true, products, "Товары успешно получены"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<IEnumerable<ProductDto>>(
                false, null, ex.Message));
        }
    }

    /// <summary>
    /// Получает информацию о конкретном продукте по его идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор продукта.</param>
    /// <returns>Информация о продукте.</returns>
    /// <response code="200">Возвращает информацию о продукте.</response>
    /// <response code="404">Продукт не найден.</response>
    /// <response code="400">Некорректный идентификатор продукта.</response>
    /// <example>
    /// GET /api/products/12345678-1234-1234-1234-123456789012
    /// </example>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetProduct(Guid id)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                return NotFound(new ApiResponse<ProductDto>(
                    false, null, "Товар не найден"));
            }

            return Ok(new ApiResponse<ProductDto>(
                true, product, "Товар найден"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<ProductDto>(
                false, null, ex.Message));
        }
    }

    /// <summary>
    /// Получает диапазон цен для продуктов, опционально отфильтрованных по бренду.
    /// </summary>
    /// <param name="brandId">Идентификатор бренда для фильтрации (необязательно).</param>
    /// <returns>Минимальная и максимальная цена продуктов.</returns>
    /// <response code="200">Возвращает диапазон цен.</response>
    /// <response code="400">Ошибка при получении диапазона цен.</response>
    /// <example>
    /// GET /api/products/price-range?brandId=12345678-1234-1234-1234-123456789012
    /// </example>
    [HttpGet("price-range")]
    public async Task<ActionResult<ApiResponse<PriceRangeDto>>> GetPriceRange(
        [FromQuery] Guid? brandId = null)
    {
        try
        {
            var priceRange = await _productService.GetPriceRangeAsync(brandId);
            
            return Ok(new ApiResponse<PriceRangeDto>(
                true, priceRange, "Диапазон цен получен"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<PriceRangeDto>(
                false, null, ex.Message));
        }
    }

    /// <summary>
    /// Создает новый продукт в каталоге.
    /// </summary>
    /// <param name="dto">Данные для создания нового продукта.</param>
    /// <returns>Информация о созданном продукте.</returns>
    /// <response code="201">Продукт успешно создан.</response>
    /// <response code="400">Некорректные данные для создания продукта.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    /// <example>
    /// POST /api/products
    /// {
    ///   "name": "Coca Cola",
    ///   "price": 50.00,
    ///   "brandId": "12345678-1234-1234-1234-123456789012"
    /// }
    /// </example>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct(
        [FromBody] CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateProductAsync(dto);
            
            return CreatedAtAction(
                nameof(GetProduct), 
                new { id = product.Id }, 
                new ApiResponse<ProductDto>(true, product, "Товар создан"));
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<ProductDto>(
                false, null, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ProductDto>(
                false, null, "Внутренняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Обновляет информацию о существующем продукте.
    /// </summary>
    /// <param name="id">Идентификатор продукта для обновления.</param>
    /// <param name="dto">Новые данные продукта.</param>
    /// <returns>Обновленная информация о продукте.</returns>
    /// <response code="200">Продукт успешно обновлен.</response>
    /// <response code="400">Некорректные данные для обновления продукта.</response>
    /// <response code="404">Продукт не найден.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    /// <example>
    /// PUT /api/products/12345678-1234-1234-1234-123456789012
    /// {
    ///   "name": "Pepsi Cola",
    ///   "price": 45.00
    /// }
    /// </example>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(
        Guid id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(id, dto);
            
            return Ok(new ApiResponse<ProductDto>(
                true, product, "Товар обновлен"));
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<ProductDto>(
                false, null, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ProductDto>(
                false, null, "Внутренняя ошибка сервера"));
        }
    }

    /// <summary>
    /// Удаляет продукт из каталога.
    /// </summary>
    /// <param name="id">Идентификатор продукта для удаления.</param>
    /// <returns>Подтверждение удаления продукта.</returns>
    /// <response code="200">Продукт успешно удален.</response>
    /// <response code="400">Ошибка при удалении продукта.</response>
    /// <response code="404">Продукт не найден.</response>
    /// <example>
    /// DELETE /api/products/12345678-1234-1234-1234-123456789012
    /// </example>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(Guid id)
    {
        try
        {
            await _productService.DeleteProductAsync(id);
            
            return Ok(new ApiResponse<object>(
                true, null, "Товар удален"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>(
                false, null, ex.Message));
        }
    }
    
    /// <summary>
    /// Импортирует продукты из Excel-файла.
    /// </summary>
    /// <param name="file">Excel-файл с данными продуктов.</param>
    /// <param name="excelImportService">Сервис для импорта данных из Excel.</param>
    /// <returns>Количество импортированных продуктов.</returns>
    /// <response code="200">Импорт успешно завершен.</response>
    /// <response code="400">Файл пустой или некорректный.</response>
    /// <response code="500">Ошибка при импорте данных.</response>
    /// <example>
    /// POST /api/products/import-excel
    /// Content-Type: multipart/form-data
    /// file: products.xlsx
    /// </example>
    [HttpPost("import-excel")]
    public async Task<IActionResult> ImportExcel(
        IFormFile? file,
        [FromServices] ExcelCatalogImportService excelImportService)
    {
        if (file == null || file.Length == 0) return BadRequest("Файл пустой");
        await using var stream = file.OpenReadStream();
        var created = await excelImportService.ImportProductsAsync(stream).ConfigureAwait(false);
        return Ok(new { imported = created });
    }
}