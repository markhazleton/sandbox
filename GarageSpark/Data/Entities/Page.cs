using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class Page : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string SectionsJson { get; set; } = string.Empty;

        public string SeoJson { get; set; } = string.Empty;

        // Navigation Properties
        public List<PageCTA> PageCTAs { get; set; } = new();
        public List<CTA> CTAs { get; set; } = new();
    }
}
