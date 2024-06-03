namespace HongJun.Service.Dto;


public class BingSearchResult
{
    public string _type { get; set; }
    public QueryContext queryContext { get; set; }
    public WebPages webPages { get; set; }
    public Images images { get; set; }
    public News news { get; set; }
    public Videos videos { get; set; }
    public RankingResponse rankingResponse { get; set; }
}

public class QueryContext
{
    public string originalQuery { get; set; }
}

public class WebPages
{
    public string webSearchUrl { get; set; }
    public int totalEstimatedMatches { get; set; }
    public Value[] value { get; set; }
}

public class Value
{
    public string id { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string datePublished { get; set; }
    public string datePublishedDisplayText { get; set; }
    public bool isFamilyFriendly { get; set; }
    public string displayUrl { get; set; }
    public string snippet { get; set; }
    public string dateLastCrawled { get; set; }
    public string cachedPageUrl { get; set; }
    public string language { get; set; }
    public bool isNavigational { get; set; }
    public bool noCache { get; set; }
    public string datePublishedFreshnessText { get; set; }
    public string thumbnailUrl { get; set; }
    public PrimaryImageOfPage primaryImageOfPage { get; set; }
}

public class PrimaryImageOfPage
{
    public string thumbnailUrl { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public string imageId { get; set; }
}

public class Images
{
    public string id { get; set; }
    public string readLink { get; set; }
    public string webSearchUrl { get; set; }
    public bool isFamilyFriendly { get; set; }
    public Value1[] value { get; set; }
}

public class Value1
{
    public string webSearchUrl { get; set; }
    public string name { get; set; }
    public string thumbnailUrl { get; set; }
    public string datePublished { get; set; }
    public string contentUrl { get; set; }
    public string hostPageUrl { get; set; }
    public string contentSize { get; set; }
    public string encodingFormat { get; set; }
    public string hostPageDisplayUrl { get; set; }
    public int width { get; set; }
    public int height { get; set; }
    public Thumbnail thumbnail { get; set; }
}

public class Thumbnail
{
    public int width { get; set; }
    public int height { get; set; }
}

public class News
{
    public string id { get; set; }
    public string readLink { get; set; }
    public Value2[] value { get; set; }
}

public class Value2
{
    public ContractualRules[] contractualRules { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string description { get; set; }
    public Provider[] provider { get; set; }
    public string datePublished { get; set; }
    public string category { get; set; }
    public Image image { get; set; }
    public About[] about { get; set; }
}

public class ContractualRules
{
    public string _type { get; set; }
    public string text { get; set; }
}

public class Provider
{
    public string _type { get; set; }
    public string name { get; set; }
    public Image1 image { get; set; }
}

public class Image1
{
    public Thumbnail1 thumbnail { get; set; }
}

public class Thumbnail1
{
    public string contentUrl { get; set; }
}

public class Image
{
    public string contentUrl { get; set; }
    public Thumbnail2 thumbnail { get; set; }
}

public class Thumbnail2
{
    public string contentUrl { get; set; }
    public int width { get; set; }
    public int height { get; set; }
}

public class About
{
    public string readLink { get; set; }
    public string name { get; set; }
}

public class Videos
{
    public string id { get; set; }
    public string readLink { get; set; }
    public string webSearchUrl { get; set; }
    public bool isFamilyFriendly { get; set; }
    public string scenario { get; set; }
}

public class Publisher
{
    public string name { get; set; }
}

public class Creator
{
    public string name { get; set; }
}

public class Thumbnail3
{
    public int width { get; set; }
    public int height { get; set; }
}

public class RankingResponse
{
    public Mainline mainline { get; set; }
    public Sidebar sidebar { get; set; }
}

public class Mainline
{
    public Items[] items { get; set; }
}

public class Items
{
    public string answerType { get; set; }
    public Value4 value { get; set; }
    public int resultIndex { get; set; }
}

public class Value4
{
    public string id { get; set; }
}

public class Sidebar
{
    public Items1[] items { get; set; }
}

public class Items1
{
    public string answerType { get; set; }
    public Value5 value { get; set; }
}

public class Value5
{
    public string id { get; set; }
}