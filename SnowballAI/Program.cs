using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SnowballAI.Tavily;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddUserSecrets(typeof(Program).Assembly);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddHttpClient("TavilyClient");
        services.AddSingleton<TavilyApiClient>();
        services.AddLogging();
    })
    .Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var logger = services.GetRequiredService<ILogger<Program>>();
var tavilyClient = services.GetRequiredService<TavilyApiClient>();

string topic = "Riffusion AI and Its Impact on the Music Industry";
var searchRequest = new TavilySearchRequest { Query = topic, MaxResults = 5, Days = 7, IncludeAnswer = "basic" };

try
{
    var searchResults = await tavilyClient.SearchAsync(searchRequest);
    string searchJson = JsonSerializer.Serialize(searchResults, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine("Search Results:");
    Console.WriteLine(searchJson);
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while calling the Tavily API");
}
Console.ReadLine();