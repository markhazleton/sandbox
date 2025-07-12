using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class Tag : BaseEntity
    {
        [MaxLength(200)]
        public string? Slug { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(7)]
        public string ColorHex { get; set; } = "#000000";

        // Navigation Properties
        public List<ProjectTag> ProjectTags { get; set; } = new();
        public List<SparkKitTag> SparkKitTags { get; set; } = new();
        public List<BlogPostTag> BlogPostTags { get; set; } = new();
        public List<Project> Projects { get; set; } = new();
        public List<SparkKit> SparkKits { get; set; } = new();
        public List<BlogPost> BlogPosts { get; set; } = new();
    }
}
