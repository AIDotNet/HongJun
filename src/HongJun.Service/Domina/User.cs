using HongJun.Service.Domina.Core;
using HongJun.Service.Infrastructure.Helper;

namespace HongJun.Service.Domina;


public sealed class User : Entity<string>
{
    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string PasswordHas { get; set; } = null!;

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// 角色
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 是否禁用
    /// </summary>
    public bool IsDisabled { get; set; }

    public bool IsDelete { get; set; }

    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// 账号额度
    /// </summary>
    public long ResidualCredit { get; set; }

    public string? GithubId { get; set; }

    public string? GiteeId { get; set; }
   
    protected User()
    {
    }

    public User(string id, string userName, string email, string password)
    {
        Id = id;
        UserName = userName;
        Email = email;
        SetUser();
        SetPassword(password);
        IsDisabled = false;
        IsDelete = false;
        DeletedAt = null;
    }

    public void SetAdmin()
    {
        Role = RoleConstant.Admin;
    }

    public void SetUser()
    {
        Role = RoleConstant.User;
    }

    public void SetPassword(string password)
    {
        PasswordHas = Guid.NewGuid().ToString("N");
        Password = StringHelper.HashPassword(password, PasswordHas);
    }

    public void SetResidualCredit(long residualCredit)
    {
        ResidualCredit = residualCredit;
    }

    public bool VerifyPassword(string password)
    {
        return StringHelper.HashPassword(password, PasswordHas) == Password;
    }
}