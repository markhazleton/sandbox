using Microsoft.Extensions.DependencyInjection;
using SimpleSiteCrawler.Helpers;

string domain = "https://travel.frogsfolly.com";
var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
var client = httpClientFactory.CreateClient();
var crawler = new SiteCrawler(domain,client);
await crawler.Crawl();
string domainName = SiteCrawlerHelpers.GetDomainName(domain);
string fileName = $"{domainName}_crawled_links.csv";
await crawler.ExportToCSV(fileName);
