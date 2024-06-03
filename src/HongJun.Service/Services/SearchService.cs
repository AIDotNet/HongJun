using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using HongJun.Service.DataAccess;
using HongJun.Service.Dto;
using HongJun.Service.Functions;
using HongJun.Service.Infrastructure;
using HongJun.Service.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace HongJun.Service.Services;

public sealed class SearchService(
    Kernel kernel,
    ILogger<SearchService> logger,
    IMemoryCache memoryCache,
    KernelFactory kernelFactory,
    GithubService githubService,
    WebService webService)
{
    private readonly ConcurrentDictionary<string, Task> _ipTasks = new();

    private readonly IChatCompletionService
        _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

    private static readonly OpenAIPromptExecutionSettings OpenAiPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    private static KernelPlugin? _basePlugins;

    private KernelPlugin BasePlugins
    {
        get
        {
            if (_basePlugins == null)
            {
                var pluginsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "plugins", "BasePlugins");

                _basePlugins = kernel
                    .ImportPluginFromPromptDirectory(pluginsDirectory);
            }

            return _basePlugins;
        }
    }

    public async Task HandleRequest(HttpContext context, SearchInput input)
    {
        var userContext = context.RequestServices.GetRequiredService<UserContext>();

        if (userContext.IsAuthenticated)
        {
            var dbContext = context.RequestServices.GetRequiredService<MasterDbContext>();
            try
            {
                var user = await dbContext.Users.FindAsync(userContext.CurrentUserId);

                if (user == null)
                {
                    await WriteErrorAsync(context, "用户不存在", 404);
                    return;
                }

                if (user.ResidualCredit <= 0)
                {
                    await WriteErrorAsync(context, "抱歉您的体验次数已用完。", 429);
                    return;
                }

                // 返回sse
                context.Response.Headers.ContentType = "text/event-stream";
                await SearchAsync(context, input);

                await dbContext.Users.Where(x => x.Id == user.Id)
                    .ExecuteUpdateAsync(item => item.SetProperty(x => x.ResidualCredit, x => x.ResidualCredit - 1));
            }
            catch (Exception e)
            {
                logger.LogError(e, "HandleRequest Error");
                await WriteErrorAsync(context, e.Message);
            }

            return;
        }

        var ip = context.Connection.RemoteIpAddress?.ToString();

        // 可能是网关的IP
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var header))
        {
            ip = header;
        }

        logger.LogInformation("ip: {0}", ip);


        if (_ipTasks.TryGetValue(ip, out var ipTask))
        {
            await WriteErrorAsync(context, "请等待上一个请求完成。", 429);
            return;
        }

        if (memoryCache.TryGetValue(ip, out int value))
        {
            if (value >= HongJunOptions.LimitDayNumber)
            {
                await WriteErrorAsync(context, "抱歉您今天体验次数已用完。", 429);
                return;
            }

            memoryCache.Set(ip, value + 1);
        }
        else
        {
            memoryCache.Set(ip, 1, new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
            });
        }

        // 返回sse
        context.Response.Headers.ContentType = "text/event-stream";

        var task = SearchAsync(context, input);
        _ipTasks[ip] = task;

        try
        {
            await task;
        }
        finally
        {
            // 当任务完成时，从字典中移除
            _ipTasks.TryRemove(ip, out _);
        }
    }

    public List<string> GetHeat(IMemoryCache memoryCache)
    {
        if (memoryCache.TryGetValue(nameof(HeatBackgroundService), out List<string> value))
        {
            return value;
        }

        return new List<string>();
    }

    public async Task SearchAsync(HttpContext context, SearchInput input)
    {
        if (input.Type.Equals("bing") || string.IsNullOrEmpty(input.Type))
        {
            await BingSearchAsync(context, input);
        }
        else if (input.Type.Equals("github"))
        {
            await GithubSearchAsync(context, input);
        }
    }

    public async ValueTask GithubSearchAsync(HttpContext context, SearchInput input)
    {
        logger.LogInformation("SearchAsync: {0} 搜索引擎类型：{1}", input.Query, input.Type);

        var sw = Stopwatch.StartNew();
        try
        {
            // 优化用户提问，以便在Github更好找到结果
            var githubQuery = await kernelFactory.CreateKernel(OpenAIOptions.AnalysisModel)
                .InvokeAsync(BasePlugins["GithubChatRepositories"], new KernelArguments()
                {
                    ["input"] = input.Query
                });

            await WriteAsync(context, new SearchResult()
            {
                Step = 0,
                Content = githubQuery.ToString()
            }).ConfigureAwait(false);

            var items = new List<GithubSearchRepositoriesItemsDto>();

            foreach (var query in JsonSerializer.Deserialize<string[]>(githubQuery.ToString()))
            {
                var githubRes = await githubService.SearchRepositoriesAsync(query);
                if (githubRes?.items != null)
                {
                    items.AddRange(githubRes.items);
                }
            }

            await WriteAsync(context, new SearchResult()
            {
                Step = 1,
                Content = JsonSerializer.Serialize(items)
            }).ConfigureAwait(false);

            await WriteAsync(context, new SearchResult()
            {
                Step = 2,
            }).ConfigureAwait(false);

            var respositors = await _chatCompletionService.GetChatMessageContentsAsync(
                new ChatHistory(PromptConstant.GithubRepoPrompt)
                {
                    new(AuthorRole.User,
                        string.Join('\n', items.Select(x => JsonSerializer.Serialize(x))))
                });

            await WriteAsync(context, new SearchResult()
            {
                Step = 3,
            }).ConfigureAwait(false);

            var githubQA = string.Format(PromptConstant.GithubRecommendPrompt, input.Query,
                respositors.FirstOrDefault()?.Content);

            await foreach (var item in _chatCompletionService.GetStreamingChatMessageContentsAsync([
                               new(AuthorRole.User, githubQA)
                           ]))
            {
                if (string.IsNullOrEmpty(item.Content))
                {
                    continue;
                }

                var result = new SearchResult()
                {
                    Content = item.Content ?? string.Empty,
                    Step = 4
                };
                await WriteAsync(context, result).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            sw.Stop();
        }

        await WriteAsync(context, new SearchResult()
        {
            Step = 5,
            ElapsedTime = (int?)sw.ElapsedMilliseconds,
        });
    }

    public async ValueTask BingSearchAsync(HttpContext context, SearchInput input)
    {
        logger.LogInformation("SearchAsync: {0} 搜索引擎类型：{1}", input.Query, input.Type);
        var sw = Stopwatch.StartNew();
        try
        {
            logger.LogInformation("SearchAsync: {0} step 1", input.Query);

            await WriteAsync(context, new SearchResult()
            {
                Step = 0,
                Content = input.Query
            }).ConfigureAwait(false);

            var html = await webService.SearchAsync(input).ConfigureAwait(false);

            await WriteAsync(context, new SearchResult()
            {
                Step = -1,
                Content = string.Join("\n", html.Links)
            }).ConfigureAwait(false);

            await WriteAsync(context, new SearchResult()
            {
                Step = 1,
                Content = html.Links.Count.ToString()
            }).ConfigureAwait(false);

            var user = string.Format(PromptConstant.SearchUserPrompt, html.Markdown,
                input.Query);

            // 调用对话返回结果。
            await foreach (var item in _chatCompletionService.GetStreamingChatMessageContentsAsync([
                               new(AuthorRole.User, user)
                           ]))
            {
                if (string.IsNullOrEmpty(item.Content))
                {
                    continue;
                }

                var result = new SearchResult()
                {
                    Content = item.Content ?? string.Empty,
                    Step = 2
                };
                await WriteAsync(context, result).ConfigureAwait(false);
            }

            logger.LogInformation("SearchAsync: {0} completed", input.Query);

            await WriteAsync(context, new SearchResult()
            {
                Step = 3,
            }).ConfigureAwait(false);
        }
        finally
        {
            sw.Stop();
        }

        try
        {
            var content = await _chatCompletionService.GetChatMessageContentsAsync(
                new ChatHistory(PromptConstant.QuestionPrompt)
                {
                    new(AuthorRole.User, input.Query),
                }, new OpenAIPromptExecutionSettings()
                {
                    Temperature = 0.5,
                });

            var result = JsonSerializer.Deserialize<string[]>(content.FirstOrDefault()?.Content);

            if (result != null)
            {
                await WriteAsync(context, new SearchResult()
                {
                    Step = 4,
                    Content = JsonSerializer.Serialize(result)
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        await WriteAsync(context, new SearchResult()
        {
            Step = 5,
            ElapsedTime = (int?)sw.ElapsedMilliseconds,
        });
    }

    public static async ValueTask WriteErrorAsync(HttpContext context, string error, int code = 400)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new SearchResult()
        {
            Error = error
        }, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        })).ConfigureAwait(false);
    }

    public static async ValueTask WriteAsync(HttpContext context, SearchResult result)
    {
        await context.Response.WriteAsync("data: " + JsonSerializer.Serialize(result, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        }) + "\n\n").ConfigureAwait(false);
        await context.Response.Body.FlushAsync().ConfigureAwait(false);
    }

    public sealed class SearchResult
    {
        public string Content { get; set; }

        public string? Error { get; set; }

        public int Step { get; set; }

        /// <summary>
        /// 消耗时间
        /// </summary>
        public int? ElapsedTime { get; set; }
    }
}