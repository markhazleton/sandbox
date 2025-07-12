using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for ProjectStatus entity with validation attributes and display names
    /// </summary>
    public class ProjectStatusViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Status Name")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10, ErrorMessage = "Emoji cannot exceed 10 characters")]
        [Display(Name = "Emoji")]
        public string Emoji { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Display Color cannot exceed 50 characters")]
        [Display(Name = "Display Color")]
        public string DisplayColor { get; set; } = string.Empty;

        [Display(Name = "Projects with this Status")]
        public int ProjectCount { get; set; }
    }

    /// <summary>
    /// Simplified ProjectStatus view model for dropdowns and summaries
    /// </summary>
    public class ProjectStatusSummaryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Emoji { get; set; } = string.Empty;
        public string DisplayColor { get; set; } = string.Empty;
    }
}
