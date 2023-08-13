using WebApiCache.SystemCache;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();
app.UseSwagger();
app.UseHttpsRedirection();

app.MapGet("/weatherforecast/", () =>
{
    return SystemValuesCache.GetCachedData<WeatherForecast>(
        "weatherforecast",
         async () => 
        {
            return await WeatherForecast.GetWeatherForecastListAsync();
        }, 
        10);
});
app.UseSwaggerUI();
app.Run();
