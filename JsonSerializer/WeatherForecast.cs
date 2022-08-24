public class WeatherForecast
{
    public DateTimeOffset Date { get; set; }
    public int TemperatureCelsius { get; set; }
    public string? Summary { get; set; }
    public Dictionary<string, string> IDs { get; set; } = new Dictionary<string, string>();

}