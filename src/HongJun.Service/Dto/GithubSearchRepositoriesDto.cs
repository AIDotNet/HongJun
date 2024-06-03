namespace HongJun.Service.Dto;

public class GithubSearchRepositoriesDto
{
    public int total_count { get; set; }
    
    public bool incomplete_results { get; set; }
    
    public GithubSearchRepositoriesItemsDto[] items { get; set; }
}

public class GithubSearchRepositoriesItemsDto
{
    public string name { get; set; }
    public string html_url { get; set; }
    public string description { get; set; }
    public string homepage { get; set; }
    public int stargazers_count { get; set; }
    public string language { get; set; }
    public int forks { get; set; }
}
