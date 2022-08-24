// Working with JSON using the JsonDocument and JsonNode Classes
using System.Text.Json;

Console.Clear();
Console.WriteLine("JsonDocument");

string filename = "weather.json";

// Load the JSON text
string jsonString = File.ReadAllText(filename);

double sum = 0;
int count = 0;
TempDataPoint? perfectTemperature;

// Notice the 'using'. This class utilizes resources from pooled memory
// Failure to properly dispose this object will result in the memory not being returned to the pool,
// which will increase GC impact across various parts of the framework
using (JsonDocument document = JsonDocument.Parse(jsonString))
{
    JsonElement root = document.RootElement;

    // Get the entire JSON from the root JsonElement
    Console.WriteLine(root.ToString());
    Console.WriteLine();

    // Get one property (make sure it does exist) 
    JsonElement temp = root.GetProperty("temperaturecelsius");
    Console.WriteLine(temp.ToString());
    Console.WriteLine();

    // Get a property, which in this particular case is an array 
    JsonElement last7daysElement = root.GetProperty("last7days");
    Console.WriteLine(last7daysElement.ToString());
    Console.WriteLine();

    // Iterate over the JsonElement objects it contains
    foreach (JsonElement measure in last7daysElement.EnumerateArray())
    {
        if (measure.TryGetProperty("temperature", out JsonElement tempElement))
        {
            sum += tempElement.GetDouble();
            count++;
            if (tempElement.GetInt32() == 7)
            {
                // Use a method from JsonSerializer to deserialize a JsonElement
                perfectTemperature = JsonSerializer.Deserialize<TempDataPoint>(measure);
                // Instead of doing it manually, as shown below
                // perfectTemperature = new TempDataPoint{ 
                //   humidity = measure.GetProperty("humidity").GetInt32(),
                //   temperature = measure.GetProperty("temperature").GetInt32()               
                //};
            }
        }
    }
}

double average = sum / count;
Console.WriteLine($"Average temperature : {average}");
Console.WriteLine();