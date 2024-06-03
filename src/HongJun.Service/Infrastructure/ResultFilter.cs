using HongJun.Service.Dto;

namespace HongJun.Service.Infrastructure;

public sealed class ResultFilter(ILogger<ResultFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            var result = await next(context);

            if (result is ResultDto resultDto)
            {
                context.HttpContext.Response.StatusCode = 200;
                context.HttpContext.Response.ContentType = "application/json";
                return resultDto;
            }

            if (result == null)
            {
                return result;
            }

            return ResultDto.SuccessResult("Success", result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while processing the request");
            context.HttpContext.Response.StatusCode = 200;
            return ResultDto.FailResult(e.Message);
        }
    }
}