using GarageSpark.Models;
using WebSpark.HttpClientUtility.RequestResult;

namespace GarageSpark.Services;

public class ApiService
{
    private readonly IHttpRequestResultService _requestService;
    private readonly ILogger<ApiService> _logger;

    public ApiService(IHttpRequestResultService requestService, ILogger<ApiService> logger)
    {
        _requestService = requestService;
        _logger = logger;
    }

    public async Task<WeatherData?> GetWeatherAsync(string city)
    {
        try
        {
            // Example using a free weather API (OpenWeatherMap or similar)
            var request = new HttpRequestResult<WeatherData>
            {
                RequestPath = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid=demo&units=metric",
                RequestMethod = HttpMethod.Get,
                CacheDurationMinutes = 5, // Cache for 5 minutes
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "GarageSpark/1.0" }
                }
            };

            _logger.LogInformation("Attempting to get weather data for city: {City}", city);

            var result = await _requestService.HttpSendRequestResultAsync(request);

            if (result.IsSuccessStatusCode && result.ResponseResults != null)
            {
                _logger.LogInformation("Successfully retrieved weather data for city: {City}. CorrelationId: {CorrelationId}, Duration: {DurationMs}ms",
                    city, result.CorrelationId, result.RequestDurationMilliseconds);
                return result.ResponseResults;
            }
            else
            {
                _logger.LogError("Failed to retrieve weather data for city: {City}. Status: {StatusCode}, Errors: [{Errors}], CorrelationId: {CorrelationId}, Duration: {DurationMs}ms",
                    city, result.StatusCode, string.Join(", ", result.ErrorList), result.CorrelationId, result.RequestDurationMilliseconds);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting weather data for city: {City}", city);
            return null;
        }
    }

    public async Task<List<PostData>?> GetPostsAsync(int limit = 10)
    {
        try
        {
            // Example using JSONPlaceholder API
            var request = new HttpRequestResult<List<PostData>>
            {
                RequestPath = $"https://jsonplaceholder.typicode.com/posts?_limit={limit}",
                RequestMethod = HttpMethod.Get,
                CacheDurationMinutes = 2, // Cache for 2 minutes
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "GarageSpark/1.0" }
                }
            };

            _logger.LogInformation("Attempting to get {Limit} posts", limit);

            var result = await _requestService.HttpSendRequestResultAsync(request);

            if (result.IsSuccessStatusCode && result.ResponseResults != null)
            {
                _logger.LogInformation("Successfully retrieved {Count} posts. CorrelationId: {CorrelationId}, Duration: {DurationMs}ms",
                    result.ResponseResults.Count, result.CorrelationId, result.RequestDurationMilliseconds);
                return result.ResponseResults;
            }
            else
            {
                _logger.LogError("Failed to retrieve posts. Status: {StatusCode}, Errors: [{Errors}], CorrelationId: {CorrelationId}, Duration: {DurationMs}ms",
                    result.StatusCode, string.Join(", ", result.ErrorList), result.CorrelationId, result.RequestDurationMilliseconds);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while getting posts");
            return null;
        }
    }

    public async Task<bool> CreatePostAsync(PostData post)
    {
        try
        {
            var request = new HttpRequestResult<PostData>
            {
                RequestPath = "https://jsonplaceholder.typicode.com/posts",
                RequestMethod = HttpMethod.Post,
                // RequestContent = post,  // Will fix this property name
                RequestHeaders = new Dictionary<string, string>
                {
                    { "User-Agent", "GarageSpark/1.0" },
                    { "Content-type", "application/json" }
                }
            };

            _logger.LogInformation("Attempting to create post: {@Post}", post);

            var result = await _requestService.HttpSendRequestResultAsync(request);

            if (result.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully created post. Response: {@Response}, CorrelationId: {CorrelationId}, Duration: {DurationMs}ms",
                    result.ResponseResults, result.CorrelationId, result.RequestDurationMilliseconds);
                return true;
            }
            else
            {
                _logger.LogError("Failed to create post. Status: {StatusCode}, Errors: [{Errors}], CorrelationId: {CorrelationId}, Duration: {DurationMs}ms",
                    result.StatusCode, string.Join(", ", result.ErrorList), result.CorrelationId, result.RequestDurationMilliseconds);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating post: {@Post}", post);
            return false;
        }
    }
}
