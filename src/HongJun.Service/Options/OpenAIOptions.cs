namespace HongJun.Service.Options;

public class OpenAIOptions
{
    public static string Model { get; set; }
    
    public static string ApiKey { get; set; }

    public static string Address { get; set; }
    
    /// <summary>
    /// AzureOpenAI/OpenAI
    /// </summary>
    public static string Type { get; set; }
    
    /// <summary>
    /// 分析模型
    /// </summary>
    public static string AnalysisModel { get; set; }
}