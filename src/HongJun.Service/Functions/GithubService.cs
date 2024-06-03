using System.Text.Json;
using HongJun.Service.Dto;
using HongJun.Service.Options;

namespace HongJun.Service.Functions;

public class GithubService
{
    private static readonly HttpClient HttpClient = new(new SocketsHttpHandler()
    {
        SslOptions =
        {
            RemoteCertificateValidationCallback = (_, _, _, _) => true
        },
    });

    static GithubService()
    {
        HttpClient.DefaultRequestHeaders.Add("User-Agent", "HongJun");
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {GithubOptions.Token}");
    }

    public async Task<GithubSearchRepositoriesDto> SearchRepositoriesAsync(string keyword)
    {
        try
        {
            var url = $"https://api.github.com/search/repositories?q={keyword}&per_page=5";

            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<GithubSearchRepositoriesDto>();
        }
        catch (Exception e)
        {
            return new GithubSearchRepositoriesDto();
        }
    }
}