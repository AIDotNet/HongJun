using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;

namespace HongJun.Service.Services;

public sealed class HeatBackgroundService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache,
    ILogger<HeatBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            using var client = httpClientFactory.CreateClient(nameof(HeatBackgroundService));

            // 伪装成浏览器
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
            client.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

            var html = await client.GetStringAsync("https://www.baidu.com/?tn=68018901_16_pg", stoppingToken);

            var web = new HtmlDocument();
            web.LoadHtml(html);

            // 获取 ul id=hotsearch-content-wrapper
            var node = web.DocumentNode.SelectSingleNode("//ul[@id='hotsearch-content-wrapper']");

            // 获取所有 class title-content-title
            var title = node.SelectNodes("//span[@class='title-content-title']");

            logger.LogInformation("获取到热搜：{0}", title.Count);

            foreach (var item in title)
            {
                logger.LogInformation("热搜：{0}", item.InnerText);
            }

            memoryCache.Set(nameof(HeatBackgroundService), title.Select(x => x.InnerText).ToList());

            // 等待一小时
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            logger.LogInformation("等待一小时后继续执行，更新热搜");
        }
    }
}