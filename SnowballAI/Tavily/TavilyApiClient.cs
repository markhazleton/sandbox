using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json.Serialization;

namespace SnowballAI.Tavily;

public class TavilyApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly ILogger<TavilyApiClient> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public TavilyApiClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<TavilyApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("TavilyClient");
        _baseUrl = configuration["Tavily:BaseUrl"]?.TrimEnd('/') ?? "https://api.tavily.com";
        string apiKey = configuration["Tavily:ApiKey"] ?? throw new ArgumentNullException("Tavily API key is missing");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        _logger = logger;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };
    }

    private async Task<string> PostTavilyApiAsync(string endpoint, object payload)
    {
        string url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
        string jsonPayload = JsonSerializer.Serialize(payload, _serializerOptions);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        _logger.LogInformation("Sending request to {Url} with payload {Payload}", url, jsonPayload);

        using HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<TavilySearchResponse> SearchAsync(TavilySearchRequest request)
    {
        TavilySearchResponse response = null;
        try
        {
            string jsonResponse = await PostTavilyApiAsync("search", request);
            response = JsonSerializer.Deserialize<TavilySearchResponse>(jsonResponse, _serializerOptions) ?? throw new InvalidOperationException("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search Tavily API");
        }
        return response;
    }
}

public class TavilySearchRequest
{
    [JsonPropertyName("query")]
    public required string Query { get; set; }

    [JsonPropertyName("max_results")]
    public int MaxResults { get; set; } = 5;

    [JsonPropertyName("days")]
    public int? Days { get; set; }

    [JsonPropertyName("exclude_domains")]
    public List<string>? ExcludeDomains { get; set; }

    [JsonPropertyName("include_answer")]
    public string? IncludeAnswer { get; set; }

    [JsonPropertyName("include_domains")]
    public List<string>? IncludeDomains { get; set; }

    [JsonPropertyName("include_images")]
    public bool? IncludeImages { get; set; }

    [JsonPropertyName("include_raw_content")]
    public bool? IncludeRawContent { get; set; }

    [JsonPropertyName("search_depth")]
    public string? SearchDepth { get; set; }

    [JsonPropertyName("time_range")]
    public string? TimeRange { get; set; }

    [JsonPropertyName("topic")]
    public string? Topic { get; set; }
}

public class TavilySearchResponse
{
    [JsonPropertyName("answer")]
    public string? Answer { get; set; }

    [JsonPropertyName("query")]
    public string? Query { get; set; }

    [JsonPropertyName("response_time")]
    public double ResponseTime { get; set; }

    [JsonPropertyName("results")]
    public List<TavilySearchResult>? Results { get; set; }
}

public class TavilySearchResult
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("snippet")]
    public string? Snippet { get; set; }

    [JsonPropertyName("raw_content")]
    public string? RawContent { get; set; }
}
