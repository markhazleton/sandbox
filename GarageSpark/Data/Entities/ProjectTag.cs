namespace GarageSpark.Data.Entities
{

    // ===== JUNCTION ENTITIES =====
    public class ProjectTag : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid TagId { get; set; }

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
