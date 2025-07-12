using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for SparkKit entity with validation attributes and display names
    /// </summary>
    public class SparkKitViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Slug is required")]
        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        [Display(Name = "URL Slug")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "SparkKit Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
        [Display(Name = "Summary")]
        public string Summary { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Version cannot exceed 50 characters")]
        [Display(Name = "Version")]
        public string Version { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Install Command cannot exceed 200 characters")]
        [Display(Name = "Install Command")]
        public string InstallCommand { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Hero Image URL cannot exceed 500 characters")]
        [Display(Name = "Hero Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string HeroImageUrl { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Repository URL cannot exceed 500 characters")]
        [Display(Name = "Repository URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string RepoUrl { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Documentation URL cannot exceed 500 characters")]
        [Display(Name = "Documentation URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string DocsUrl { get; set; } = string.Empty;

        [Display(Name = "Tags")]
        public List<TagSummaryViewModel> Tags { get; set; } = new();

        // For form binding
        [Display(Name = "Selected Tag IDs")]
        public List<Guid> SelectedTagIds { get; set; } = new();
    }

    /// <summary>
    /// Simplified SparkKit view model for lists and summaries
    /// </summary>
    public class SparkKitSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public List<string> TagNames { get; set; } = new();
    }
}
