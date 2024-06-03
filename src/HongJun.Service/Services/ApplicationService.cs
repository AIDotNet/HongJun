using HongJun.Service.DataAccess;
using HongJun.Service.Infrastructure;
using HongJun.Service.Infrastructure.Helper;
using MapsterMapper;
using Masa.Contrib.Service.MinimalAPIs;

namespace HongJun.Service.Services;

public abstract class ApplicationService(IServiceProvider serviceProvider) : ServiceBase
{
    protected T GetService<T>()
    {
        return serviceProvider.GetRequiredService<T>();
    }

    public ILogger<T> GetLogger<T>()
    {
        return serviceProvider.GetRequiredService<ILogger<T>>();
    }

    protected T? GetKeyedService<T>(string key)
    {
        return serviceProvider.GetKeyedService<T>(key);
    }

    protected T GetRequiredKeyedService<T>(string key)
        => serviceProvider.GetRequiredKeyedService<T>(key);

    protected UserContext UserContext => GetService<UserContext>();


    protected IMapper Mapper => GetService<IMapper>();

    protected override RouteHandlerBuilder MapMethods(ServiceRouteOptions globalOptions, string pattern,
        string? httpMethod, Delegate handler)
    {
        var router = base.MapMethods(globalOptions, pattern, httpMethod, handler);

        router.AddEndpointFilter<ResultFilter>();

        return router;
    }
}