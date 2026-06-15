namespace EmployeeManagement.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> ErrorResponse(string message, IEnumerable<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Errors = errors
        };
    }

    public static ApiResponse<T> ValidationErrorResponse(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        var message = errorList.Count == 1 ? errorList[0] : "Validation failed.";
        return ErrorResponse(message, errorList);
    }
}
