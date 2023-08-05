using HtmlAgilityPack;

namespace SimpleSiteCrawler.Models;

public class CrawlResult
{
    public int Id { get; set; }
    public int Depth { get; set; }
    public string baseUrl { get; }
    public int StatusCode { get; set; }
    public double ElapsedTime { get; set; }
    public DateTime CrawlDate { get; set; }
    public List<string> ResponseLinks { get; } = new List<string>();
    public List<String> Errors { get; set; } = new List<string>();
    public string PageFound { get; set; }

    public CrawlResult(string url)
    {
        baseUrl = url;
    }

    private bool IsSameFullDomain(Uri uri)
    {
        string host = new Uri(this.baseUrl).Host;
        string targetHost = uri.Host;

        // Check if the target host matches the _domain host and the URL has a valid path
        return string.Equals(host, targetHost, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(uri.AbsolutePath)
            && uri.AbsolutePath != "/";
    }
    private static bool IsValidLink(string link)
    {
        // Check if the link either has no extension or has .html or .htm extension
        string extension = Path.GetExtension(link);
        if (string.IsNullOrEmpty(extension) || extension.Equals(".html", StringComparison.OrdinalIgnoreCase) || extension.Equals(".htm", StringComparison.OrdinalIgnoreCase))
        {
            // Exclude image, XML, and video links
            return !link.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".avi", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase)
                && !link.EndsWith(".mov", StringComparison.OrdinalIgnoreCase)
                && !link.Contains("/cdn-cgi/");
        }
        return false;
    }

    private string RemoveQueryAndOnPageLinks(string? link)
    {
        if (string.IsNullOrWhiteSpace(link)) return string.Empty;

        // Remove query parameters (if any)
        int queryIndex = link.IndexOf('?');
        if (queryIndex >= 0)
        {
            link = link[..queryIndex];
        }

        // Remove on-page links (if any)
        int hashIndex = link.IndexOf('#');
        if (hashIndex >= 0)
        {
            link = link[..hashIndex];
        }

        // Convert relative links to absolute links using the base domain
        if (!link.StartsWith("//") && Uri.TryCreate(link, UriKind.Relative, out var relativeUri))
        {
            Uri baseUri = new(this.baseUrl);
            Uri absoluteUri = new(baseUri, relativeUri);
            link = absoluteUri.ToString();
        }

        // Remove trailing '/' (if any)
        link = link.TrimEnd('/');

        return link.ToLower();
    }

    public void SetLinkList(string ResponseResults)
    {
        if (string.IsNullOrWhiteSpace(ResponseResults))
        {
            return;
        }
        try
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(ResponseResults);
            if (htmlDoc != null)
            {
                foreach (var link in htmlDoc.DocumentNode
                    .Descendants("a")
                    .Select(a => RemoveQueryAndOnPageLinks(a.GetAttributeValue("href", null)))
                    .Where(link => !string.IsNullOrWhiteSpace(link))
                    )
                {
                    if (ResponseLinks.Contains(link))
                    {
                        continue;
                    }
                    if (IsValidLink(link))
                    {
                        if (IsSameFullDomain(new Uri(link)))
                        {
                            ResponseLinks.Add(link);
                        }
                    }
                }
            }
        }
        catch
        {
            return;
        }
        return;
    }

}

