using GarageSpark.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GarageSpark.Data
{

    // ===== DBCONTEXT =====
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Project> Projects { get; set; }
        public DbSet<SparkKit> SparkKits { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public DbSet<CTA> CTAs { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<IdeaPitch> IdeaPitches { get; set; }
        public DbSet<IdeaPitchStatus> IdeaPitchStatuses { get; set; }
        public DbSet<ProjectTag> ProjectTags { get; set; }
        public DbSet<SparkKitTag> SparkKitTags { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<PageCTA> PageCTAs { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }
        public DbSet<BlogPostMediaFile> BlogPostMediaFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names
            modelBuilder.Entity<Project>().ToTable("Projects");
            modelBuilder.Entity<SparkKit>().ToTable("SparkKits");
            modelBuilder.Entity<BlogPost>().ToTable("BlogPosts");
            modelBuilder.Entity<Tag>().ToTable("Tags");
            modelBuilder.Entity<ProjectStatus>().ToTable("ProjectStatuses");
            modelBuilder.Entity<CTA>().ToTable("CTAs");
            modelBuilder.Entity<Page>().ToTable("Pages");
            modelBuilder.Entity<IdeaPitch>().ToTable("IdeaPitches");
            modelBuilder.Entity<IdeaPitchStatus>().ToTable("IdeaPitchStatuses");
            modelBuilder.Entity<ProjectTag>().ToTable("ProjectTags");
            modelBuilder.Entity<SparkKitTag>().ToTable("SparkKitTags");
            modelBuilder.Entity<BlogPostTag>().ToTable("BlogPostTags");
            modelBuilder.Entity<PageCTA>().ToTable("PageCTAs");
            modelBuilder.Entity<MediaFile>().ToTable("MediaFiles");
            modelBuilder.Entity<BlogPostMediaFile>().ToTable("BlogPostMediaFiles");

            // Configure unique indexes for slugs
            modelBuilder.Entity<Project>()
                .HasIndex(e => e.Slug)
                .IsUnique();

            modelBuilder.Entity<SparkKit>()
                .HasIndex(e => e.Slug)
                .IsUnique();

            modelBuilder.Entity<BlogPost>()
                .HasIndex(e => e.Slug)
                .IsUnique();

            modelBuilder.Entity<Tag>()
                .HasIndex(e => e.Slug)
                .IsUnique();

            modelBuilder.Entity<Page>()
                .HasIndex(e => e.Slug)
                .IsUnique();

            // Configure relationships
            // Project -> ProjectStatus
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Status)
                .WithMany(s => s.Projects)
                .HasForeignKey(p => p.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project -> Tags (many-to-many through ProjectTag)
            modelBuilder.Entity<ProjectTag>()
                .HasOne(pt => pt.Project)
                .WithMany(p => p.ProjectTags)
                .HasForeignKey(pt => pt.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProjectTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Projects)
                .UsingEntity<ProjectTag>();

            // SparkKit -> Tags (many-to-many through SparkKitTag)
            modelBuilder.Entity<SparkKitTag>()
                .HasOne(st => st.SparkKit)
                .WithMany(s => s.SparkKitTags)
                .HasForeignKey(st => st.SparkKitId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SparkKitTag>()
                .HasOne(st => st.Tag)
                .WithMany(t => t.SparkKitTags)
                .HasForeignKey(st => st.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SparkKit>()
                .HasMany(s => s.Tags)
                .WithMany(t => t.SparkKits)
                .UsingEntity<SparkKitTag>();

            // BlogPost -> Tags (many-to-many through BlogPostTag)
            modelBuilder.Entity<BlogPostTag>()
                .HasOne(bt => bt.BlogPost)
                .WithMany(b => b.BlogPostTags)
                .HasForeignKey(bt => bt.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogPostTag>()
                .HasOne(bt => bt.Tag)
                .WithMany(t => t.BlogPostTags)
                .HasForeignKey(bt => bt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogPost>()
                .HasMany(b => b.Tags)
                .WithMany(t => t.BlogPosts)
                .UsingEntity<BlogPostTag>();

            // Page -> CTAs (many-to-many through PageCTA with ordering)
            modelBuilder.Entity<PageCTA>()
                .HasOne(pc => pc.Page)
                .WithMany(p => p.PageCTAs)
                .HasForeignKey(pc => pc.PageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PageCTA>()
                .HasOne(pc => pc.CTA)
                .WithMany(c => c.PageCTAs)
                .HasForeignKey(pc => pc.CTAId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Page>()
                .HasMany(p => p.CTAs)
                .WithMany(c => c.Pages)
                .UsingEntity<PageCTA>();

            // IdeaPitch -> IdeaPitchStatus
            modelBuilder.Entity<IdeaPitch>()
                .HasOne(ip => ip.Status)
                .WithMany(s => s.IdeaPitches)
                .HasForeignKey(ip => ip.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // IdeaPitch -> Project (optional)
            modelBuilder.Entity<IdeaPitch>()
                .HasOne(ip => ip.LinkedProject)
                .WithMany(p => p.LinkedPitches)
                .HasForeignKey(ip => ip.LinkedProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure composite indexes for junction tables
            modelBuilder.Entity<ProjectTag>()
                .HasIndex(pt => new { pt.ProjectId, pt.TagId })
                .IsUnique();

            modelBuilder.Entity<SparkKitTag>()
                .HasIndex(st => new { st.SparkKitId, st.TagId })
                .IsUnique();

            modelBuilder.Entity<BlogPostTag>()
                .HasIndex(bt => new { bt.BlogPostId, bt.TagId })
                .IsUnique();

            modelBuilder.Entity<PageCTA>()
                .HasIndex(pc => new { pc.PageId, pc.CTAId })
                .IsUnique();

            // BlogPost -> MediaFiles (many-to-many through BlogPostMediaFile)
            modelBuilder.Entity<BlogPostMediaFile>()
                .HasKey(bm => new { bm.BlogPostId, bm.MediaFileId });

            modelBuilder.Entity<BlogPostMediaFile>()
                .HasOne(bm => bm.BlogPost)
                .WithMany(b => b.BlogPostMediaFiles)
                .HasForeignKey(bm => bm.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlogPostMediaFile>()
                .HasOne(bm => bm.MediaFile)
                .WithMany(m => m.BlogPostMediaFiles)
                .HasForeignKey(bm => bm.MediaFileId)
                .OnDelete(DeleteBehavior.Cascade);

            // BlogPost -> CoverMediaFile (optional one-to-one)
            modelBuilder.Entity<BlogPost>()
                .HasOne(b => b.CoverMediaFile)
                .WithMany()
                .HasForeignKey(b => b.CoverMediaFileId)
                .OnDelete(DeleteBehavior.SetNull);

            // MediaFile indexes
            modelBuilder.Entity<MediaFile>()
                .HasIndex(m => m.FileName);

            modelBuilder.Entity<MediaFile>()
                .HasIndex(m => new { m.Source, m.ExternalId });
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}