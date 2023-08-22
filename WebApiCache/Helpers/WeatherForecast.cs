namespace WebApiCache.Helpers;

public record WeatherForecast(string City, DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public static List<Tuple<int, int, string>> TemperatureSummaries = new()
    {
        Tuple.Create(int.MinValue, -10, "Freezing"),
        Tuple.Create(-10, 0, "Bracing"),
        Tuple.Create(0, 10, "Chilly"),
        Tuple.Create(10, 20, "Cool"),
        Tuple.Create(20, 30, "Mild"),
        Tuple.Create(30, 40, "Warm"),
        Tuple.Create(40, 50, "Balmy"),
        Tuple.Create(50, int.MaxValue, "Hot"),
    };

    public static Task<List<WeatherForecast>> GetWeatherForecastListAsync(string city)
    {
        var random = new Random();
        string GetTemperatureSummary(int temperature)
        {
            var summaryTuple = TemperatureSummaries.FirstOrDefault(tuple => temperature >= tuple.Item1 && temperature < tuple.Item2);
            return summaryTuple?.Item3 ?? "Unknown";
        }
        (int TempF, string Summary) GetRandomWeather()
        {
            int temperature = random.Next(-20, 55);
            string summary = GetTemperatureSummary(temperature);
            return (temperature, summary);
        }
        return Task.FromResult(Enumerable.Range(1, 5).Select(index =>
        {
            var (temperature, summary) = GetRandomWeather();
            return new WeatherForecast
            (
                city,
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                temperature,
                summary
            );
        })
        .ToList());
    }
}
