using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for BlogPost entity with validation attributes and display names
    /// </summary>
    public class BlogPostViewModel : BaseViewModel
    {
        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        [Display(Name = "URL Slug (leave empty to auto-generate from title)")]
        [RegularExpression(@"^$|^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Blog Post Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Excerpt cannot exceed 500 characters")]
        [Display(Name = "Excerpt")]
        public string Excerpt { get; set; } = string.Empty;

        [Display(Name = "Content (Markdown)")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Processed HTML")]
        public string ProcessedHtml { get; set; } = string.Empty;

        [Display(Name = "Frontmatter Data")]
        public Dictionary<string, object> FrontmatterData { get; set; } = new();

        [MaxLength(500, ErrorMessage = "Cover Image URL cannot exceed 500 characters")]
        [Display(Name = "Cover Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Display(Name = "Published Date")]
        public DateTime? PublishedAt { get; set; }

        [Display(Name = "Is Published")]
        public bool IsPublished => PublishedAt.HasValue && PublishedAt <= DateTime.UtcNow;

        [Display(Name = "Is Draft")]
        public bool IsDraft => !IsPublished;

        [Display(Name = "Meta Title")]
        [MaxLength(60, ErrorMessage = "Meta title should not exceed 60 characters for SEO")]
        public string MetaTitle { get; set; } = string.Empty;

        [Display(Name = "Meta Description")]
        [MaxLength(160, ErrorMessage = "Meta description should not exceed 160 characters for SEO")]
        public string MetaDescription { get; set; } = string.Empty;

        [Display(Name = "Meta Keywords")]
        [MaxLength(255, ErrorMessage = "Meta keywords cannot exceed 255 characters")]
        public string MetaKeywords { get; set; } = string.Empty;

        [Display(Name = "Reading Time (minutes)")]
        public int ReadingTimeMinutes { get; set; }

        [Display(Name = "Word Count")]
        public int WordCount { get; set; }

        [Display(Name = "Tags")]
        public List<TagSummaryViewModel> Tags { get; set; } = new();

        [Display(Name = "Cover Media File")]
        public MediaFileViewModel? CoverMediaFile { get; set; }

        [Display(Name = "Cover Media File ID")]
        public Guid? CoverMediaFileId { get; set; }

        [Display(Name = "Associated Media Files")]
        public List<MediaFileViewModel> MediaFiles { get; set; } = new();

        // For form binding
        [Display(Name = "Selected Tag IDs")]
        public List<Guid> SelectedTagIds { get; set; } = new();

        [Display(Name = "Tag Names (comma-separated)")]
        public string TagNamesInput { get; set; } = string.Empty;
    }

    /// <summary>
    /// Simplified BlogPost view model for lists and summaries
    /// </summary>
    public class BlogPostSummaryViewModel
    {
        public Guid Id { get; set; }
        public string? Slug { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string CoverImageUrl { get; set; } = string.Empty;
        public MediaFileViewModel? CoverMediaFile { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsPublished => PublishedAt.HasValue && PublishedAt <= DateTime.UtcNow;
        public List<string> TagNames { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int WordCount { get; set; }
        public int ReadingTimeMinutes { get; set; }
    }

    /// <summary>
    /// Create/Edit form view model for BlogPost with enhanced CMS features
    /// </summary>
    public class BlogPostFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Post Title")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        [Display(Name = "URL Slug (leave empty to auto-generate from title)")]
        [RegularExpression(@"^$|^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string? Slug { get; set; }

        [MaxLength(500, ErrorMessage = "Excerpt cannot exceed 500 characters")]
        [Display(Name = "Excerpt")]
        public string Excerpt { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [Display(Name = "Content (Markdown)")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Cover Image URL cannot exceed 500 characters")]
        [Display(Name = "Cover Image URL (deprecated)")]
        [Obsolete("Use CoverMediaFileId instead")]
        public string CoverImageUrl { get; set; } = string.Empty;

        [Display(Name = "Cover Image (optional)")]
        public Guid? CoverMediaFileId { get; set; }

        [Display(Name = "Selected Media Files")]
        public List<Guid> SelectedMediaFileIds { get; set; } = new();

        // String property for form binding (JavaScript sets comma-separated GUIDs)
        [Display(Name = "Selected Media Files (String)")]
        public string SelectedMediaFileIdsString { get; set; } = string.Empty;

        [Display(Name = "Publish Immediately")]
        public bool PublishNow { get; set; }

        [Display(Name = "Is Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Scheduled Publish Date")]
        public DateTime? ScheduledPublishAt { get; set; }

        [Display(Name = "Meta Title (SEO)")]
        [MaxLength(60, ErrorMessage = "Meta title should not exceed 60 characters for SEO")]
        public string MetaTitle { get; set; } = string.Empty;

        [Display(Name = "Meta Description (SEO)")]
        [MaxLength(160, ErrorMessage = "Meta description should not exceed 160 characters for SEO")]
        public string MetaDescription { get; set; } = string.Empty;

        [Display(Name = "Tags (comma-separated)")]
        public string TagNames { get; set; } = string.Empty;

        [Display(Name = "Action")]
        public BlogPostAction Action { get; set; } = BlogPostAction.SaveDraft;

        // Additional properties for Edit view
        [Display(Name = "Word Count")]
        public int WordCount { get; set; }

        [Display(Name = "Reading Time (minutes)")]
        public int ReadingTimeMinutes { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Updated Date")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; } = string.Empty;
    }

    /// <summary>
    /// BlogPost search and filter view model
    /// </summary>
    public class BlogPostSearchViewModel
    {
        [Display(Name = "Search")]
        public string SearchTerm { get; set; } = string.Empty;

        [Display(Name = "Tag")]
        public Guid? TagId { get; set; }

        [Display(Name = "Status")]
        public BlogPostStatus? Status { get; set; }

        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Sort By")]
        public BlogPostSortBy SortBy { get; set; } = BlogPostSortBy.CreatedDesc;

        [Display(Name = "Page Size")]
        [Range(5, 100, ErrorMessage = "Page size must be between 5 and 100")]
        public int PageSize { get; set; } = 10;

        public int Page { get; set; } = 1;
    }

    /// <summary>
    /// Paginated result for blog posts
    /// </summary>
    public class BlogPostPagedResultViewModel
    {
        public List<BlogPostSummaryViewModel> Posts { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
        public BlogPostSearchViewModel SearchCriteria { get; set; } = new();
    }

    /// <summary>
    /// Blog analytics view model
    /// </summary>
    public class BlogAnalyticsViewModel
    {
        public int TotalPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public int ScheduledPosts { get; set; }
        public int TotalTags { get; set; }
        public int TotalWords { get; set; }
        public double AverageReadingTime { get; set; }
        public double AverageWordsPerPost { get; set; }
        public int PostsWithExcerpts { get; set; }
        public int PostsWithImages { get; set; }
        public int PostsWithSEO { get; set; }

        public List<TagUsageViewModel> PopularTags { get; set; } = new();
        public List<TagStatsViewModel> TagStats { get; set; } = new();
        public List<BlogPostSummaryViewModel> RecentPosts { get; set; } = new();
        public List<BlogPostSummaryViewModel> TopPostsByLength { get; set; } = new();
        public List<MonthlyPostCountViewModel> MonthlyPostCounts { get; set; } = new();
    }

    /// <summary>
    /// Monthly post count for analytics
    /// </summary>
    public class MonthlyPostCountViewModel
    {
        public string MonthYear { get; set; } = string.Empty;
        public int PostCount { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }

    /// <summary>
    /// Tag statistics for analytics
    /// </summary>
    public class TagStatsViewModel
    {
        public string TagName { get; set; } = string.Empty;
        public int UsageCount { get; set; }
        public string ColorHex { get; set; } = string.Empty;
    }

    /// <summary>
    /// Tag usage analytics
    /// </summary>
    public class TagUsageViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string ColorHex { get; set; } = string.Empty;
        public int UsageCount { get; set; }
    }

    /// <summary>
    /// Enums for BlogPost operations
    /// </summary>
    public enum BlogPostAction
    {
        SaveDraft,
        PublishNow,
        Schedule,
        Unpublish
    }

    public enum BlogPostStatus
    {
        Draft,
        Published,
        Scheduled
    }

    public enum BlogPostSortBy
    {
        TitleAsc,
        TitleDesc,
        CreatedAsc,
        CreatedDesc,
        PublishedAsc,
        PublishedDesc,
        UpdatedAsc,
        UpdatedDesc
    }
}
