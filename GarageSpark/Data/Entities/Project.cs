using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{

    // ===== CORE ENTITIES =====
    public class Project : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Summary { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public Guid StatusId { get; set; }

        [MaxLength(500)]
        public string HeroImageUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DemoUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string RepoUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DocsUrl { get; set; } = string.Empty;

        public int Order { get; set; }

        // Navigation Properties
        public ProjectStatus Status { get; set; } = null!;
        public List<ProjectTag> ProjectTags { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public List<IdeaPitch> LinkedPitches { get; set; } = new();
    }
}
