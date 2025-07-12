namespace GarageSpark.Data.Entities
{
    public class BlogPostTag : BaseEntity
    {
        public Guid BlogPostId { get; set; }
        public Guid TagId { get; set; }

        // Navigation Properties
        public BlogPost BlogPost { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
