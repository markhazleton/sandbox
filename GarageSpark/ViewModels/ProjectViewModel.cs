using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for Project entity with validation attributes and display names
    /// </summary>
    public class ProjectViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Slug is required")]
        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        [Display(Name = "URL Slug")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Project Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Summary cannot exceed 500 characters")]
        [Display(Name = "Summary")]
        public string Summary { get; set; } = string.Empty;

        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Project Status")]
        public Guid StatusId { get; set; }

        [MaxLength(500, ErrorMessage = "Hero Image URL cannot exceed 500 characters")]
        [Display(Name = "Hero Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string HeroImageUrl { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Demo URL cannot exceed 500 characters")]
        [Display(Name = "Demo URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string DemoUrl { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Repository URL cannot exceed 500 characters")]
        [Display(Name = "Repository URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string RepoUrl { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Documentation URL cannot exceed 500 characters")]
        [Display(Name = "Documentation URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string DocsUrl { get; set; } = string.Empty;

        [Display(Name = "Display Order")]
        [Range(0, int.MaxValue, ErrorMessage = "Order must be a positive number")]
        public int Order { get; set; }

        // Navigation Properties for Display
        [Display(Name = "Status")]
        public string StatusName { get; set; } = string.Empty;

        [Display(Name = "Status Color")]
        public string StatusColor { get; set; } = string.Empty;

        [Display(Name = "Status Emoji")]
        public string StatusEmoji { get; set; } = string.Empty;

        [Display(Name = "Tags")]
        public List<TagSummaryViewModel> Tags { get; set; } = new();

        [Display(Name = "Linked Idea Pitches")]
        public List<IdeaPitchSummaryViewModel> LinkedPitches { get; set; } = new();

        // For form binding
        [Display(Name = "Selected Tag IDs")]
        public List<Guid> SelectedTagIds { get; set; } = new();
    }

    /// <summary>
    /// Simplified Project view model for lists and summaries
    /// </summary>
    public class ProjectSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string HeroImageUrl { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string StatusEmoji { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<string> TagNames { get; set; } = new();
    }
}
