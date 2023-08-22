using WebApiCache.Helpers;
using WebApiCache.SystemCache;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache();

var app = builder.Build();
app.UseSwagger();
app.UseHttpsRedirection();

app.UseOutputCache();

app.MapGet("/weatherforecast/{city}", (string? city) =>
{
    if (string.IsNullOrEmpty(city))
    {
        city = "Dallas";
    }
    return SystemValuesCache.GetCachedData(
        $"weatherforecast-{city}",
         async () =>
        {
            return await WeatherForecast.GetWeatherForecastListAsync(city);
        },
        10);

}).CacheOutput(x => x.Expire(TimeSpan.FromMinutes(5)))
.WithName("GetWeatherForecast").WithOpenApi();

app.UseSwaggerUI();
app.Run();
