using HtmlAgilityPack;
using SimpleSiteCrawler.Models;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace SimpleSiteCrawler.Helpers;

public partial class SiteCrawler
{
    private readonly List<CrawlResult> crawlResults;
    private readonly string domain;
    private readonly HttpClient httpClient;
    private readonly Queue<string> linksToParse;
    private readonly HashSet<string> visitedLinks;

    public SiteCrawler(string domain, HttpClient httpClient)
    {
        this.domain = domain;
        this.httpClient = httpClient;
        visitedLinks = new HashSet<string>();
        crawlResults = new List<CrawlResult>();
        linksToParse = new Queue<string>();
    }

    private async Task CrawlPage(string url, int depth, CancellationToken ct = default)
    {
        if (!visitedLinks.Contains(url) && depth >= 0)
        {
            visitedLinks.Add(url);
            var crawlResult = new CrawlResult(url);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                var response = await httpClient.GetAsync(url, ct);
                crawlResult.StatusCode = (int)response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    string htmlContent = await response.Content.ReadAsStringAsync(ct);

                    // Use a proper HTML parser like AngleSharp or HtmlAgilityPack
                    foreach (string link in ParseLinks(htmlContent))
                    {
                        if (!visitedLinks.Contains(link))
                        {
                            crawlResult.FoundLinks.Add(link);

                            // Use a priority queue (e.g., MinHeap) instead of a regular queue
                            linksToParse.Enqueue(link);
                        }
                    }

                    // Implement depth limitation
                    foreach (var link in crawlResult.FoundLinks)
                    {
                        await CrawlPage(link, depth - 1, ct);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP errors
                crawlResult.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                Console.WriteLine("Error accessing page: " + url);
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                crawlResult.StatusCode = -1; // An error occurred
                Console.WriteLine("Error accessing page: " + url);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                stopwatch.Stop();
                crawlResult.ElapsedTime = stopwatch.ElapsedMilliseconds;
                crawlResult.CrawlDate = DateTime.Now;
                crawlResults.Add(crawlResult);
            }
        }
    }

    [GeneratedRegex("<a\\s+(?:[^>]*?\\s+)?href=\"(.*?)\"", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex FindLinksInHtml();
    private string GetAbsoluteUrl(string link)
    {
        if (Uri.TryCreate(new Uri(domain), link, out var absoluteUri))
        {
            return absoluteUri.AbsoluteUri;
        }
        return null;
    }

    private bool IsSameDomain(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false; // Invalid URL, not the same domain
        }
        string host = new Uri(domain).Host;
        string targetHost = uri.Host;

        // Check if the target host matches the domain host and the URL has a valid path
        return string.Equals(host, targetHost, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(uri.AbsolutePath)
            && uri.AbsolutePath != "/";
    }
    private List<string> ParseLinks(string html)
    {
        var result = new List<string>();

        // Use HtmlAgilityPack to parse the HTML and extract anchor tags with href attributes
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var links = doc.DocumentNode.Descendants("a")
            .Select(a => a.GetAttributeValue("href", null))
            .Where(link => !string.IsNullOrWhiteSpace(link));

        foreach (string link in links)
        {
            string absoluteUrl = GetAbsoluteUrl(link);
            if (IsSameDomain(absoluteUrl) && !visitedLinks.Contains(absoluteUrl) && !linksToParse.Contains(absoluteUrl)
                && !absoluteUrl.StartsWith("/cdn-cgi/"))
            {
                result.Add(absoluteUrl);
            }
        }

        return result;
    }

    public async Task Crawl(CancellationToken ct = default)
    {
        linksToParse.Enqueue(domain); // Enqueue the starting domain
        while (linksToParse.Count > 0)
        {
            string link = linksToParse.Dequeue();
            await CrawlPage(link, 100, ct);
        }
    }

    public async Task ExportToCSV(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            await writer.WriteLineAsync("URL,Status Code,Elapsed Time,Crawl Date,Found Links");

            foreach (var result in crawlResults)
            {
                var line = $"{result.Url},{result.StatusCode},{result.ElapsedTime},{result.CrawlDate},{result.FoundLinks.Count}";
                await writer.WriteLineAsync(line);
            }
        }
        Console.WriteLine($"Crawl results with {crawlResults.Count} pages exported to: {filePath}");
    }
}