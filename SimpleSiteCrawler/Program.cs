using Microsoft.Extensions.DependencyInjection;
using SimpleSiteCrawler.Helpers;
using SimpleSiteCrawler.Models;
using System.Collections;
using System.Text;

string domain = "https://travel.frogsfolly.com";
var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
var client = httpClientFactory.CreateClient();

Console.WriteLine($"Crawling: {domain}");

var crawler = new SiteCrawler(domain,client);

await crawler.Crawl(domain);

string domainName = SiteCrawlerHelpers.GetDomainName(domain);
string fileName = $"{domainName}_crawled_links.csv";

WriteToCsv<CrawlResult>(crawler.crawlResults, fileName);

static void WriteToCsv<T>(IEnumerable<T> data, string filePath)
{
    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
    {
        // Write CSV header
        var properties = typeof(T).GetProperties();
        writer.WriteLine(string.Join(",", properties.Select(p => p.Name)));

        // Write data rows
        foreach (var item in data)
        {
            var values = new List<string>();
            foreach (var property in properties)
            {
                if (property.PropertyType.IsGenericType &&
                    property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listValue = (ICollection)property.GetValue(item);
                    values.Add(listValue.Count.ToString());
                }
                else
                {
                    values.Add(property.GetValue(item)?.ToString() ?? string.Empty);
                }
            }
            writer.WriteLine(string.Join(",", values));
        }
    }
}
