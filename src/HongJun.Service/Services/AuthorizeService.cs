using System.Net.Http.Headers;
using HongJun.Service.DataAccess;
using HongJun.Service.Domina;
using HongJun.Service.Dto;
using HongJun.Service.Infrastructure.Helper;
using HongJun.Service.Options;
using Microsoft.EntityFrameworkCore;

namespace HongJun.Service.Services;

public sealed class AuthorizeService(IServiceProvider serviceProvider) : ApplicationService(serviceProvider)
{
    private static readonly HttpClient HttpClient = new(new SocketsHttpHandler()
    {
        SslOptions =
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        },
    });

    static AuthorizeService()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "HongJun");
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<object> GithubAsync(string code, MasterDbContext dbContext)
    {
        var response =
            await HttpClient.PostAsync(
                $"https://github.com/login/oauth/access_token?code={code}&client_id={GithubOptions.ClientId}&client_secret={GithubOptions.ClientSecret}",
                null);


        var result = await response.Content.ReadFromJsonAsync<GitTokenDto>();
        if (result is null) throw new Exception("Github授权失败");

        var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://api.github.com/user")
        {
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", result.access_token),
            },
        };

        var responseMessage = await HttpClient.SendAsync(request);

        var githubUser = await responseMessage.Content.ReadFromJsonAsync<GithubUserDto>();
        if (githubUser is null) throw new Exception("Github授权失败");

        if (githubUser.id < 1000) throw new Exception("Github授权失败");

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.GithubId == githubUser.id.ToString());

        if (user is null)
        {
            user = new User(githubUser.id.ToString(), githubUser.id.ToString(),
                githubUser.id + "@token-ai.cn",
                Guid.NewGuid().ToString("N"));
            user.SetUser();
            user.Avatar = githubUser.avatar_url;
            user.GithubId = githubUser.id.ToString();

            user.SetPassword(Guid.NewGuid().ToString("N"));

            user.SetResidualCredit(HongJunOptions.NewUserNumber);

            await dbContext.Users.AddAsync(user);

            await dbContext.SaveChangesAsync();
        }

        var token = JwtHelper.GeneratorAccessToken(user);

        return new
        {
            token = token,
            role = user.Role
        };
    }
}