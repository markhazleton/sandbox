// Serialize and deserialize JSON using System.Text.Json

using System.Text.Json;

Console.Clear();
Console.WriteLine("JsonSerializer Demo");

var ids = new Dictionary<string, string>
{
    { "a", "1" },
    { "b", "2" },
    { "c", "3" },
    { "d", "4" }
};


// Serialize to a JSON file
var weatherForecast = new WeatherForecast
{
    Date = DateTime.Parse("2019-08-01"),
    TemperatureCelsius = 25,
    Summary = "Hot",
    IDs = ids
};

Console.WriteLine("Serializing a JSON file Async with JsonSerializerOptions");
string jsonString = JsonSerializer.Serialize(weatherForecast);
Console.WriteLine(jsonString);

Console.WriteLine("Serializing a JSON file Async with JsonSerializerOptions");
string fileName = "WeatherForecast.json";
using FileStream createStream = File.Create(fileName);

JsonSerializerOptions options = new()
{
    WriteIndented = true
};
await JsonSerializer.SerializeAsync(createStream, weatherForecast, options);
await createStream.DisposeAsync();


// Review created file: "WeatherForecast.json"
Console.WriteLine("WeatherForecast.json has been created");
Console.WriteLine(File.ReadAllText(fileName));


// Deserialize from a JSON file
fileName = "WeatherForecast.json";
string jsonFile = File.ReadAllText(fileName);

Console.WriteLine("Deserializing from a JSON file");
WeatherForecast weather = JsonSerializer.Deserialize<WeatherForecast>(jsonFile)!;

Console.WriteLine($"Date: {weather.Date}");
Console.WriteLine($"TemperatureCelsius: {weather.TemperatureCelsius}");
Console.WriteLine($"Summary: {weather.Summary}");
Console.WriteLine($"IDs: {JsonSerializer.Serialize(weather.IDs)}");
Console.WriteLine();