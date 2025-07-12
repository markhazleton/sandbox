using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for Page entity with validation attributes and display names
    /// </summary>
    public class PageViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Slug is required")]
        [MaxLength(200, ErrorMessage = "Slug cannot exceed 200 characters")]
        [Display(Name = "URL Slug")]
        [RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
        public string Slug { get; set; } = string.Empty;

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        [Display(Name = "Page Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Page Sections (JSON)")]
        public string SectionsJson { get; set; } = string.Empty;

        [Display(Name = "SEO Data (JSON)")]
        public string SeoJson { get; set; } = string.Empty;

        [Display(Name = "Call to Actions")]
        public List<CTASelectionViewModel> CTAs { get; set; } = new();

        // For form binding
        [Display(Name = "Selected CTA IDs with Order")]
        public List<PageCTAViewModel> SelectedCTAs { get; set; } = new();
    }

    /// <summary>
    /// View model for PageCTA junction entity
    /// </summary>
    public class PageCTAViewModel : BaseViewModel
    {
        public Guid PageId { get; set; }
        public Guid CTAId { get; set; }

        [Display(Name = "Display Order")]
        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a positive number")]
        public int DisplayOrder { get; set; }

        // Navigation properties for display
        public string PageTitle { get; set; } = string.Empty;
        public string CTALabel { get; set; } = string.Empty;
        public string CTAStyle { get; set; } = string.Empty;
    }

    /// <summary>
    /// Simplified Page view model for lists and summaries
    /// </summary>
    public class PageSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int CTACount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
