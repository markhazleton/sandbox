using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class ProjectStatus : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Emoji { get; set; } = string.Empty;

        [MaxLength(50)]
        public string DisplayColor { get; set; } = string.Empty;

        // Navigation Properties
        public List<Project> Projects { get; set; } = new();
    }
}
