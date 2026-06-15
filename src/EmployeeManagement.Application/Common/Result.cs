namespace EmployeeManagement.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }
    public IEnumerable<string>? Errors { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, T? value, string? error, IEnumerable<string>? errors, int statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors;
        StatusCode = statusCode;
    }

    public static Result<T> Success(T value, int statusCode = 200)
        => new(true, value, null, null, statusCode);

    public static Result<T> Failure(string error, int statusCode = 400)
        => new(false, default, error, null, statusCode);

    public static Result<T> Failure(IEnumerable<string> errors, int statusCode = 400)
    {
        var errorList = errors.ToList();
        var message = errorList.Count == 1 ? errorList[0] : string.Join("; ", errorList);
        return new(false, default, message, errorList, statusCode);
    }
}

public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public IEnumerable<string>? Errors { get; }
    public int StatusCode { get; }

    private Result(bool isSuccess, string? error, IEnumerable<string>? errors, int statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
        StatusCode = statusCode;
    }

    public static Result Success(int statusCode = 200)
        => new(true, null, null, statusCode);

    public static Result Failure(string error, int statusCode = 400)
        => new(false, error, null, statusCode);

    public static Result Failure(IEnumerable<string> errors, int statusCode = 400)
    {
        var errorList = errors.ToList();
        var message = errorList.Count == 1 ? errorList[0] : string.Join("; ", errorList);
        return new(false, message, errorList, statusCode);
    }
}
