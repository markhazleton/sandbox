// Working with JSON using the JsonDocument and JsonNode Classes

using System.Text.Json;
using System.Text.Json.Nodes;

Console.Clear();
Console.WriteLine("JsonNode");

// Load a couple of JSON texts
string filenameForecast = "forecast.json";
string filenameWind = "wind.json";

string jsonForecast = File.ReadAllText(filenameForecast);
string jsonWind = File.ReadAllText(filenameWind);

// 1. Create a JsonNode DOM from a JSON string and make a few changes
JsonNode? forecastNode = JsonNode.Parse(jsonForecast);

// Prepare options for pretty printing JSON
var options = new JsonSerializerOptions { WriteIndented = true };

// Print out the JSON object; notice there is no "wind" and description is there (I'll remove it later)
if (forecastNode != null)
{
    Console.WriteLine(forecastNode.ToJsonString(options));
}
Console.WriteLine();

// Cast as JsonObject to get the number of elements
Console.WriteLine(forecastNode.AsObject().Count());
Console.WriteLine();

// Get the list of all keys included in the object
List<string> keys = forecastNode.AsObject().Select(child => child.Key).ToList();
Console.WriteLine(string.Join(", ", keys));
Console.WriteLine();

// Get the value and type of one JsonNode
JsonNode tempCelsius = forecastNode["temperaturecelsius"];
Console.WriteLine($"Type={tempCelsius.GetType()}");
Console.WriteLine($"JSON={tempCelsius.ToJsonString()}");
Console.WriteLine();

// Get a JSON array
JsonNode keywords = forecastNode["keywords"];
Console.WriteLine($"Type={keywords.GetType()}");
Console.WriteLine($"JSON={keywords.ToJsonString()}");

Console.WriteLine($"Type={keywords[0].GetType()}");
Console.WriteLine($"JSON={keywords[0].ToJsonString()}");
Console.WriteLine();

// Properties can be chained, which is useful for extracting single values
decimal coord = (decimal)forecastNode["coord"]["lon"];
Console.WriteLine($"coord.lon={coord}");
Console.WriteLine();

// JsonNode DOM is mutable, new objects can be added and removed
// Remove description
string summary = forecastNode["description"].GetValue<string>();
forecastNode.AsObject().Remove("description");
if (forecastNode != null)
{
    Console.WriteLine(forecastNode.ToJsonString(options));
}
Console.WriteLine();

// Add summary and wind
forecastNode.AsObject().Add("summary", summary);

JsonNode? windNode = JsonNode.Parse(jsonWind);
forecastNode.AsObject().Add("wind", windNode);

if (forecastNode != null)
{
    Console.WriteLine(forecastNode.ToJsonString(options));
}
Console.WriteLine();

// 2. Create a new JsonObject using object initializers
var forecastObject = new JsonObject
{
    ["date"] = new DateTime(2019, 8, 1),
    ["temperaturecelsius"] = 7,
    ["feelslike"] = 3.14,
    ["description"] = "overcast clouds",
    ["pressure"] = 1003,
    ["humidity"] = 79,
    ["coord"] = new JsonObject()
    {
        //["lon"] = 48.75,
        ["lat"] = 8.24
    },
    ["wind"] = new JsonObject()
    {
        ["speed"] = 11.32,
        ["deg"] = 200,
        ["gust"] = 17.49
    },
    ["keywords"] = new JsonArray("Chill", "Windy"),
    ["country"] = "DE",
    ["city"] = "Baden-Baden"
};

// Print the JSON object
if (forecastObject != null)
{
    Console.WriteLine(forecastObject.ToJsonString(options));
}
Console.WriteLine();

// Add an object
forecastObject["coord"]["lon"] = 48.75;

// Remove a property
forecastObject.Remove("keywords");

// Change the value of a property
forecastObject["date"] = new DateTime(2022, 1, 1);

Console.WriteLine(forecastObject.ToJsonString(options));
Console.WriteLine();