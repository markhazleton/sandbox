using GarageSpark.Data.Entities;
using GarageSpark.ViewModels;
using System.Text.Json;

namespace GarageSpark.Services.Mapping
{
    /// <summary>
    /// Mapping service implementation for converting between entities and view models
    /// </summary>
    public partial class MappingService : IMappingService
    {
        #region Project Mappings

        public ProjectViewModel ToViewModel(Project entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProjectViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Slug = entity.Slug,
                Name = entity.Name,
                Summary = entity.Summary,
                Description = entity.Description,
                StatusId = entity.StatusId,
                HeroImageUrl = entity.HeroImageUrl,
                DemoUrl = entity.DemoUrl,
                RepoUrl = entity.RepoUrl,
                DocsUrl = entity.DocsUrl,
                Order = entity.Order,
                StatusName = entity.Status?.Name ?? string.Empty,
                StatusColor = entity.Status?.DisplayColor ?? string.Empty,
                StatusEmoji = entity.Status?.Emoji ?? string.Empty,
                Tags = entity.Tags?.Select(ToSummaryViewModel).ToList() ?? new List<TagSummaryViewModel>(),
                LinkedPitches = entity.LinkedPitches?.Select(ToSummaryViewModel).ToList() ?? new List<IdeaPitchSummaryViewModel>(),
                SelectedTagIds = entity.Tags?.Select(t => t.Id).ToList() ?? new List<Guid>()
            };
        }

