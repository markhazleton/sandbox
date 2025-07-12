using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class BlogPost : BaseEntity
    {
        [MaxLength(200)]
        public string? Slug { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Excerpt { get; set; } = string.Empty;

        /// <summary>
        /// Raw Markdown content with frontmatter
        /// This is the source of truth - all other content is derived from this
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Processed HTML content (generated from markdown)
        /// </summary>
        public string ProcessedHtml { get; set; } = string.Empty;

        /// <summary>
        /// Frontmatter data as JSON (extracted from markdown)
        /// </summary>
        public string FrontmatterJson { get; set; } = string.Empty;

        /// <summary>
        /// SEO meta title (can be overridden by frontmatter)
        /// </summary>
        [MaxLength(60)]
        public string MetaTitle { get; set; } = string.Empty;

        /// <summary>
        /// SEO meta description (can be overridden by frontmatter)
        /// </summary>
        [MaxLength(160)]
        public string MetaDescription { get; set; } = string.Empty;

        /// <summary>
        /// SEO meta keywords
        /// </summary>
        [MaxLength(255)]
        public string MetaKeywords { get; set; } = string.Empty;

        /// <summary>
        /// Calculated word count from content
        /// </summary>
        public int WordCount { get; set; }

        /// <summary>
        /// Estimated reading time in minutes
        /// </summary>
        public int ReadingTimeMinutes { get; set; }

        [MaxLength(500)]
        public string CoverImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Cover media file ID - preferred over CoverImageUrl
        /// </summary>
        public Guid? CoverMediaFileId { get; set; }

        public DateTime? PublishedAt { get; set; }

        // Navigation Properties
        public MediaFile? CoverMediaFile { get; set; }
        public List<BlogPostTag> BlogPostTags { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public List<BlogPostMediaFile> BlogPostMediaFiles { get; set; } = new();
    }
}
