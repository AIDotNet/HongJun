using System.Net;
using HongJun.Service.Dto;

namespace HongJun.Service.Functions;

/// <summary>
/// Web functions
/// </summary>
public partial class WebService(
    IHttpClientFactory httpClientFactory,
    ILogger<WebService> logger)
{
    public async Task<WebSearchResult> SearchAsync(SearchInput query)
    {
        logger.LogInformation("SearchAsync: {0}", query.Query);


        var client = httpClientFactory.CreateClient(nameof(WebService));


        if (string.IsNullOrEmpty(query.Type))
        {
            query.Type = "bing";
        }

        var links = new List<string>();

        if (query.Type.Equals("baidu", StringComparison.InvariantCultureIgnoreCase))
        {
            return new WebSearchResult("", new List<string>());
        }
        else
        {
            var response = await client.GetAsync("https://api.bing.microsoft.com/v7.0/search?q=" +
                                                 WebUtility.UrlEncode(query.Query));

            var content = await response.Content.ReadFromJsonAsync<BingSearchResult>();


            links.AddRange(content.webPages.value.Select(x => $"#### {x.name} \n\n {x.snippet} \n\n [详情连接]({x.url})"));

            return new WebSearchResult(
                string.Join('\n',
                    content.webPages.value.Select(x => $"#### {x.name} \n\n {x.snippet} \n\n [详情连接]({x.url})").ToArray()),
                links);
        }
    }


    public class WebSearchResult(string markdown, List<string> links)

    {
        /// <summary>
        /// Md数据
        /// </summary>
        public string Markdown { get; set; } = markdown;

        /// <summary>
        /// 外连接
        /// </summary>
        public List<string> Links { get; set; } = links;
    }
}