        public Project ToEntity(ProjectViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new Project
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Slug = viewModel.Slug,
                Name = viewModel.Name,
                Summary = viewModel.Summary,
                Description = viewModel.Description,
                StatusId = viewModel.StatusId,
                HeroImageUrl = viewModel.HeroImageUrl,
                DemoUrl = viewModel.DemoUrl,
                RepoUrl = viewModel.RepoUrl,
                DocsUrl = viewModel.DocsUrl,
                Order = viewModel.Order
            };
        }

        public void UpdateEntity(Project entity, ProjectViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Slug = viewModel.Slug;
            entity.Name = viewModel.Name;
            entity.Summary = viewModel.Summary;
            entity.Description = viewModel.Description;
            entity.StatusId = viewModel.StatusId;
            entity.HeroImageUrl = viewModel.HeroImageUrl;
            entity.DemoUrl = viewModel.DemoUrl;
            entity.RepoUrl = viewModel.RepoUrl;
            entity.DocsUrl = viewModel.DocsUrl;
            entity.Order = viewModel.Order;
            entity.ModifiedBy = viewModel.ModifiedBy;
            // UpdatedAt will be handled by the DbContext
        }

        public ProjectSummaryViewModel ToSummaryViewModel(Project entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProjectSummaryViewModel
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Name = entity.Name,
                Summary = entity.Summary,
                HeroImageUrl = entity.HeroImageUrl,
                StatusName = entity.Status?.Name ?? string.Empty,
                StatusColor = entity.Status?.DisplayColor ?? string.Empty,
                StatusEmoji = entity.Status?.Emoji ?? string.Empty,
                Order = entity.Order,
                TagNames = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
            };
        }

        #endregion

        #region SparkKit Mappings

        public SparkKitViewModel ToViewModel(SparkKit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new SparkKitViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Slug = entity.Slug,
                Name = entity.Name,
                Summary = entity.Summary,
                Description = entity.Description,
                Version = entity.Version,
                InstallCommand = entity.InstallCommand,
                HeroImageUrl = entity.HeroImageUrl,
                RepoUrl = entity.RepoUrl,
                DocsUrl = entity.DocsUrl,
                Tags = entity.Tags?.Select(ToSummaryViewModel).ToList() ?? new List<TagSummaryViewModel>(),
                SelectedTagIds = entity.Tags?.Select(t => t.Id).ToList() ?? new List<Guid>()
            };
        }

        public SparkKit ToEntity(SparkKitViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new SparkKit
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Slug = viewModel.Slug,
                Name = viewModel.Name,
                Summary = viewModel.Summary,
                Description = viewModel.Description,
                Version = viewModel.Version,
                InstallCommand = viewModel.InstallCommand,
                HeroImageUrl = viewModel.HeroImageUrl,
                RepoUrl = viewModel.RepoUrl,
                DocsUrl = viewModel.DocsUrl
            };
        }

        public void UpdateEntity(SparkKit entity, SparkKitViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Slug = viewModel.Slug;
            entity.Name = viewModel.Name;
            entity.Summary = viewModel.Summary;
            entity.Description = viewModel.Description;
            entity.Version = viewModel.Version;
            entity.InstallCommand = viewModel.InstallCommand;
            entity.HeroImageUrl = viewModel.HeroImageUrl;
            entity.RepoUrl = viewModel.RepoUrl;
            entity.DocsUrl = viewModel.DocsUrl;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public SparkKitSummaryViewModel ToSummaryViewModel(SparkKit entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new SparkKitSummaryViewModel
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Name = entity.Name,
                Summary = entity.Summary,
                Version = entity.Version,
                HeroImageUrl = entity.HeroImageUrl,
                TagNames = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>()
            };
        }

        #endregion

        #region BlogPost Mappings

        public BlogPostViewModel ToViewModel(BlogPost entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var frontmatterData = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(entity.FrontmatterJson))
            {
                try
                {
                    frontmatterData = JsonSerializer.Deserialize<Dictionary<string, object>>(entity.FrontmatterJson) ?? new();
                }
                catch (JsonException)
                {
                    frontmatterData = new Dictionary<string, object>();
                }
            }

            return new BlogPostViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Slug = entity.Slug,
                Title = entity.Title,
                Excerpt = entity.Excerpt,
                Content = entity.Content,
                ProcessedHtml = entity.ProcessedHtml,
                FrontmatterData = frontmatterData,
                CoverImageUrl = entity.CoverImageUrl,
                PublishedAt = entity.PublishedAt,
                ReadingTimeMinutes = entity.ReadingTimeMinutes,
                WordCount = entity.WordCount,
                MetaTitle = entity.MetaTitle,
                MetaDescription = entity.MetaDescription,
                Tags = entity.Tags?.Select(ToSummaryViewModel).ToList() ?? new List<TagSummaryViewModel>(),
                SelectedTagIds = entity.Tags?.Select(t => t.Id).ToList() ?? new List<Guid>()
            };
        }

        public BlogPost ToEntity(BlogPostViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new BlogPost
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Slug = viewModel.Slug,
                Title = viewModel.Title,
                Excerpt = viewModel.Excerpt,
                Content = viewModel.Content,
                ProcessedHtml = viewModel.ProcessedHtml,
                FrontmatterJson = viewModel.FrontmatterData.Any() ? JsonSerializer.Serialize(viewModel.FrontmatterData) : string.Empty,
                CoverImageUrl = viewModel.CoverImageUrl,
                PublishedAt = viewModel.PublishedAt,
                ReadingTimeMinutes = viewModel.ReadingTimeMinutes,
                WordCount = viewModel.WordCount,
                MetaTitle = viewModel.MetaTitle,
                MetaDescription = viewModel.MetaDescription
            };
        }

        public void UpdateEntity(BlogPost entity, BlogPostViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Slug = viewModel.Slug;
            entity.Title = viewModel.Title;
            entity.Excerpt = viewModel.Excerpt;
            entity.Content = viewModel.Content;
            entity.ProcessedHtml = viewModel.ProcessedHtml;
            entity.FrontmatterJson = viewModel.FrontmatterData.Any() ? JsonSerializer.Serialize(viewModel.FrontmatterData) : string.Empty;
            entity.CoverImageUrl = viewModel.CoverImageUrl;
            entity.PublishedAt = viewModel.PublishedAt;
            entity.ReadingTimeMinutes = viewModel.ReadingTimeMinutes;
            entity.WordCount = viewModel.WordCount;
            entity.MetaTitle = viewModel.MetaTitle;
            entity.MetaDescription = viewModel.MetaDescription;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public BlogPostSummaryViewModel ToSummaryViewModel(BlogPost entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new BlogPostSummaryViewModel
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Title = entity.Title,
                Excerpt = entity.Excerpt,
                CoverImageUrl = entity.CoverImageUrl,
                PublishedAt = entity.PublishedAt,
                TagNames = entity.Tags?.Select(t => t.Name).ToList() ?? new List<string>(),
                CreatedAt = entity.CreatedAt
            };
        }

        #endregion

        #region Tag Mappings

        public TagViewModel ToViewModel(Tag entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new TagViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Slug = entity.Slug,
                Name = entity.Name,
                ColorHex = entity.ColorHex,
                ProjectCount = entity.Projects?.Count ?? 0,
                SparkKitCount = entity.SparkKits?.Count ?? 0,
                BlogPostCount = entity.BlogPosts?.Count ?? 0
            };
        }

        public Tag ToEntity(TagViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new Tag
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Slug = viewModel.Slug,
                Name = viewModel.Name,
                ColorHex = viewModel.ColorHex
            };
        }

        public void UpdateEntity(Tag entity, TagViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Slug = viewModel.Slug;
            entity.Name = viewModel.Name;
            entity.ColorHex = viewModel.ColorHex;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public TagSummaryViewModel ToSummaryViewModel(Tag entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new TagSummaryViewModel
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Name = entity.Name,
                ColorHex = entity.ColorHex
            };
        }

        #endregion

        #region Collection Mapping Extensions

        /// <summary>
        /// Maps a collection of entities to view models
        /// </summary>
        public IEnumerable<TViewModel> ToViewModels<TEntity, TViewModel>(
            IEnumerable<TEntity> entities,
            Func<TEntity, TViewModel> mapper)
            where TEntity : BaseEntity
            where TViewModel : BaseViewModel
        {
            return entities?.Select(mapper) ?? Enumerable.Empty<TViewModel>();
        }

        /// <summary>
        /// Maps a collection of entities to summary view models
        /// </summary>
        public IEnumerable<TSummaryViewModel> ToSummaryViewModels<TEntity, TSummaryViewModel>(
            IEnumerable<TEntity> entities,
            Func<TEntity, TSummaryViewModel> mapper)
            where TEntity : BaseEntity
        {
            return entities?.Select(mapper) ?? Enumerable.Empty<TSummaryViewModel>();
        }

        // Specific collection mappings for common scenarios
        public IEnumerable<ProjectViewModel> ToViewModels(IEnumerable<Project> entities) =>
            ToViewModels(entities, ToViewModel);

        public IEnumerable<ProjectSummaryViewModel> ToSummaryViewModels(IEnumerable<Project> entities) =>
            ToSummaryViewModels(entities, ToSummaryViewModel);

        public IEnumerable<SparkKitViewModel> ToViewModels(IEnumerable<SparkKit> entities) =>
            ToViewModels(entities, ToViewModel);

        public IEnumerable<SparkKitSummaryViewModel> ToSummaryViewModels(IEnumerable<SparkKit> entities) =>
            ToSummaryViewModels(entities, ToSummaryViewModel);

        public IEnumerable<BlogPostViewModel> ToViewModels(IEnumerable<BlogPost> entities) =>
            ToViewModels(entities, ToViewModel);

        public IEnumerable<BlogPostSummaryViewModel> ToSummaryViewModels(IEnumerable<BlogPost> entities) =>
            ToSummaryViewModels(entities, ToSummaryViewModel);

        public IEnumerable<TagViewModel> ToViewModels(IEnumerable<Tag> entities) =>
            ToViewModels(entities, ToViewModel);

        public IEnumerable<TagSummaryViewModel> ToSummaryViewModels(IEnumerable<Tag> entities) =>
            ToSummaryViewModels(entities, ToSummaryViewModel);

        #endregion

        #region MediaFile Mappings

        public MediaFileViewModel ToViewModel(MediaFile entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new MediaFileViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                FileName = entity.FileName,
                OriginalFileName = entity.OriginalFileName,
                Url = entity.Url,
                ContentType = entity.ContentType,
                FileSize = entity.FileSize,
                Width = entity.Width,
                Height = entity.Height,
                Alt = entity.Alt,
                Caption = entity.Caption,
                Description = entity.Description,
                Source = entity.Source,
                ExternalId = entity.ExternalId,
                ExternalUrl = entity.ExternalUrl,
                ExternalAuthor = entity.ExternalAuthor,
                ExternalAuthorUrl = entity.ExternalAuthorUrl,
                ExternalSource = entity.ExternalSource,
                IsPublic = entity.IsPublic
            };
        }

        public MediaFile ToEntity(MediaFileViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new MediaFile
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                FileName = viewModel.FileName,
                OriginalFileName = viewModel.OriginalFileName,
                Url = viewModel.Url,
                ContentType = viewModel.ContentType,
                FileSize = viewModel.FileSize,
                Width = viewModel.Width,
                Height = viewModel.Height,
                Alt = viewModel.Alt,
                Caption = viewModel.Caption,
                Description = viewModel.Description,
                Source = viewModel.Source,
                ExternalId = viewModel.ExternalId,
                ExternalUrl = viewModel.ExternalUrl,
                ExternalAuthor = viewModel.ExternalAuthor,
                ExternalAuthorUrl = viewModel.ExternalAuthorUrl,
                ExternalSource = viewModel.ExternalSource,
                IsPublic = viewModel.IsPublic
            };
        }

        public void UpdateEntity(MediaFile entity, MediaFileViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Alt = viewModel.Alt;
            entity.Caption = viewModel.Caption;
            entity.Description = viewModel.Description;
            entity.IsPublic = viewModel.IsPublic;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        #endregion
    }
}
