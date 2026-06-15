using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using EmployeeManagement.Application.Common;

namespace EmployeeManagement.API.ExceptionHandling;

/// <summary>
/// Maps unhandled exceptions to <see cref="ApiResponse{T}"/> for consistent error responses.
/// Handles validation errors, not found errors, and unexpected exceptions.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Handle FluentValidation errors
        if (exception is ValidationException validationException)
        {
            _logger.LogDebug("Validation failed for {Path}", httpContext.Request.Path);
            await WriteResponseAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.ValidationErrorResponse(
                    validationException.Errors.Select(e => e.ErrorMessage).ToList()),
                cancellationToken);
            return true;
        }

        var (statusCode, response) = MapException(exception);

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception at {Path}", httpContext.Request.Path);
        else
            _logger.LogDebug(exception, "Client error at {Path}", httpContext.Request.Path);

        await WriteResponseAsync(httpContext, statusCode, response, cancellationToken);
        return true;
    }

    private (int StatusCode, ApiResponse<object> Response) MapException(Exception exception)
    {
        return exception switch
        {
            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.ErrorResponse(ex.Message)),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                ApiResponse<object>.ErrorResponse("Unauthorized")),

            KeyNotFoundException ex => (
                StatusCodes.Status404NotFound,
                ApiResponse<object>.ErrorResponse(ex.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    _environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred. Please try again later or contact support."))
        };
    }

    private static Task WriteResponseAsync(
        HttpContext httpContext,
        int statusCode,
        ApiResponse<object> response,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        return httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    }
}
