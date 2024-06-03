using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HongJun.Service.Domina;
using HongJun.Service.Options;
using Microsoft.IdentityModel.Tokens;

namespace HongJun.Service.Infrastructure.Helper;

public class JwtHelper
{
    /// <summary>
    /// 生成token
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <returns></returns>
    public static string GeneratorAccessToken(ClaimsIdentity claimsIdentity)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(JwtOptions.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddHours(JwtOptions.Effective),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// 生成token
    /// </summary>
    /// <param name="claimsIdentity"></param>
    /// <returns></returns>
    public static string GeneratorAccessToken(User user)
    {
        var claimsIdentity = GetClaimsIdentity(user);

        return GeneratorAccessToken(claimsIdentity);
    }


    public static ClaimsIdentity GetClaimsIdentity(User user)
    {
        return new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Sid, user.Id),
            new(ClaimTypes.Role, user.Role),
        });
    }
}