using HongJun.Service.DataAccess;
using HongJun.Service.Dto;
using HongJun.Service.Exceptions;
using HongJun.Service.Options;
using Microsoft.Extensions.Caching.Memory;

namespace HongJun.Service.Services;

public sealed class UserService(IServiceProvider serviceProvider) : ApplicationService(serviceProvider)
{
    public async Task<UserInfoDto> GetAsync(MasterDbContext context, HttpContext httpContext, IMemoryCache memoryCache)
    {
        var user = await context.Users.FindAsync(UserContext.CurrentUserId);

        if (user is null)
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString();

            // 可能是网关的IP
            if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var header))
            {
                ip = header;
            }

            if (memoryCache.TryGetValue(ip, out int value))
            {
                return new UserInfoDto
                {
                    ResidualCredit = HongJunOptions.LimitDayNumber - value
                };
            }

            return new UserInfoDto
            {
                ResidualCredit = HongJunOptions.LimitDayNumber
            };
        }

        return Mapper.Map<UserInfoDto>(user);
    }
}