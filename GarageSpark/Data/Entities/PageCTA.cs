namespace GarageSpark.Data.Entities
{
    public class PageCTA : BaseEntity
    {
        public Guid PageId { get; set; }
        public Guid CTAId { get; set; }
        public int DisplayOrder { get; set; }

        // Navigation Properties
        public Page Page { get; set; } = null!;
        public CTA CTA { get; set; } = null!;
    }
}
