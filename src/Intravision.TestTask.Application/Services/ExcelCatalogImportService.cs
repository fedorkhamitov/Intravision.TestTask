using System.Globalization;
using Intravision.TestTask.Application.Interfaces.Repositories;
using Intravision.TestTask.Domain.Entities;
using Intravision.TestTask.Domain.ValueObjects;
using OfficeOpenXml;

namespace Intravision.TestTask.Application.Services;

public class ExcelCatalogImportService
{
    private readonly IProductRepository _productRepository;
    private readonly IBrandRepository _brandRepository;

    public ExcelCatalogImportService(IProductRepository productRepository, IBrandRepository brandRepository)
    {
        _productRepository = productRepository;
        _brandRepository = brandRepository;
    }
    
    public async Task<int> ImportProductsAsync(Stream excelStream)
    {
        using var package = new ExcelPackage(excelStream);
        var worksheet = package.Workbook.Worksheets[0];

        var created = 0;
        var rowCount = worksheet.Dimension.Rows;

        // Получаем список брендов для быстрого сопоставления по имени
        var brands = (await _brandRepository.GetAllAsync()).ToList();
        var brandDict = brands.ToDictionary(b => b.Name.Trim().ToLowerInvariant(), b => b);

        for (var row = 2; row <= rowCount; row++) // 1 — заголовок!
        {
            var name = worksheet.Cells[row, 1].Text?.Trim();
            var description = worksheet.Cells[row, 2].Text?.Trim() ?? "";
            var priceRaw = worksheet.Cells[row, 3].Text?.Trim();
            var brandName = worksheet.Cells[row, 4].Text?.Trim().ToLowerInvariant();
            var stockRaw = worksheet.Cells[row, 5].Text?.Trim();

            if (string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(priceRaw)
                || string.IsNullOrWhiteSpace(brandName)
                || string.IsNullOrWhiteSpace(stockRaw))
                continue;

            if (!decimal.TryParse(priceRaw.Replace(',', '.'), 
                    NumberStyles.Any, 
                    CultureInfo.InvariantCulture, 
                    out var price))
                continue;
            
            if (!int.TryParse(stockRaw, out var stock))
                continue;

            if (!brandDict.TryGetValue(brandName, out var brand))
                continue;

            var product = new Product(name, description, new Money(price), brand.Id, stock);
            await _productRepository.AddAsync(product);
            created++;
        }
        return created;
    }
}
