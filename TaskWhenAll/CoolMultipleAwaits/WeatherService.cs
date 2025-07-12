namespace CoolMultipleAwaits;


public class WeatherService
{
    private readonly (string Summary, int MaxCTemp)[] Summaries = {
        ("Freezing", -20), ("Bracing", -10), ("Chilly", 0), ("Cool", 10), ("Mild", 20), ("Warm", 30), ("Balmy", 35), ("Hot", 40), ("Sweltering", 50), ("Scorching", 55)
    };

    public async Task<IEnumerable<WeatherForecast>> GetWeather(string city)
    {
        await Task.Delay(Random.Shared.Next(500,3000));

        // Introduce randomness for failure
        if (Random.Shared.NextDouble() < 0.3) // Adjust the probability as needed
        {
            throw new Exception("Random failure occurred.");
        }

        var weather = Enumerable.Range(1, 5).Select(index =>
        {
            var temperatureC = Random.Shared.Next(-20, 55);
            return new WeatherForecast
            {
                City = city,
                Date = DateTime.Now.AddDays(index),
                TemperatureC = temperatureC,
                Summary = Summaries.FirstOrDefault(s => s.MaxCTemp >= temperatureC).Summary
            };
        })
        .ToArray();
        return weather;
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public string City { get; set; } = string.Empty;
        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string? Summary { get; set; }

        public override string ToString()
        {
            return $"City:{City} Date: {Date}, TemperatureF: {TemperatureF}, Summary: {Summary}";
        }
    }
}
