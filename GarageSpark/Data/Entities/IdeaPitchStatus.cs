using System.ComponentModel.DataAnnotations;

namespace GarageSpark.Data.Entities
{
    public class IdeaPitchStatus : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(7)]
        public string ColorHex { get; set; } = "#000000";

        // Navigation Properties
        public List<IdeaPitch> IdeaPitches { get; set; } = new();
    }
}
