using HongJun.Service.DataAccess;

namespace HongJun.Service.Infrastructure.Middlewares;

public sealed class UnitOfWorkMiddleware(ILogger<UnitOfWorkMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 如果不是Get则自动开启事务
        if (context.Request.Method != "GET" && context.Request.Method != "OPTIONS" &&
            context.Request.Method != "HEAD" && context.Request.Method != "TRACE" &&
            context.Request.Method != "CONNECT")
        {
            var dbContext = context.RequestServices.GetRequiredService<MasterDbContext>();

            try
            {
                await next(context);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred during the transaction. Message: {Message}",
                    exception.Message);
                
            }

            return;
        }

        await next(context);
    }
}