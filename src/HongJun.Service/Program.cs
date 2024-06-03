using HongJun.Service.DataAccess;
using HongJun.Service.Dto;
using HongJun.Service.Functions;
using HongJun.Service.Infrastructure.Middlewares;
using HongJun.Service.Options;
using HongJun.Service.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using Microsoft.SemanticKernel;
using Serilog;

namespace HongJun.Service;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();

        builder.Host.UseSerilog(logger);

        builder.Configuration.GetSection("OpenAI")
            .Get<OpenAIOptions>();
        
        builder.Configuration.GetSection("Github")
            .Get<GithubOptions>();

        builder.Configuration.GetSection("HongJun")
            .Get<HongJunOptions>();

        builder.Configuration.GetSection("AzureBing")
            .Get<AzureBingOptions>();
        
        builder.Configuration.GetSection(JwtOptions.Name)
            .Get<JwtOptions>();
        
        builder.Services.AddMemoryCache()
            .AddHostedService<HeatBackgroundService>()
            .AddSingleton<WebService>()
            .AddSingleton<KernelFactory>()
            .AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "HongJun.Service", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            })
            .AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder => builder
                        .SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            })
            .AddHttpClient()
            .AddHttpClient(nameof(WebService), options =>
            {
                // 伪装成浏览器
                options.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");
                options.DefaultRequestHeaders.Add("Accept",
                    "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");

                options.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AzureBingOptions.SubscriptionKey);
            }).ConfigurePrimaryHttpMessageHandler((() =>
            {
                return new SocketsHttpHandler()
                {
                    SslOptions =
                    {
                        RemoteCertificateValidationCallback = (_, _, _, _) => true
                    },
                    // 保持长连接
                    PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                    MaxConnectionsPerServer = int.MaxValue,
                    KeepAlivePingTimeout = TimeSpan.FromMinutes(2),
                    KeepAlivePingDelay = TimeSpan.FromMinutes(1),
                };
            }));

        builder.Services.AddSingleton(_ =>
        {
            var kernelBuilder = Kernel.CreateBuilder();

            if (OpenAIOptions.Type == "AzureOpenAI")
            {
                kernelBuilder.AddAzureOpenAIChatCompletion(
                    deploymentName: OpenAIOptions.Model,
                    apiKey: OpenAIOptions.ApiKey,
                    endpoint: OpenAIOptions.Address);
            }
            else
            {
                kernelBuilder.AddOpenAIChatCompletion(
                    modelId: OpenAIOptions.Model,
                    apiKey: OpenAIOptions.ApiKey,
                    httpClient: new HttpClient(new OpenAIHttpClientHandler(OpenAIOptions.Address)));
            }

            var kernel = kernelBuilder
                .Build();

            return kernel;
        });

        builder.Services.AddDbContext<MasterDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
        });

        builder.Services.AddSingleton<SearchService>()
            .AddAuthorization()
            .AddHngJunService()
            .AddEndpointsApiExplorer()
            .AddResponseCompression()
            .AddSingleton<UnitOfWorkMiddleware>();

        var app = builder.AddServices(options =>
        {
            options.Prefix = "api"; //自定义前缀
            options.MapHttpMethodsForUnmatched = ["Post"]; //当请求类型匹配失败后，默认映射为Post请求 (当前项目范围内，除非范围配置单独指定)
        });

        app.UseCors("AllowAll");
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "HongJun.Service v1"); });


        app.Use((async (context, next) =>
        {
            // 解决react软路由问题

            if (context.Request.Path == "/")
            {
                context.Request.Path = "/index.html";
            }

            await next();

            if (context.Response.StatusCode == 404)
            {
                context.Request.Path = "/index.html";
                await next();
            }
        }));

        app.UseMiddleware<UnitOfWorkMiddleware>();

        app.UseResponseCompression();

        app.UseStaticFiles();

        app.MapPost("/api/v1/search",
            async ([FromServices] SearchService searchService, HttpContext context, SearchInput input) =>
            {
                await searchService.HandleRequest(context, input);
            });

        app.MapGet("/api/v1/heat",
            ([FromServices] SearchService searchService, IMemoryCache memoryCache) =>
                searchService.GetHeat(memoryCache));

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<MasterDbContext>();
            await dbContext.Database.MigrateAsync();
        }


        await app.RunAsync();
    }
}