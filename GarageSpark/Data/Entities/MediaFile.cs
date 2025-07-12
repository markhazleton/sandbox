using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    /// <summary>
    /// Represents a media file (image, video, document) in the system
    /// </summary>
    public class MediaFile : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public int? Width { get; set; }
        public int? Height { get; set; }

        [MaxLength(255)]
        public string Alt { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Caption { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public MediaSource Source { get; set; } = MediaSource.Local;

        [MaxLength(255)]
        public string? ExternalId { get; set; }

        [MaxLength(500)]
        public string? ExternalUrl { get; set; }

        [MaxLength(255)]
        public string? ExternalAuthor { get; set; }

        [MaxLength(500)]
        public string? ExternalAuthorUrl { get; set; }

        [MaxLength(255)]
        public string? ExternalSource { get; set; }

        public bool IsPublic { get; set; } = true;

        // Navigation properties
        public List<BlogPostMediaFile> BlogPostMediaFiles { get; set; } = new();
    }

    /// <summary>
    /// Junction table for many-to-many relationship between BlogPost and MediaFile
    /// </summary>
    public class BlogPostMediaFile
    {
        public Guid BlogPostId { get; set; }
        public BlogPost BlogPost { get; set; } = null!;

        public Guid MediaFileId { get; set; }
        public MediaFile MediaFile { get; set; } = null!;

        public MediaUsageType UsageType { get; set; } = MediaUsageType.Content;
        public int? SortOrder { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsFeatured { get; set; } = false;
    }

    /// <summary>
    /// Source of the media file
    /// </summary>
    public enum MediaSource
    {
        Local = 0,
        Unsplash = 1,
        External = 2
    }

    /// <summary>
    /// How the media file is used in the blog post
    /// </summary>
    public enum MediaUsageType
    {
        CoverImage = 0,
        Content = 1,
        Gallery = 2,
        Thumbnail = 3
    }
}
