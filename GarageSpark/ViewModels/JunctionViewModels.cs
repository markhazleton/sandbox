using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for ProjectTag junction entity
    /// </summary>
    public class ProjectTagViewModel : BaseViewModel
    {
        [Required]
        public Guid ProjectId { get; set; }

        [Required]
        public Guid TagId { get; set; }

        // Navigation properties for display
        public string ProjectName { get; set; } = string.Empty;
        public string ProjectSlug { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string TagColorHex { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for SparkKitTag junction entity
    /// </summary>
    public class SparkKitTagViewModel : BaseViewModel
    {
        [Required]
        public Guid SparkKitId { get; set; }

        [Required]
        public Guid TagId { get; set; }

        // Navigation properties for display
        public string SparkKitName { get; set; } = string.Empty;
        public string SparkKitSlug { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string TagColorHex { get; set; } = string.Empty;
    }

    /// <summary>
    /// View model for BlogPostTag junction entity
    /// </summary>
    public class BlogPostTagViewModel : BaseViewModel
    {
        [Required]
        public Guid BlogPostId { get; set; }

        [Required]
        public Guid TagId { get; set; }

        // Navigation properties for display
        public string BlogPostTitle { get; set; } = string.Empty;
        public string BlogPostSlug { get; set; } = string.Empty;
        public string TagName { get; set; } = string.Empty;
        public string TagColorHex { get; set; } = string.Empty;
    }
}
