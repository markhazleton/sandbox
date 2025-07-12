namespace GarageSpark.Data.Entities
{
    public class SparkKitTag : BaseEntity
    {
        public Guid SparkKitId { get; set; }
        public Guid TagId { get; set; }

        // Navigation Properties
        public SparkKit SparkKit { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
