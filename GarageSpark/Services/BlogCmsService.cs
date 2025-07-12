using GarageSpark.Data;
using GarageSpark.Data.Entities;
using GarageSpark.Services.Mapping;
using GarageSpark.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace GarageSpark.Services
{
    /// <summary>
    /// Blog CMS Service for managing blog posts with full CRUD operations
    /// Uses DbContext directly and ViewModels for all inputs/outputs
    /// </summary>
    public interface IBlogCmsService
    {
        // Core CRUD operations
        Task<BlogPostViewModel?> GetPostByIdAsync(Guid id);
        Task<BlogPostViewModel?> GetPostBySlugAsync(string slug);
        Task<BlogPostPagedResultViewModel> GetPostsAsync(BlogPostSearchViewModel searchCriteria);
        Task<BlogPostViewModel> CreatePostAsync(BlogPostFormViewModel formModel, string currentUser);
        Task<BlogPostViewModel?> UpdatePostAsync(Guid id, BlogPostFormViewModel formModel, string currentUser);
        Task<bool> DeletePostAsync(Guid id);

        // Publishing workflow
        Task<BlogPostViewModel?> PublishPostAsync(Guid id, string currentUser);
        Task<BlogPostViewModel?> UnpublishPostAsync(Guid id, string currentUser);
        Task<BlogPostViewModel?> SchedulePostAsync(Guid id, DateTime publishAt, string currentUser);

        // Tag management
        Task<List<TagSummaryViewModel>> GetAllTagsAsync();
        Task<List<TagSummaryViewModel>> SearchTagsAsync(string searchTerm);
        Task<TagSummaryViewModel> CreateTagAsync(string name, string colorHex, string currentUser);

        // Analytics and reporting
        Task<BlogAnalyticsViewModel> GetAnalyticsAsync();
        Task<List<BlogPostSummaryViewModel>> GetRecentPostsAsync(int count = 5);
        Task<List<BlogPostSummaryViewModel>> GetPopularPostsAsync(int count = 10);

        // Utility methods
        Task<string> GenerateUniqueSlugAsync(string title, Guid? excludePostId = null);
        Task<bool> IsSlugAvailableAsync(string slug, Guid? excludePostId = null);
    }

    public class BlogCmsService : IBlogCmsService
    {
        private readonly AppDbContext _context;
        private readonly IMappingService _mappingService;
        private readonly IMarkdownService _markdownService;

        public BlogCmsService(AppDbContext context, IMappingService mappingService, IMarkdownService markdownService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
            _markdownService = markdownService ?? throw new ArgumentNullException(nameof(markdownService));
        }

        #region Core CRUD Operations

        public async Task<BlogPostViewModel?> GetPostByIdAsync(Guid id)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Tags)
                .Include(p => p.CoverMediaFile)
                .Include(p => p.BlogPostMediaFiles)
                    .ThenInclude(bm => bm.MediaFile)
                .FirstOrDefaultAsync(p => p.Id == id);

            return post != null ? _mappingService.ToViewModel(post) : null;
        }

        public async Task<BlogPostViewModel?> GetPostBySlugAsync(string slug)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Tags)
                .Include(p => p.CoverMediaFile)
                .Include(p => p.BlogPostMediaFiles)
                    .ThenInclude(bm => bm.MediaFile)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            return post != null ? _mappingService.ToViewModel(post) : null;
        }

        public async Task<BlogPostPagedResultViewModel> GetPostsAsync(BlogPostSearchViewModel searchCriteria)
        {
            var query = _context.BlogPosts
                .Include(p => p.Tags)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchCriteria.SearchTerm))
            {
                var searchTerm = searchCriteria.SearchTerm.ToLower();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Excerpt.ToLower().Contains(searchTerm) ||
                    p.Content.ToLower().Contains(searchTerm));
            }

            if (searchCriteria.TagId.HasValue)
            {
                query = query.Where(p => p.Tags.Any(t => t.Id == searchCriteria.TagId.Value));
            }

            if (searchCriteria.Status.HasValue)
            {
                switch (searchCriteria.Status.Value)
                {
                    case BlogPostStatus.Draft:
                        query = query.Where(p => !p.PublishedAt.HasValue);
                        break;
                    case BlogPostStatus.Published:
                        query = query.Where(p => p.PublishedAt.HasValue && p.PublishedAt <= DateTime.UtcNow);
                        break;
                    case BlogPostStatus.Scheduled:
                        query = query.Where(p => p.PublishedAt.HasValue && p.PublishedAt > DateTime.UtcNow);
                        break;
                }
            }

            if (searchCriteria.FromDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt >= searchCriteria.FromDate.Value);
            }

            if (searchCriteria.ToDate.HasValue)
            {
                query = query.Where(p => p.CreatedAt <= searchCriteria.ToDate.Value);
            }

            // Apply sorting
            query = searchCriteria.SortBy switch
            {
                BlogPostSortBy.TitleAsc => query.OrderBy(p => p.Title),
                BlogPostSortBy.TitleDesc => query.OrderByDescending(p => p.Title),
                BlogPostSortBy.CreatedAsc => query.OrderBy(p => p.CreatedAt),
                BlogPostSortBy.CreatedDesc => query.OrderByDescending(p => p.CreatedAt),
                BlogPostSortBy.PublishedAsc => query.OrderBy(p => p.PublishedAt ?? DateTime.MaxValue),
                BlogPostSortBy.PublishedDesc => query.OrderByDescending(p => p.PublishedAt ?? DateTime.MinValue),
                BlogPostSortBy.UpdatedAsc => query.OrderBy(p => p.UpdatedAt),
                BlogPostSortBy.UpdatedDesc => query.OrderByDescending(p => p.UpdatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt)
            };

            // Get total count
            var totalCount = await query.CountAsync();

            // Apply pagination
            var posts = await query
                .Skip((searchCriteria.Page - 1) * searchCriteria.PageSize)
                .Take(searchCriteria.PageSize)
                .ToListAsync();

            return new BlogPostPagedResultViewModel
            {
                Posts = _mappingService.ToSummaryViewModels(posts).ToList(),
                TotalCount = totalCount,
                Page = searchCriteria.Page,
                PageSize = searchCriteria.PageSize,
                SearchCriteria = searchCriteria
            };
        }

        public async Task<BlogPostViewModel> CreatePostAsync(BlogPostFormViewModel formModel, string currentUser)
        {
            // Generate slug if not provided
            var slug = string.IsNullOrWhiteSpace(formModel.Slug)
                ? await GenerateUniqueSlugAsync(formModel.Title)
                : formModel.Slug;

            // Process markdown content and extract frontmatter
            var frontmatterResult = _markdownService.ExtractFrontmatter(formModel.Content);
            var processedHtml = _markdownService.ToHtml(frontmatterResult.Content);
            var frontmatterJson = frontmatterResult.Frontmatter.Any() ?
                System.Text.Json.JsonSerializer.Serialize(frontmatterResult.Frontmatter) : string.Empty;

            // Calculate content metrics
            var wordCount = CalculateWordCount(formModel.Content);
            var readingTime = CalculateReadingTime(wordCount);

            // Extract meta information from frontmatter or use form values
            var metaTitle = frontmatterResult.GetFrontmatterValue("title", formModel.Title) ?? formModel.Title;
            var metaDescription = frontmatterResult.GetFrontmatterValue("description", formModel.Excerpt) ?? formModel.Excerpt;

            // Determine publish date
            DateTime? publishedAt = null;
            if (formModel.Action == BlogPostAction.PublishNow || formModel.PublishNow)
            {
                publishedAt = DateTime.UtcNow;
            }
            else if (formModel.Action == BlogPostAction.Schedule && formModel.ScheduledPublishAt.HasValue)
            {
                publishedAt = formModel.ScheduledPublishAt.Value;
            }

            // Create entity
            var entity = new BlogPost
            {
                Id = Guid.NewGuid(),
                Slug = slug,
                Title = formModel.Title,
                Excerpt = formModel.Excerpt,
                Content = formModel.Content, // Store raw markdown
                ProcessedHtml = processedHtml, // Store processed HTML
                FrontmatterJson = frontmatterJson, // Store frontmatter as JSON
                MetaTitle = metaTitle,
                MetaDescription = metaDescription,
                WordCount = wordCount,
                ReadingTimeMinutes = readingTime,
                CoverMediaFileId = formModel.CoverMediaFileId,
                PublishedAt = publishedAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = currentUser,
                ModifiedBy = currentUser
            };

            _context.BlogPosts.Add(entity);
            await _context.SaveChangesAsync();

            // Handle tags
            if (!string.IsNullOrWhiteSpace(formModel.TagNames))
            {
                await UpdatePostTagsAsync(entity.Id, formModel.TagNames, currentUser);
            }

            // Handle media file relationships
            if (formModel.SelectedMediaFileIds?.Any() == true)
            {
                await AddPostMediaFilesAsync(entity.Id, formModel.SelectedMediaFileIds);
            }

            // Return the created post
            return await GetPostByIdAsync(entity.Id) ?? throw new InvalidOperationException("Failed to retrieve created post");
        }

        public async Task<BlogPostViewModel?> UpdatePostAsync(Guid id, BlogPostFormViewModel formModel, string currentUser)
        {
            var entity = await _context.BlogPosts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (entity == null)
                return null;

            // Update slug logic
            if (!string.IsNullOrWhiteSpace(formModel.Slug) && formModel.Slug != entity.Slug)
            {
                // User provided a specific slug
                if (await IsSlugAvailableAsync(formModel.Slug, id))
                {
                    entity.Slug = formModel.Slug;
                }
            }
            else if (string.IsNullOrWhiteSpace(formModel.Slug) || string.IsNullOrWhiteSpace(entity.Slug) || formModel.Title != entity.Title)
            {
                // Generate new slug if no slug provided, entity has no slug, or title changed
                entity.Slug = await GenerateUniqueSlugAsync(formModel.Title, id);
            }

            // Process markdown content and extract frontmatter
            var frontmatterResult = _markdownService.ExtractFrontmatter(formModel.Content);
            var processedHtml = _markdownService.ToHtml(frontmatterResult.Content);
            var frontmatterJson = frontmatterResult.Frontmatter.Any() ?
                System.Text.Json.JsonSerializer.Serialize(frontmatterResult.Frontmatter) : string.Empty;

            // Calculate content metrics
            var wordCount = CalculateWordCount(formModel.Content);
            var readingTime = CalculateReadingTime(wordCount);

            // Extract meta information from frontmatter or use form values
            var metaTitle = frontmatterResult.GetFrontmatterValue("title", formModel.Title) ?? formModel.Title;
            var metaDescription = frontmatterResult.GetFrontmatterValue("description", formModel.Excerpt) ?? formModel.Excerpt;

            // Update basic properties
            entity.Title = formModel.Title;
            entity.Excerpt = formModel.Excerpt;
            entity.Content = formModel.Content; // Store raw markdown
            entity.ProcessedHtml = processedHtml; // Store processed HTML
            entity.FrontmatterJson = frontmatterJson; // Store frontmatter as JSON
            entity.MetaTitle = metaTitle;
            entity.MetaDescription = metaDescription;
            entity.WordCount = wordCount;
            entity.ReadingTimeMinutes = readingTime;
            entity.CoverMediaFileId = formModel.CoverMediaFileId;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ModifiedBy = currentUser;

            // Handle publishing actions
            switch (formModel.Action)
            {
                case BlogPostAction.PublishNow:
                    entity.PublishedAt = DateTime.UtcNow;
                    break;
                case BlogPostAction.Schedule when formModel.ScheduledPublishAt.HasValue:
                    entity.PublishedAt = formModel.ScheduledPublishAt.Value;
                    break;
                case BlogPostAction.Unpublish:
                    entity.PublishedAt = null;
                    break;
                case BlogPostAction.SaveDraft:
                    // Keep existing publish date for drafts
                    break;
            }

            await _context.SaveChangesAsync();

            // Handle tags
            if (!string.IsNullOrWhiteSpace(formModel.TagNames))
            {
                await UpdatePostTagsAsync(entity.Id, formModel.TagNames, currentUser);
            }

            // Handle media files
            if (formModel.SelectedMediaFileIds?.Any() == true)
            {
                await UpdatePostMediaFilesAsync(entity.Id, formModel.SelectedMediaFileIds);
            }

            return await GetPostByIdAsync(id);
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
                return false;

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Publishing Workflow

        public async Task<BlogPostViewModel?> PublishPostAsync(Guid id, string currentUser)
        {
            var entity = await _context.BlogPosts.FindAsync(id);
            if (entity == null)
                return null;

            entity.PublishedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ModifiedBy = currentUser;

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(id);
        }

        public async Task<BlogPostViewModel?> UnpublishPostAsync(Guid id, string currentUser)
        {
            var entity = await _context.BlogPosts.FindAsync(id);
            if (entity == null)
                return null;

            entity.PublishedAt = null;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ModifiedBy = currentUser;

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(id);
        }

        public async Task<BlogPostViewModel?> SchedulePostAsync(Guid id, DateTime publishAt, string currentUser)
        {
            var entity = await _context.BlogPosts.FindAsync(id);
            if (entity == null)
                return null;

            entity.PublishedAt = publishAt;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ModifiedBy = currentUser;

            await _context.SaveChangesAsync();
            return await GetPostByIdAsync(id);
        }

        #endregion

        #region Tag Management

        public async Task<List<TagSummaryViewModel>> GetAllTagsAsync()
        {
            var tags = await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(tags).ToList();
        }

        public async Task<List<TagSummaryViewModel>> SearchTagsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllTagsAsync();

            var tags = await _context.Tags
                .Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()))
                .OrderBy(t => t.Name)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(tags).ToList();
        }

        public async Task<TagSummaryViewModel> CreateTagAsync(string name, string colorHex, string currentUser)
        {
            var slug = GenerateSlug(name);

            // Ensure unique slug
            var existingTag = await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
            if (existingTag != null)
            {
                throw new InvalidOperationException($"A tag with slug '{slug}' already exists");
            }

            var entity = new Tag
            {
                Id = Guid.NewGuid(),
                Name = name,
                Slug = slug,
                ColorHex = colorHex,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = currentUser,
                ModifiedBy = currentUser
            };

            _context.Tags.Add(entity);
            await _context.SaveChangesAsync();

            return _mappingService.ToSummaryViewModel(entity);
        }

        #endregion

        #region Analytics and Reporting

        public async Task<BlogAnalyticsViewModel> GetAnalyticsAsync()
        {
            var now = DateTime.UtcNow;

            var totalPosts = await _context.BlogPosts.CountAsync();
            var publishedPosts = await _context.BlogPosts.CountAsync(p => p.PublishedAt.HasValue && p.PublishedAt <= now);
            var draftPosts = await _context.BlogPosts.CountAsync(p => !p.PublishedAt.HasValue);
            var scheduledPosts = await _context.BlogPosts.CountAsync(p => p.PublishedAt.HasValue && p.PublishedAt > now);
            var totalTags = await _context.Tags.CountAsync();

            // Calculate content metrics
            var publishedPostsData = await _context.BlogPosts
                .Where(p => p.PublishedAt.HasValue && p.PublishedAt <= now)
                .Select(p => p.Content)
                .ToListAsync();

            var totalWords = publishedPostsData.Sum(CalculateWordCount);
            var averageReadingTime = publishedPostsData.Any()
                ? publishedPostsData.Average(content => CalculateReadingTime(CalculateWordCount(content)))
                : 0;

            // Get popular tags
            var popularTags = await _context.Tags
                .Include(t => t.BlogPosts)
                .Where(t => t.BlogPosts.Any())
                .OrderByDescending(t => t.BlogPosts.Count)
                .Take(10)
                .Select(t => new TagUsageViewModel
                {
                    Name = t.Name,
                    Slug = t.Slug,
                    ColorHex = t.ColorHex,
                    UsageCount = t.BlogPosts.Count
                })
                .ToListAsync();

            // Get recent posts
            var recentPosts = await GetRecentPostsAsync(5);

            return new BlogAnalyticsViewModel
            {
                TotalPosts = totalPosts,
                PublishedPosts = publishedPosts,
                DraftPosts = draftPosts,
                ScheduledPosts = scheduledPosts,
                TotalTags = totalTags,
                TotalWords = totalWords,
                AverageReadingTime = averageReadingTime,
                PopularTags = popularTags,
                RecentPosts = recentPosts
            };
        }

        public async Task<List<BlogPostSummaryViewModel>> GetRecentPostsAsync(int count = 5)
        {
            var posts = await _context.BlogPosts
                .Include(p => p.Tags)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(posts).ToList();
        }

        public async Task<List<BlogPostSummaryViewModel>> GetPopularPostsAsync(int count = 10)
        {
            // For now, return most recent published posts
            // In future, could integrate with analytics service for view counts
            var posts = await _context.BlogPosts
                .Include(p => p.Tags)
                .Where(p => p.PublishedAt.HasValue && p.PublishedAt <= DateTime.UtcNow)
                .OrderByDescending(p => p.PublishedAt)
                .Take(count)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(posts).ToList();
        }

        #endregion

        #region Utility Methods

        public async Task<string> GenerateUniqueSlugAsync(string title, Guid? excludePostId = null)
        {
            var baseSlug = GenerateSlug(title);
            var slug = baseSlug;
            var counter = 1;

            while (!await IsSlugAvailableAsync(slug, excludePostId))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludePostId = null)
        {
            var query = _context.BlogPosts.Where(p => p.Slug == slug);

            if (excludePostId.HasValue)
            {
                query = query.Where(p => p.Id != excludePostId.Value);
            }

            return !await query.AnyAsync();
        }

        private static string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Convert to lowercase and replace spaces with hyphens
            var slug = input.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace("_", "-");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from start and end
            slug = slug.Trim('-');

            return slug;
        }

        private static int CalculateWordCount(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return 0;

            // Remove markdown syntax for more accurate word count
            var cleanContent = Regex.Replace(content, @"[#*_`\[\]()]+", " ");
            cleanContent = Regex.Replace(cleanContent, @"\s+", " ");

            return cleanContent.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static int CalculateReadingTime(int wordCount)
        {
            // Average reading speed is 200-250 words per minute
            // Using 225 as average
            const int wordsPerMinute = 225;
            return Math.Max(1, (int)Math.Ceiling((double)wordCount / wordsPerMinute));
        }

        private async Task UpdatePostTagsAsync(Guid postId, string tagNames, string currentUser)
        {
            // Remove existing tag associations
            var existingTags = await _context.BlogPostTags
                .Where(bt => bt.BlogPostId == postId)
                .ToListAsync();

            _context.BlogPostTags.RemoveRange(existingTags);

            // Parse and create/find tags
            var tagNameList = tagNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Distinct()
                .ToList();

            foreach (var tagName in tagNameList)
            {
                var slug = GenerateSlug(tagName);

                // Find or create tag
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Id = Guid.NewGuid(),
                        Name = tagName,
                        Slug = slug,
                        ColorHex = GenerateRandomColor(),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        CreatedBy = currentUser,
                        ModifiedBy = currentUser
                    };
                    _context.Tags.Add(tag);
                }

                // Create association
                _context.BlogPostTags.Add(new BlogPostTag
                {
                    Id = Guid.NewGuid(),
                    BlogPostId = postId,
                    TagId = tag.Id,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = currentUser,
                    ModifiedBy = currentUser
                });
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdatePostMediaFilesAsync(Guid postId, List<Guid> mediaFileIds)
        {
            // Remove existing media file associations
            var existingMediaFiles = await _context.BlogPostMediaFiles
                .Where(bm => bm.BlogPostId == postId)
                .ToListAsync();

            if (existingMediaFiles.Any())
            {
                _context.BlogPostMediaFiles.RemoveRange(existingMediaFiles);
                await _context.SaveChangesAsync(); // Save removal first to avoid conflicts
            }

            // Add new associations if any
            if (mediaFileIds?.Any() == true)
            {
                var displayOrder = 0;
                var newAssociations = new List<BlogPostMediaFile>();

                foreach (var mediaFileId in mediaFileIds.Distinct()) // Use Distinct to avoid duplicates
                {
                    newAssociations.Add(new BlogPostMediaFile
                    {
                        BlogPostId = postId,
                        MediaFileId = mediaFileId,
                        DisplayOrder = displayOrder++,
                        IsFeatured = false // Feature flag can be set separately
                    });
                }

                _context.BlogPostMediaFiles.AddRange(newAssociations);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddPostMediaFilesAsync(Guid postId, List<Guid> mediaFileIds)
        {
            if (mediaFileIds?.Any() != true)
                return;

            Console.WriteLine($"[DEBUG] AddPostMediaFilesAsync called with postId: {postId}, mediaFileIds: [{string.Join(", ", mediaFileIds)}]");

            var displayOrder = 0;
            var newAssociations = new List<BlogPostMediaFile>();

            foreach (var mediaFileId in mediaFileIds.Distinct()) // Use Distinct to avoid duplicates
            {
                Console.WriteLine($"[DEBUG] Creating association: BlogPostId={postId}, MediaFileId={mediaFileId}, DisplayOrder={displayOrder}");
                newAssociations.Add(new BlogPostMediaFile
                {
                    BlogPostId = postId,
                    MediaFileId = mediaFileId,
                    DisplayOrder = displayOrder++,
                    IsFeatured = false // Feature flag can be set separately
                });
            }

            Console.WriteLine($"[DEBUG] Adding {newAssociations.Count} associations to context");
            _context.BlogPostMediaFiles.AddRange(newAssociations);
            await _context.SaveChangesAsync();
            Console.WriteLine($"[DEBUG] Successfully saved media file associations");
        }

        private static string GenerateRandomColor()
        {
            var random = new Random();
            var colors = new[]
            {
                "#3B82F6", "#EF4444", "#10B981", "#F59E0B", "#8B5CF6",
                "#EC4899", "#06B6D4", "#84CC16", "#F97316", "#6366F1"
            };
            return colors[random.Next(colors.Length)];
        }

        #endregion
    }
}
