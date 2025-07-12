using GarageSpark.Data.Entities;
using GarageSpark.ViewModels;

namespace GarageSpark.Services.Mapping
{
    /// <summary>
    /// Extended mapping service implementation for additional entities
    /// </summary>
    public partial class MappingService
    {
        #region ProjectStatus Mappings

        public ProjectStatusViewModel ToViewModel(ProjectStatus entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProjectStatusViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Name = entity.Name,
                Emoji = entity.Emoji,
                DisplayColor = entity.DisplayColor,
                ProjectCount = entity.Projects?.Count ?? 0
            };
        }

        public ProjectStatus ToEntity(ProjectStatusViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new ProjectStatus
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Name = viewModel.Name,
                Emoji = viewModel.Emoji,
                DisplayColor = viewModel.DisplayColor
            };
        }

        public void UpdateEntity(ProjectStatus entity, ProjectStatusViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Name = viewModel.Name;
            entity.Emoji = viewModel.Emoji;
            entity.DisplayColor = viewModel.DisplayColor;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public ProjectStatusSummaryViewModel ToSummaryViewModel(ProjectStatus entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProjectStatusSummaryViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Emoji = entity.Emoji,
                DisplayColor = entity.DisplayColor
            };
        }

        #endregion

        #region CTA Mappings

        public CTAViewModel ToViewModel(CTA entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new CTAViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Label = entity.Label,
                Url = entity.Url,
                Style = entity.Style,
                Icon = entity.Icon,
                PageCount = entity.Pages?.Count ?? 0
            };
        }

        public CTA ToEntity(CTAViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new CTA
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Label = viewModel.Label,
                Url = viewModel.Url,
                Style = viewModel.Style,
                Icon = viewModel.Icon
            };
        }

        public void UpdateEntity(CTA entity, CTAViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Label = viewModel.Label;
            entity.Url = viewModel.Url;
            entity.Style = viewModel.Style;
            entity.Icon = viewModel.Icon;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public CTASummaryViewModel ToSummaryViewModel(CTA entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new CTASummaryViewModel
            {
                Id = entity.Id,
                Label = entity.Label,
                Url = entity.Url,
                Style = entity.Style,
                Icon = entity.Icon
            };
        }

        #endregion

        #region Page Mappings

        public PageViewModel ToViewModel(Page entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new PageViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Slug = entity.Slug,
                Title = entity.Title,
                SectionsJson = entity.SectionsJson,
                SeoJson = entity.SeoJson,
                SelectedCTAs = entity.PageCTAs?.Select(ToViewModel).ToList() ?? new List<PageCTAViewModel>()
            };
        }

        public Page ToEntity(PageViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new Page
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Slug = viewModel.Slug,
                Title = viewModel.Title,
                SectionsJson = viewModel.SectionsJson,
                SeoJson = viewModel.SeoJson
            };
        }

        public void UpdateEntity(Page entity, PageViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Slug = viewModel.Slug;
            entity.Title = viewModel.Title;
            entity.SectionsJson = viewModel.SectionsJson;
            entity.SeoJson = viewModel.SeoJson;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public PageSummaryViewModel ToSummaryViewModel(Page entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new PageSummaryViewModel
            {
                Id = entity.Id,
                Slug = entity.Slug,
                Title = entity.Title,
                CTACount = entity.CTAs?.Count ?? 0,
                UpdatedAt = entity.UpdatedAt
            };
        }

        #endregion

        #region IdeaPitch Mappings

        public IdeaPitchViewModel ToViewModel(IdeaPitch entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new IdeaPitchViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                SubmitterName = entity.SubmitterName,
                SubmitterEmail = entity.SubmitterEmail,
                IdeaTitle = entity.IdeaTitle,
                IdeaDescription = entity.IdeaDescription,
                StatusId = entity.StatusId,
                LinkedProjectId = entity.LinkedProjectId,
                StatusName = entity.Status?.Name ?? string.Empty,
                StatusColorHex = entity.Status?.ColorHex ?? string.Empty,
                LinkedProjectName = entity.LinkedProject?.Name ?? string.Empty,
                LinkedProjectSlug = entity.LinkedProject?.Slug ?? string.Empty
            };
        }

        public IdeaPitch ToEntity(IdeaPitchViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new IdeaPitch
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                SubmitterName = viewModel.SubmitterName,
                SubmitterEmail = viewModel.SubmitterEmail,
                IdeaTitle = viewModel.IdeaTitle,
                IdeaDescription = viewModel.IdeaDescription,
                StatusId = viewModel.StatusId,
                LinkedProjectId = viewModel.LinkedProjectId
            };
        }

        public void UpdateEntity(IdeaPitch entity, IdeaPitchViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.SubmitterName = viewModel.SubmitterName;
            entity.SubmitterEmail = viewModel.SubmitterEmail;
            entity.IdeaTitle = viewModel.IdeaTitle;
            entity.IdeaDescription = viewModel.IdeaDescription;
            entity.StatusId = viewModel.StatusId;
            entity.LinkedProjectId = viewModel.LinkedProjectId;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public IdeaPitchSummaryViewModel ToSummaryViewModel(IdeaPitch entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new IdeaPitchSummaryViewModel
            {
                Id = entity.Id,
                SubmitterName = entity.SubmitterName,
                IdeaTitle = entity.IdeaTitle,
                StatusName = entity.Status?.Name ?? string.Empty,
                StatusColorHex = entity.Status?.ColorHex ?? string.Empty,
                LinkedProjectName = entity.LinkedProject?.Name ?? string.Empty,
                CreatedAt = entity.CreatedAt
            };
        }

        #endregion

        #region IdeaPitchStatus Mappings

        public IdeaPitchStatusViewModel ToViewModel(IdeaPitchStatus entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new IdeaPitchStatusViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                Name = entity.Name,
                Description = entity.Description,
                ColorHex = entity.ColorHex,
                IdeaPitchCount = entity.IdeaPitches?.Count ?? 0
            };
        }

        public IdeaPitchStatus ToEntity(IdeaPitchStatusViewModel viewModel)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            return new IdeaPitchStatus
            {
                Id = viewModel.Id,
                CreatedAt = viewModel.CreatedAt,
                UpdatedAt = viewModel.UpdatedAt,
                CreatedBy = viewModel.CreatedBy,
                ModifiedBy = viewModel.ModifiedBy,
                Name = viewModel.Name,
                Description = viewModel.Description,
                ColorHex = viewModel.ColorHex
            };
        }

        public void UpdateEntity(IdeaPitchStatus entity, IdeaPitchStatusViewModel viewModel)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.ColorHex = viewModel.ColorHex;
            entity.ModifiedBy = viewModel.ModifiedBy;
        }

        public IdeaPitchStatusSummaryViewModel ToSummaryViewModel(IdeaPitchStatus entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new IdeaPitchStatusSummaryViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ColorHex = entity.ColorHex
            };
        }

        #endregion

        #region Junction Entity Mappings

        public ProjectTagViewModel ToViewModel(ProjectTag entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new ProjectTagViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                ProjectId = entity.ProjectId,
                TagId = entity.TagId,
                ProjectName = entity.Project?.Name ?? string.Empty,
                ProjectSlug = entity.Project?.Slug ?? string.Empty,
                TagName = entity.Tag?.Name ?? string.Empty,
                TagColorHex = entity.Tag?.ColorHex ?? string.Empty
            };
        }

        public SparkKitTagViewModel ToViewModel(SparkKitTag entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new SparkKitTagViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                SparkKitId = entity.SparkKitId,
                TagId = entity.TagId,
                SparkKitName = entity.SparkKit?.Name ?? string.Empty,
                SparkKitSlug = entity.SparkKit?.Slug ?? string.Empty,
                TagName = entity.Tag?.Name ?? string.Empty,
                TagColorHex = entity.Tag?.ColorHex ?? string.Empty
            };
        }

        public BlogPostTagViewModel ToViewModel(BlogPostTag entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new BlogPostTagViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                BlogPostId = entity.BlogPostId,
                TagId = entity.TagId,
                BlogPostTitle = entity.BlogPost?.Title ?? string.Empty,
                BlogPostSlug = entity.BlogPost?.Slug ?? string.Empty,
                TagName = entity.Tag?.Name ?? string.Empty,
                TagColorHex = entity.Tag?.ColorHex ?? string.Empty
            };
        }

        public PageCTAViewModel ToViewModel(PageCTA entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return new PageCTAViewModel
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                CreatedBy = entity.CreatedBy,
                ModifiedBy = entity.ModifiedBy,
                PageId = entity.PageId,
                CTAId = entity.CTAId,
                DisplayOrder = entity.DisplayOrder,
                PageTitle = entity.Page?.Title ?? string.Empty,
                CTALabel = entity.CTA?.Label ?? string.Empty,
                CTAStyle = entity.CTA?.Style ?? string.Empty
            };
        }

        #endregion
    }
}
