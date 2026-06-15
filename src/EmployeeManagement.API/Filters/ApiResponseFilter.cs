using EmployeeManagement.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.API.Filters;

/// <summary>
/// Wraps payloads in <see cref="ApiResponse{T}"/>.
/// Successes have Success=true and data. Errors have Success=false and a message + optional errors list.
/// Converts Result{T} to ApiResponse{T} automatically.
/// </summary>
public sealed class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: not null } objectResult
            && objectResult.StatusCode is not StatusCodes.Status204NoContent)
        {
            // Convert Result<T> to ApiResponse<T>
            if (IsResultType(objectResult.Value))
            {
                objectResult.Value = ConvertResultToApiResponse(objectResult.Value);
                await next();
                return;
            }

            // Skip wrapping if already wrapped in ApiResponse
            if (IsAlreadyWrapped(objectResult.Value))
            {
                await next();
                return;
            }

            // Wrap regular responses
            if (objectResult.StatusCode >= 400)
            {
                string errorMessage = "An error occurred.";
                IEnumerable<string>? errorList = null;

                // If ASP.NET Core returns multiple validation errors (e.g. FluentValidation failures)
                if (objectResult.Value is HttpValidationProblemDetails validationProblem)
                {
                    errorMessage = validationProblem.Title ?? "Validation failed";
                    errorList = validationProblem.Errors.SelectMany(e => e.Value);
                }
                else if (objectResult.Value is ProblemDetails problem)
                {
                    errorMessage = problem.Detail ?? problem.Title ?? errorMessage;
                }
                else
                {
                    var errorProp = objectResult.Value.GetType().GetProperty("error");
                    if (errorProp != null)
                    {
                        errorMessage = errorProp.GetValue(objectResult.Value)?.ToString() ?? errorMessage;
                    }
                    else if (objectResult.Value is string str)
                    {
                        errorMessage = str;
                    }
                }

                objectResult.Value = ApiResponse<object>.ErrorResponse(errorMessage, errorList);
            }
            else
            {
                objectResult.Value = ApiResponse<object>.SuccessResponse(objectResult.Value);
            }
        }

        await next();
    }

    private static bool IsAlreadyWrapped(object value) =>
        value.GetType().IsGenericType
        && value.GetType().GetGenericTypeDefinition() == typeof(ApiResponse<>);

    private static bool IsResultType(object value)
    {
        if (value == null) return false;
        var type = value.GetType();
        return type.IsGenericType
            && type.GetGenericTypeDefinition() == typeof(Result<>);
    }

    private static object ConvertResultToApiResponse(object resultValue)
    {
        var resultType = resultValue.GetType();
        var isSuccessProperty = resultType.GetProperty("IsSuccess");
        var valueProperty = resultType.GetProperty("Value");
        var errorProperty = resultType.GetProperty("Error");
        var errorsProperty = resultType.GetProperty("Errors");
        var statusCodeProperty = resultType.GetProperty("StatusCode");

        if (isSuccessProperty == null || valueProperty == null || errorProperty == null)
            return resultValue;

        var isSuccess = (bool?)isSuccessProperty.GetValue(resultValue) ?? false;
        var value = valueProperty.GetValue(resultValue);
        var error = (string?)errorProperty.GetValue(resultValue);
        var errors = (IEnumerable<string>?)errorsProperty?.GetValue(resultValue);
        var statusCode = (int?)statusCodeProperty?.GetValue(resultValue) ?? (isSuccess ? 200 : 400);

        var dataType = valueProperty.PropertyType;
        var apiResponseType = typeof(ApiResponse<>).MakeGenericType(dataType);
        var apiResponseInstance = Activator.CreateInstance(apiResponseType)!;

        var successProp = apiResponseType.GetProperty("Success");
        var dataProp = apiResponseType.GetProperty("Data");
        var messageProp = apiResponseType.GetProperty("Message");
        var errorsProp = apiResponseType.GetProperty("Errors");

        successProp?.SetValue(apiResponseInstance, isSuccess);
        dataProp?.SetValue(apiResponseInstance, isSuccess ? value : null);
        messageProp?.SetValue(apiResponseInstance, error);
        errorsProp?.SetValue(apiResponseInstance, errors);

        return apiResponseInstance;
    }
}
