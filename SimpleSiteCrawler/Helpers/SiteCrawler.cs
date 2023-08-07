using SimpleSiteCrawler.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SimpleSiteCrawler.Helpers;

public class SiteCrawler 
{
    private static readonly HashSet<string> crawledUrls = new();

    private static readonly ConcurrentQueue<CrawlResult> crawlList = new();
    private static readonly object lockObj = new();
    private static readonly ConcurrentDictionary<string, CrawlResult> resultsDict = new();

    private readonly IHttpClientFactory httpClientFactory;
    private int maxConcurrency = 10;

    public SiteCrawler(IHttpClientFactory factory, int semaphoreMax)
    {
        httpClientFactory = factory;
        this.maxConcurrency = semaphoreMax;
    }

    private static bool AddCrawlResult(CrawlResult? result)
    {
        if (result is null)
        {
            return false;
        }
        lock (lockObj)
        {
            if (resultsDict.ContainsKey(result.baseUrl))
            {
                return false;
            }
            resultsDict[result.baseUrl] = result;

            foreach (var foundUrl in result.ResponseLinks)
            {
                if (crawledUrls.Contains(foundUrl))
                {
                    continue;
                }
                if (resultsDict.ContainsKey(foundUrl))
                {
                    continue;
                }

                if (crawlList.Any(w => w.baseUrl == foundUrl))
                {
                    continue;
                }

                var newCrawl = new CrawlResult(foundUrl)
                {
                    Depth = result.Depth + 1,
                    PageFound = result.baseUrl
                };
                crawlList.Enqueue(newCrawl);
            }
            Console.WriteLine($"C:ID:{result.Id} C:{resultsDict.Count:D5} Q:{crawlList.Count:D5} W:{result.SemaphoreWaitTimeMS:D5} T:{result.ElapsedTime:0,000} +++ Added Result: {result.baseUrl}");
            return true;
        }
    }
    static async Task<long> AwaitSemaphoreAsync(SemaphoreSlim semaphore, CancellationToken ct = default)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        await semaphore.WaitAsync(ct);
        stopwatch.Stop();
        return stopwatch.ElapsedTicks;
    }

    private async Task CrawlPage(CrawlResult? crawlResult, SemaphoreSlim semaphore, CancellationToken ct = default)
    {
        if (crawlResult is null)
        {
            return;
        }
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            var response = await httpClientFactory.CreateClient("SiteCrawler").GetAsync(crawlResult.baseUrl, ct);
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
            crawlResult.Errors.Add("Error accessing page: " + crawlResult.baseUrl);
            Console.WriteLine(ex.Message);
        }
        catch (Exception ex)
        {
            crawlResult.StatusCode = -1; // An error occurred
            crawlResult.Errors.Add(ex.Message);
            crawlResult.Errors.Add("Error accessing page: " + crawlResult.baseUrl);
            Console.WriteLine(ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            crawlResult.ElapsedTime = stopwatch.ElapsedMilliseconds;
            crawlResult.CrawlDate = DateTime.Now;
            if (!AddCrawlResult(crawlResult))
            {
                Console.WriteLine($"FAILED TO ADD: {crawlResult.Id:D4}:{crawlResult.Depth:D4}:{crawlResult.StatusCode} --> {GetPathFromUrl(crawlResult.baseUrl)} FAILED TO ADD");
            }
            semaphore.Release();
        }

        return;
    }

    private async Task CrawlSubPagesBFS(SemaphoreSlim semaphore, CancellationToken ct)
    {
        List<Task> tasks = new();
        try
        {
            while (true)
            {

                long SemaphoreWaitTimeMS = await AwaitSemaphoreAsync(semaphore, ct);
                CrawlResult? crawlResult = GetNextCrawl();
                if (crawlResult is null)
                    break;

                crawledUrls.Add(crawlResult.baseUrl);
                crawlResult.SemaphoreWaitTimeMS = SemaphoreWaitTimeMS;

                tasks.Add(CrawlPage(crawlResult, semaphore, ct));

                if (tasks.Count >= maxConcurrency)
                {
                    Task finishedTask = await Task.WhenAny(tasks);
                    tasks.Remove(finishedTask);
                }

            }
            await Task.WhenAll(tasks);
        }
        finally
        {
        }
    }

    private CrawlResult? GetCrawlFromUrl(string url)
    {
        if (crawledUrls.Contains(url)) return null;
        crawledUrls.Add(url);
        resultsDict.TryGetValue(url, out var result);
        return result;
    }
    private static CrawlResult? GetNextCrawl()
    {
        CrawlResult? crawlNext;
        lock (lockObj)
        {
            int loopCount = 0;
            while (crawlList.TryDequeue(out crawlNext))
            {
                if (!crawledUrls.Contains(crawlNext.baseUrl))
                {
                    if (!resultsDict.ContainsKey(crawlNext.baseUrl))
                    {
                        crawlNext.Id = crawledUrls.Count + 1;
                        return crawlNext;
                    }
                }
                loopCount++;
            }
            return null;
        }
    }
    private static string GetPathFromUrl(string? fullUrl)
    {
        if (string.IsNullOrEmpty(fullUrl)) return string.Empty;

        try
        {
            Uri uri = new(fullUrl);
            return uri.AbsolutePath;
        }
        catch (UriFormatException)
        {
            throw new ArgumentException("Invalid URL format");
        }
    }

    public async Task<ICollection<CrawlResult>> Crawl(string link, CancellationToken ct = default)
    {
        var crawlResult = new CrawlResult(link)
        {
            Id = 1,
            Depth = 1,
            PageFound = link
        };
        SemaphoreSlim semaphore = new(maxConcurrency);

        crawlResult.SemaphoreWaitTimeMS = await AwaitSemaphoreAsync(semaphore);
        await CrawlPage(crawlResult, semaphore, ct);

        await CrawlSubPagesBFS(semaphore, ct);

        return resultsDict.Values;
    }
    public void DisplayUrl(string url, int level)
    {
        string indentation = new(' ', level * 4);
        var node = GetCrawlFromUrl(url);

        Console.WriteLine($"{indentation}{GetPathFromUrl(node?.baseUrl)}");

        foreach (var child in node?.ResponseLinks ?? new List<string>())
        {
            if (crawledUrls.Contains(child)) continue;

            DisplayUrl(child, level + 1);
        }
    }

    public void DisplayUrlTree(string url, int level)
    {
        crawledUrls.Clear();
        string indentation = new(' ', level * 4);
        var node = GetCrawlFromUrl(url);

        Console.WriteLine($"{indentation}{node?.baseUrl}");

        foreach (var child in node?.ResponseLinks ?? new List<string>())
        {
            if (crawledUrls.Contains(child)) continue;

            DisplayUrl(child, level + 1);
        }
    }
}