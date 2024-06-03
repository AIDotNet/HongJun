using System.Security.Claims;
using HongJun.Service.Domain.Core;

namespace HongJun.Service.Infrastructure;

public sealed class UserContext(IHttpContextAccessor httpContextAccessor)
{
    public string CurrentUserId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Sid)?.Value;
        }
    }

    public string CurrentUserName
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated ?? false;
        }
    }

    public bool IsAdmin
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.IsInRole(RoleConstant.Admin) ?? false;
        }
    }
}