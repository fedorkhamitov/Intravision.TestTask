namespace Intravision.TestTask.Application.DTOs.CommonDtos;

public record ApiResponse<T>(
    bool Success,
    T? Data,
    string? Message = null,
    IEnumerable<string>? Errors = null);