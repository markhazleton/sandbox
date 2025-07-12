using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class IdeaPitch : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string SubmitterName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string SubmitterEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string IdeaTitle { get; set; } = string.Empty;

        public string IdeaDescription { get; set; } = string.Empty;

        public Guid StatusId { get; set; }

        public Guid? LinkedProjectId { get; set; }

        // Navigation Properties
        public IdeaPitchStatus Status { get; set; } = null!;
        public Project? LinkedProject { get; set; }
    }
}
