// Reading and writing in a performant way using Utf8JsonWriter and Utf8JsonReader
using System.Text;
using System.Text.Json;

Console.Clear();
Console.WriteLine("Utf8JsonWriter and Utf8JsonReader");

var optionsWriter = new JsonWriterOptions
{
    Indented = true
};

// 1. Utf8JsonWriter 
Console.WriteLine("Utf8JsonWriter");

using var stream = new MemoryStream();
using var writer = new Utf8JsonWriter(stream, optionsWriter);

writer.WriteStartObject();

// Property
writer.WritePropertyName("company");
writer.WriteStringValue("Carved Rock");

// Date
writer.WriteString("date", DateTimeOffset.UtcNow);
writer.WriteNumber("temperaturecelsius", 7);
writer.WriteNumber("feelslike", 3.14);
writer.WriteString("description", "overcast clouds");
writer.WriteNumber("pressure", 1003);
writer.WriteNumber("humidity", 79);

// Object
writer.WriteStartObject("coord");
writer.WriteNumber("lon", 48.75);
writer.WriteNumber("lon", 8.24);
writer.WriteEndObject();

writer.WriteStartObject("wind");
writer.WriteNumber("speed", 11.32);
writer.WriteNumber("deg", 200);
writer.WriteNumber("gust", 17.49);
writer.WriteEndObject();

writer.WriteStartArray("keywords");
writer.WriteStringValue("Chill");
writer.WriteStringValue("Windy");
writer.WriteEndArray();

writer.WriteString("country", "DE");
writer.WriteString("city", "Baden-Baden");

// Nulls
writer.WriteNull("tsunami");

writer.WriteStartArray("tsunamiLocations");
writer.WriteNullValue();
writer.WriteEndArray();
writer.WriteEndObject();
writer.Flush();

string json = Encoding.UTF8.GetString(stream.ToArray());

string outputFileName = "output.json";
File.WriteAllText(outputFileName, json);

// Created output.json 
Console.WriteLine("Created file output.json");


// 2. Utf8JsonReader
Console.WriteLine("Utf8JsonReader");

// The BOM (byte order mark) which is used to determine if it is UTF8
byte[] Utf8Bom = new byte[] { 0xEF, 0xBB, 0xBF };

// ReadAllBytes 
string fileName = "output.json";
ReadOnlySpan<byte> jsonReadOnlySpan = File.ReadAllBytes(fileName);

// Read past the UTF-8 BOM bytes if a BOM exists
if (jsonReadOnlySpan.StartsWith(Utf8Bom))
{
    jsonReadOnlySpan = jsonReadOnlySpan.Slice(Utf8Bom.Length);
}

string text;

var reader = new Utf8JsonReader(jsonReadOnlySpan);

while (reader.Read())
{
    JsonTokenType tokenType = reader.TokenType;
    Console.WriteLine(tokenType.ToString());

    switch (tokenType)
    {
        case JsonTokenType.StartObject:
            break;
        case JsonTokenType.PropertyName:
            text = reader.GetString();
            Console.WriteLine(" " + text);
            break;
        case JsonTokenType.String:
        {
            text = reader.GetString();
            Console.WriteLine(" " + text);
            break;
        }
        case JsonTokenType.Number:
        {
            double DoubleValue = reader.GetDouble();
            Console.WriteLine(" " + DoubleValue);
            break;
        }
    }
}

Console.WriteLine();
