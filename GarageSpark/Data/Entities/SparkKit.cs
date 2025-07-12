using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class SparkKit : BaseEntity
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

        [MaxLength(50)]
        public string Version { get; set; } = string.Empty;

        [MaxLength(200)]
        public string InstallCommand { get; set; } = string.Empty;

        [MaxLength(500)]
        public string HeroImageUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string RepoUrl { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DocsUrl { get; set; } = string.Empty;

        // Navigation Properties
        public List<SparkKitTag> SparkKitTags { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
    }
}
