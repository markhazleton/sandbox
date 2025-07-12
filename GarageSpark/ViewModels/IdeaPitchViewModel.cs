using System.ComponentModel.DataAnnotations;

namespace GarageSpark.ViewModels
{
    /// <summary>
    /// View model for IdeaPitch entity with validation attributes and display names
    /// </summary>
    public class IdeaPitchViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Submitter name is required")]
        [MaxLength(200, ErrorMessage = "Submitter name cannot exceed 200 characters")]
        [Display(Name = "Your Name")]
        public string SubmitterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [Display(Name = "Your Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string SubmitterEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idea title is required")]
        [MaxLength(200, ErrorMessage = "Idea title cannot exceed 200 characters")]
        [Display(Name = "Idea Title")]
        public string IdeaTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idea description is required")]
        [Display(Name = "Idea Description")]
        public string IdeaDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public Guid StatusId { get; set; }

        [Display(Name = "Linked Project")]
        public Guid? LinkedProjectId { get; set; }

        // Navigation Properties for Display
        [Display(Name = "Status")]
        public string StatusName { get; set; } = string.Empty;

        [Display(Name = "Status Color")]
        public string StatusColorHex { get; set; } = string.Empty;

        [Display(Name = "Linked Project")]
        public string LinkedProjectName { get; set; } = string.Empty;

        [Display(Name = "Linked Project Slug")]
        public string LinkedProjectSlug { get; set; } = string.Empty;
    }

    /// <summary>
    /// Simplified IdeaPitch view model for lists and summaries
    /// </summary>
    public class IdeaPitchSummaryViewModel
    {
        public Guid Id { get; set; }
        public string SubmitterName { get; set; } = string.Empty;
        public string IdeaTitle { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string StatusColorHex { get; set; } = string.Empty;
        public string LinkedProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Create/Edit form view model for IdeaPitch
    /// </summary>
    public class IdeaPitchFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Your name is required")]
        [MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
        [Display(Name = "Your Name")]
        public string SubmitterName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        [Display(Name = "Your Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string SubmitterEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Idea title is required")]
        [MaxLength(200, ErrorMessage = "Idea title cannot exceed 200 characters")]
        [Display(Name = "What's your idea?")]
        public string IdeaTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please describe your idea")]
        [Display(Name = "Tell us more about your idea")]
        public string IdeaDescription { get; set; } = string.Empty;
    }
}
