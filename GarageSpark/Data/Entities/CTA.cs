using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class CTA : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Label { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Style { get; set; } = "Primary"; // Primary, Secondary, Ghost

        [MaxLength(100)]
        public string Icon { get; set; } = string.Empty;

        // Navigation Properties
        public List<PageCTA> PageCTAs { get; set; } = new();
        public List<Page> Pages { get; set; } = new();
    }
}
