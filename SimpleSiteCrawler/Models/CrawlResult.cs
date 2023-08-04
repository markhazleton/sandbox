namespace SimpleSiteCrawler.Models;

public class CrawlResult
{
    public string Url { get; }
    public int StatusCode { get; set; }
    public double ElapsedTime { get; set; }
    public DateTime CrawlDate { get; set; }
    public List<string> FoundLinks { get; }

    public CrawlResult(string url)
    {
        Url = url;
        FoundLinks = new List<string>();
    }
}
