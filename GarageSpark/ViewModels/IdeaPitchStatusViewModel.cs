using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for IdeaPitchStatus entity with validation attributes and display names
    /// </summary>
    public class IdeaPitchStatusViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Status Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [MaxLength(7, ErrorMessage = "Color must be a valid hex code")]
        [Display(Name = "Color (Hex)")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex code (e.g., #FF0000)")]
        public string ColorHex { get; set; } = "#000000";

        [Display(Name = "Idea Pitches with this Status")]
        public int IdeaPitchCount { get; set; }
    }

    /// <summary>
    /// Simplified IdeaPitchStatus view model for dropdowns and summaries
    /// </summary>
    public class IdeaPitchStatusSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ColorHex { get; set; } = "#000000";
    }
}
