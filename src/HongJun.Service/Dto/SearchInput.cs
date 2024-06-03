namespace HongJun.Service.Dto;

public class SearchInput
{
    public string Query { get; set; }

    /// <summary>
    /// 搜索引擎类型
    /// </summary>
    public string Type { get; set; }
}