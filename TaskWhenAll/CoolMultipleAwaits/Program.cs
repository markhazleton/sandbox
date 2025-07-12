using CoolMultipleAwaits;
using Microsoft.Extensions.Logging;
using static CoolMultipleAwaits.WeatherService;

Console.WriteLine(TimeProvider.System.GetLocalNow());
var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

var weatherService = new WeatherService();
var weatherCities = new TaskResultCollection();

var taskList = new List<Task>
{
    weatherCities.GetTaskResult("London", weatherService.GetWeather("London")),
    weatherCities.GetTaskResult("Paris", weatherService.GetWeather("Paris")),
    weatherCities.GetTaskResult("Rome", weatherService.GetWeather("Rome")),
    weatherCities.GetTaskResult("New York", weatherService.GetWeather("New York")),
    weatherCities.GetTaskResult("Chicago", weatherService.GetWeather("Chicago")),
    weatherCities.GetTaskResult("Dallas", weatherService.GetWeather("Dallas")),
    weatherCities.GetTaskResult("Houston", weatherService.GetWeather("Houston"))
};
await TaskResultCollection.WhenAllWithLoggingAsync<Task>(taskList,logger);


Console.WriteLine("All tasks completed\n\n");
Console.WriteLine("Telemetry:");
foreach (var cityTelemetry in weatherCities.Telemetry)
{
    Console.WriteLine(cityTelemetry);
}

Console.WriteLine("\n\nResults:");
foreach (var city in weatherCities.Sections)
{
    Console.WriteLine($"{city.Name}:");
    if (city.Data is not null)
    {
        foreach (var forecast in city.Data as IEnumerable<WeatherForecast>)
        {
            Console.WriteLine(forecast);
        }
    }
    else
    {
        Console.WriteLine("No data");
    }
    Console.WriteLine();
}

Console.WriteLine(TimeProvider.System.GetLocalNow());
Console.WriteLine();
