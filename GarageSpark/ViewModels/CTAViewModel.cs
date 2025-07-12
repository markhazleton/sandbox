using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for CTA (Call to Action) entity with validation attributes and display names
    /// </summary>
    public class CTAViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Label is required")]
        [MaxLength(200, ErrorMessage = "Label cannot exceed 200 characters")]
        [Display(Name = "Button Label")]
        public string Label { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "URL cannot exceed 500 characters")]
        [Display(Name = "Target URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Style cannot exceed 50 characters")]
        [Display(Name = "Button Style")]
        public string Style { get; set; } = "Primary";

        [MaxLength(100, ErrorMessage = "Icon cannot exceed 100 characters")]
        [Display(Name = "Icon Class")]
        public string Icon { get; set; } = string.Empty;

        [Display(Name = "Pages using this CTA")]
        public int PageCount { get; set; }

        // Available style options for dropdown
        public static readonly List<string> AvailableStyles = new() { "Primary", "Secondary", "Ghost" };
    }

    /// <summary>
    /// Simplified CTA view model for dropdowns and summaries
    /// </summary>
    public class CTASummaryViewModel
    {
        public Guid Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Style { get; set; } = "Primary";
        public string Icon { get; set; } = string.Empty;
    }

    /// <summary>
    /// CTA view model for selection in pages
    /// </summary>
    public class CTASelectionViewModel
    {
        public Guid Id { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Style { get; set; } = "Primary";
        public bool IsSelected { get; set; }
        public int DisplayOrder { get; set; }
    }
}
