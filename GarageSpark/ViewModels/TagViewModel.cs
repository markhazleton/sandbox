using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for Tag entity with validation attributes and display names
    /// </summary>
    public class TagViewModel : BaseViewModel
    {
        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        [Display(Name = "URL Slug (leave empty to auto-generate from name)")]
        [RegularExpression(@"^$|^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Tag Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(7, ErrorMessage = "Color must be a valid hex code")]
        [Display(Name = "Color (Hex)")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex code (e.g., #FF0000)")]
        public string ColorHex { get; set; } = "#000000";

        // Usage counts for display
        [Display(Name = "Projects Using This Tag")]
        public int ProjectCount { get; set; }

        [Display(Name = "SparkKits Using This Tag")]
        public int SparkKitCount { get; set; }

        [Display(Name = "Blog Posts Using This Tag")]
        public int BlogPostCount { get; set; }

        [Display(Name = "Total Usage")]
        public int TotalUsage => ProjectCount + SparkKitCount + BlogPostCount;
    }

    /// <summary>
    /// Simplified Tag view model for dropdowns and summaries
    /// </summary>
    public class TagSummaryViewModel
    {
        public Guid Id { get; set; }
        public string? Slug { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#000000";
    }

    /// <summary>
    /// Tag view model for selection lists
    /// </summary>
    public class TagSelectionViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#000000";
        public bool IsSelected { get; set; }
    }
}
