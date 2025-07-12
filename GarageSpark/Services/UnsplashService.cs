using GarageSpark.ViewModels;
using System.Text.Json;

namespace GarageSpark.Services
{
    /// <summary>
    /// Service interface for Unsplash API integration
    /// </summary>
    public interface IUnsplashService
    {
        Task<UnsplashSearchResponseViewModel> SearchPhotosAsync(string query, int page = 1, int perPage = 30);
        Task<UnsplashSearchResultViewModel?> GetPhotoAsync(string id);
        Task TriggerDownloadAsync(string id);
    }

    /// <summary>
    /// Service for integrating with Unsplash API
    /// </summary>
    public class UnsplashService : IUnsplashService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UnsplashService> _logger;
        private readonly string? _accessKey;

        public UnsplashService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<UnsplashService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _accessKey = _configuration["Unsplash:AccessKey"];

            // Configure HttpClient
            _httpClient.BaseAddress = new Uri("https://api.unsplash.com/");
            _httpClient.DefaultRequestHeaders.Add("Accept-Version", "v1");

            if (!string.IsNullOrEmpty(_accessKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_accessKey}");
            }
        }

        public async Task<UnsplashSearchResponseViewModel> SearchPhotosAsync(string query, int page = 1, int perPage = 30)
        {
            try
            {
                if (string.IsNullOrEmpty(_accessKey))
                {
                    _logger.LogWarning("Unsplash API access key not configured");
                    return new UnsplashSearchResponseViewModel();
                }

                _logger.LogInformation("Searching Unsplash for: {Query}, Page: {Page}", query, page);

                var response = await _httpClient.GetAsync($"search/photos?query={Uri.EscapeDataString(query)}&page={page}&per_page={perPage}&order_by=relevant");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Unsplash search failed with status: {StatusCode}", response.StatusCode);
                    return new UnsplashSearchResponseViewModel();
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<UnsplashApiSearchResponse>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (searchResult == null)
                {
                    _logger.LogWarning("Failed to deserialize Unsplash search response");
                    return new UnsplashSearchResponseViewModel();
                }

                return MapSearchResponse(searchResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching Unsplash for query: {Query}", query);
                return new UnsplashSearchResponseViewModel();
            }
        }

        public async Task<UnsplashSearchResultViewModel?> GetPhotoAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(_accessKey))
                {
                    _logger.LogWarning("Unsplash API access key not configured");
                    return null;
                }

                _logger.LogInformation("Getting Unsplash photo: {PhotoId}", id);

                var response = await _httpClient.GetAsync($"photos/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Unsplash photo request failed with status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var jsonContent = await response.Content.ReadAsStringAsync();
                var photo = JsonSerializer.Deserialize<UnsplashApiPhoto>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return photo != null ? MapPhoto(photo) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Unsplash photo: {PhotoId}", id);
                return null;
            }
        }

        public async Task TriggerDownloadAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(_accessKey))
                {
                    _logger.LogWarning("Unsplash API access key not configured");
                    return;
                }

                _logger.LogInformation("Triggering Unsplash download for: {PhotoId}", id);

                var response = await _httpClient.GetAsync($"photos/{id}/download");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Unsplash download trigger failed with status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering Unsplash download for: {PhotoId}", id);
            }
        }

        private static UnsplashSearchResponseViewModel MapSearchResponse(UnsplashApiSearchResponse apiResponse)
        {
            return new UnsplashSearchResponseViewModel
            {
                Total = apiResponse.Total,
                TotalPages = apiResponse.TotalPages,
                Results = apiResponse.Results?.Select(MapPhoto).ToList() ?? new List<UnsplashSearchResultViewModel>()
            };
        }

        private static UnsplashSearchResultViewModel MapPhoto(UnsplashApiPhoto photo)
        {
            return new UnsplashSearchResultViewModel
            {
                Id = photo.Id ?? string.Empty,
                Description = photo.Description ?? string.Empty,
                AltDescription = photo.AltDescription ?? string.Empty,
                Width = photo.Width,
                Height = photo.Height,
                Color = photo.Color ?? "#FFFFFF",
                Downloads = photo.Downloads,
                Likes = photo.Likes,
                Urls = new UnsplashUrlsViewModel
                {
                    Raw = photo.Urls?.Raw ?? string.Empty,
                    Full = photo.Urls?.Full ?? string.Empty,
                    Regular = photo.Urls?.Regular ?? string.Empty,
                    Small = photo.Urls?.Small ?? string.Empty,
                    Thumb = photo.Urls?.Thumb ?? string.Empty
                },
                User = new UnsplashUserViewModel
                {
                    Id = photo.User?.Id ?? string.Empty,
                    Username = photo.User?.Username ?? string.Empty,
                    Name = photo.User?.Name ?? string.Empty,
                    PortfolioUrl = photo.User?.PortfolioUrl ?? string.Empty,
                    Links = new UnsplashUserLinksViewModel
                    {
                        Self = photo.User?.Links?.Self ?? string.Empty,
                        Html = photo.User?.Links?.Html ?? string.Empty,
                        Portfolio = photo.User?.Links?.Portfolio ?? string.Empty
                    }
                }
            };
        }
    }

    // Internal classes for API deserialization
    internal class UnsplashApiSearchResponse
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public List<UnsplashApiPhoto>? Results { get; set; }
    }

    internal class UnsplashApiPhoto
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public string? AltDescription { get; set; }
        public UnsplashApiUrls? Urls { get; set; }
        public UnsplashApiUser? User { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Color { get; set; }
        public int Downloads { get; set; }
        public int Likes { get; set; }
    }

    internal class UnsplashApiUrls
    {
        public string? Raw { get; set; }
        public string? Full { get; set; }
        public string? Regular { get; set; }
        public string? Small { get; set; }
        public string? Thumb { get; set; }
    }

    internal class UnsplashApiUser
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? PortfolioUrl { get; set; }
        public UnsplashApiUserLinks? Links { get; set; }
    }

    internal class UnsplashApiUserLinks
    {
        public string? Self { get; set; }
        public string? Html { get; set; }
        public string? Portfolio { get; set; }
    }
}
