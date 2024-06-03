namespace HongJun.Service.Options;

public class HongJunOptions
{
    /// <summary>
    /// 限制每天用户体验次数
    /// </summary>
    public static int LimitDayNumber { get; set; }
    
    /// <summary>
    /// 新用户体验次数
    /// </summary>
    public static int NewUserNumber { get; set; }
}