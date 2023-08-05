using SimpleSiteCrawler.Models;
using System.Diagnostics;

namespace SimpleSiteCrawler.Helpers;

public partial class SiteCrawler
{
    public readonly List<CrawlResult> crawlResults;
    private readonly HttpClient httpClient;

    public SiteCrawler(string domain, HttpClient httpClient)
    {
        this.httpClient = httpClient;
        crawlResults = new List<CrawlResult>();
    }

    private async Task<CrawlResult> CrawlPage(string url, int Id, int Depth, string fromUrl, CancellationToken ct = default)
    {
        var crawlResult = new CrawlResult(url)
        {
            Id = Id,
            Depth = Depth,
            PageFound = fromUrl
            
        };
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var response = await httpClient.GetAsync(url, ct);
            crawlResult.StatusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                crawlResult.SetLinkList(await response.Content.ReadAsStringAsync(ct));

            }
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP errors
            crawlResult.StatusCode = (int)ex.StatusCode;
            crawlResult.Errors.Add(ex.Message);
            Console.WriteLine("Error accessing page: " + url);
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            crawlResult.StatusCode = -1; // An error occurred
            crawlResult.Errors.Add(ex.Message);
            Console.WriteLine("Error accessing page: " + url);
            Console.WriteLine(ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            crawlResult.ElapsedTime = stopwatch.ElapsedMilliseconds;
            crawlResult.CrawlDate = DateTime.Now;
        }
        Console.WriteLine($"{crawlResult.Id:D4}:{crawlResult.Depth:D4}:{crawlResult.StatusCode} --> {GetPathFromUrl(crawlResult.baseUrl)} found at {GetPathFromUrl(crawlResult.PageFound)}");

        return crawlResult;
    }
    public static string GetPathFromUrl(string fullUrl)
    {
        if (string.IsNullOrEmpty(fullUrl)) return string.Empty;

        try
        {
            Uri uri = new Uri(fullUrl);
            return uri.AbsolutePath;
        }
        catch (UriFormatException)
        {
            throw new ArgumentException("Invalid URL format");
        }
    }

    public async Task Crawl(string link, CancellationToken ct = default)
    {
        int CrawlDepth = 1;
        int CurrentID = 1;
        var pageResult = await CrawlPage(link, CurrentID, CrawlDepth, link, ct);

        crawlResults.Add(pageResult);

        await CrawlSubPagesBFS(pageResult, ct);

    }
    private async Task CrawlSubPagesBFS(CrawlResult pageResult, CancellationToken ct)
    {
        Queue<CrawlResult> queue = new Queue<CrawlResult>();
        queue.Enqueue(pageResult);

        while (queue.Count > 0)
        {
            var currentResult = queue.Dequeue();

            if (currentResult.Depth > 3) continue; // Max depth

            foreach (var foundLink in currentResult.ResponseLinks)
            {
                if (ct.IsCancellationRequested) break;

                if (crawlResults.Any(x => x.baseUrl == foundLink)) continue;

                int nextID = currentResult.Id + 1;
                var subPageResult = await CrawlPage(foundLink, nextID, currentResult.Depth + 1, currentResult.baseUrl, ct);
                subPageResult.PageFound = currentResult.baseUrl;
                crawlResults.Add(subPageResult);

                queue.Enqueue(subPageResult);
            }
        }
    }

    private async Task CrawlSubPage(CrawlResult pageResult, int CurrentID, CancellationToken ct)
    {
        int depth = pageResult.Depth + 1;
        
        if(depth > 200) return; // Max depth

        foreach (var foundLink in pageResult.ResponseLinks)
        {
            if (ct.IsCancellationRequested) break;

            if (crawlResults.Any(x => x.baseUrl == foundLink)) continue;

            CurrentID++;
            var subPageResult = await CrawlPage(foundLink, CurrentID, depth, pageResult.baseUrl, ct);
            subPageResult.PageFound = pageResult.baseUrl;
            crawlResults.Add(subPageResult);
            await CrawlSubPage(subPageResult, CurrentID, ct);
        }
    }

    public async Task ExportToCSV(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            await writer.WriteLineAsync("URL,Status Code,Elapsed Time,Crawl Date,Found Links,Id,Depth,PageFound");

            foreach (var result in crawlResults)
            {
                var line = $"{result.baseUrl},{result.StatusCode},{result.ElapsedTime},{result.CrawlDate},{result.ResponseLinks.Count},{result.Id},{result.Depth},{result.PageFound}";
                await writer.WriteLineAsync(line);
            }
        }
        Console.WriteLine($"Crawl results with {crawlResults.Count} pages exported to: {filePath}");
    }
}