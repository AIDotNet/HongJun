using System.Collections.Concurrent;
using HongJun.Service.Options;
using Microsoft.SemanticKernel;

namespace HongJun.Service.Services;

public sealed class KernelFactory(ILogger<KernelFactory> logger)
{
    private readonly ConcurrentDictionary<string, Kernel> _kernels = new();

    public Kernel CreateKernel(string model)
    {
        if (_kernels.TryGetValue(model, out var kernel))
        {
            logger.LogInformation("Kernel {0} already exists", model);
            return kernel;
        }

        var kernelBuilder = Kernel.CreateBuilder();

        if (OpenAIOptions.Type == "AzureOpenAI")
        {
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: model,
                apiKey: OpenAIOptions.ApiKey,
                endpoint: OpenAIOptions.Address);
        }
        else
        {
            kernelBuilder.AddOpenAIChatCompletion(
                modelId: model,
                apiKey: OpenAIOptions.ApiKey,
                httpClient: new HttpClient(new OpenAIHttpClientHandler(OpenAIOptions.Address)));
        }

        kernel = kernelBuilder
            .Build();

        _kernels.TryAdd(model, kernel);

        return kernel;
    }
}