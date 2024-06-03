namespace HongJun.Service.Dto;

public sealed class ResultDto
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public object? Data { get; set; }

    public ResultDto(bool success, string? message, object? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public ResultDto(bool success, string? message)
    {
        Success = success;
        Message = message;
    }

    public static ResultDto SuccessResult(string message, object data)
    {
        return new ResultDto(true, message, data);
    }

    public static ResultDto SuccessResult(string message)
    {
        return new ResultDto(true, message);
    }

    public static ResultDto FailResult(string message, object data)
    {
        return new ResultDto(false, message, data);
    }

    public static ResultDto FailResult(string message)
    {
        return new ResultDto(false, message);
    }
}

public sealed class ResultDto<T>
{
    public bool Success { get; set; }

    public string? Message { get; set; }

    public T? Data { get; set; }

    public ResultDto(bool success, string? message, T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public ResultDto(bool success, string? message)
    {
        Success = success;
        Message = message;
    }

    public static ResultDto<T> SuccessResult(string message, T data)
    {
        return new ResultDto<T>(true, message, data);
    }

    public static ResultDto<T> SuccessResult(string message)
    {
        return new ResultDto<T>(true, message);
    }

    public static ResultDto<T> FailResult(string message, T data)
    {
        return new ResultDto<T>(false, message, data);
    }

    public static ResultDto<T> FailResult(string message)
    {
        return new ResultDto<T>(false, message);
    }
}