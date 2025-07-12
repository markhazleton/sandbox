using System.ComponentModel.DataAnnotations;
using GarageSpark.Data.Entities;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for MediaFile entity
    /// </summary>
    public class MediaFileViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "File name is required")]
        [MaxLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        [Display(Name = "File Name")]
        public string FileName { get; set; } = string.Empty;

        [Display(Name = "Original File Name")]
        public string OriginalFileName { get; set; } = string.Empty;

        [Display(Name = "File URL")]
        public string Url { get; set; } = string.Empty;

        [Display(Name = "Content Type")]
        public string ContentType { get; set; } = string.Empty;

        [Display(Name = "File Size")]
        public long FileSize { get; set; }

        [Display(Name = "Width")]
        public int? Width { get; set; }

        [Display(Name = "Height")]
        public int? Height { get; set; }

        [MaxLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
        [Display(Name = "Alt Text")]
        public string Alt { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Caption cannot exceed 500 characters")]
        [Display(Name = "Caption")]
        public string Caption { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Source")]
        public MediaSource Source { get; set; }

        [Display(Name = "External ID")]
        public string? ExternalId { get; set; }

        [Display(Name = "External URL")]
        public string? ExternalUrl { get; set; }

        [Display(Name = "External Author")]
        public string? ExternalAuthor { get; set; }

        [Display(Name = "External Author URL")]
        public string? ExternalAuthorUrl { get; set; }

        [Display(Name = "External Source")]
        public string? ExternalSource { get; set; }

        [Display(Name = "Is Public")]
        public bool IsPublic { get; set; } = true;

        // Computed properties
        public string FormattedFileSize => FormatFileSize(FileSize);
        public string Dimensions => Width.HasValue && Height.HasValue ? $"{Width}×{Height}" : "Unknown";
        public bool IsImage => ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        public bool IsVideo => ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
        public bool IsDocument => !IsImage && !IsVideo;

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    /// <summary>
    /// Form view model for uploading media files
    /// </summary>
    public class MediaUploadViewModel
    {
        [Required(ErrorMessage = "Please select a file")]
        [Display(Name = "File")]
        public IFormFile File { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Alt text cannot exceed 255 characters")]
        [Display(Name = "Alt Text")]
        public string Alt { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Caption cannot exceed 500 characters")]
        [Display(Name = "Caption")]
        public string Caption { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Is Public")]
        public bool IsPublic { get; set; } = true;
    }

    /// <summary>
    /// Search and filter view model for media library
    /// </summary>
    public class MediaSearchViewModel
    {
        [Display(Name = "Search")]
        public string SearchTerm { get; set; } = string.Empty;

        [Display(Name = "File Type")]
        public MediaFileType? FileType { get; set; }

        [Display(Name = "Source")]
        public MediaSource? Source { get; set; }

        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Sort By")]
        public MediaSortBy SortBy { get; set; } = MediaSortBy.CreatedDesc;

        [Display(Name = "Page Size")]
        [Range(12, 100, ErrorMessage = "Page size must be between 12 and 100")]
        public int PageSize { get; set; } = 24;

        public int Page { get; set; } = 1;
    }

    /// <summary>
    /// Paginated result for media files
    /// </summary>
    public class MediaPagedResultViewModel
    {
        public List<MediaFileViewModel> MediaFiles { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
        public MediaSearchViewModel SearchCriteria { get; set; } = new();
    }

    /// <summary>
    /// Unsplash search result view model
    /// </summary>
    public class UnsplashSearchResultViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AltDescription { get; set; } = string.Empty;
        public UnsplashUrlsViewModel Urls { get; set; } = new();
        public UnsplashUserViewModel User { get; set; } = new();
        public int Width { get; set; }
        public int Height { get; set; }
        public string Color { get; set; } = string.Empty;
        public int Downloads { get; set; }
        public int Likes { get; set; }
    }

    /// <summary>
    /// Unsplash image URLs
    /// </summary>
    public class UnsplashUrlsViewModel
    {
        public string Raw { get; set; } = string.Empty;
        public string Full { get; set; } = string.Empty;
        public string Regular { get; set; } = string.Empty;
        public string Small { get; set; } = string.Empty;
        public string Thumb { get; set; } = string.Empty;
    }

    /// <summary>
    /// Unsplash user information
    /// </summary>
    public class UnsplashUserViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PortfolioUrl { get; set; } = string.Empty;
        public UnsplashUserLinksViewModel Links { get; set; } = new();
    }

    /// <summary>
    /// Unsplash user links
    /// </summary>
    public class UnsplashUserLinksViewModel
    {
        public string Self { get; set; } = string.Empty;
        public string Html { get; set; } = string.Empty;
        public string Portfolio { get; set; } = string.Empty;
    }

    /// <summary>
    /// Unsplash search response
    /// </summary>
    public class UnsplashSearchResponseViewModel
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public List<UnsplashSearchResultViewModel> Results { get; set; } = new();
    }

    /// <summary>
    /// Media library stats
    /// </summary>
    public class MediaLibraryStatsViewModel
    {
        public int TotalFiles { get; set; }
        public int Images { get; set; }
        public int Videos { get; set; }
        public int Documents { get; set; }
        public long TotalSize { get; set; }
        public int LocalFiles { get; set; }
        public int UnsplashFiles { get; set; }
        public int ExternalFiles { get; set; }
        public string FormattedTotalSize => FormatFileSize(TotalSize);

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }

    /// <summary>
    /// Enums for media operations
    /// </summary>
    public enum MediaFileType
    {
        Image,
        Video,
        Document
    }

    public enum MediaSortBy
    {
        NameAsc,
        NameDesc,
        CreatedAsc,
        CreatedDesc,
        SizeAsc,
        SizeDesc,
        TypeAsc,
        TypeDesc
    }
}
