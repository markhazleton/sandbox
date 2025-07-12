using GarageSpark.Data.Entities;
using GarageSpark.ViewModels;

namespace GarageSpark.Services.Mapping
{
    /// <summary>
    /// Interface for mapping between entities and view models
    /// </summary>
    /// <typeparam name="TEntity">The entity type</typeparam>
    /// <typeparam name="TViewModel">The view model type</typeparam>
    /// <typeparam name="TSummaryViewModel">The summary view model type</typeparam>
    public interface IEntityMapper<TEntity, TViewModel, TSummaryViewModel>
        where TEntity : BaseEntity
        where TViewModel : BaseViewModel
    {
        /// <summary>
        /// Maps an entity to a view model
        /// </summary>
        TViewModel ToViewModel(TEntity entity);

        /// <summary>
        /// Maps a view model to an entity
        /// </summary>
        TEntity ToEntity(TViewModel viewModel);

        /// <summary>
        /// Updates an existing entity with values from a view model
        /// </summary>
        void UpdateEntity(TEntity entity, TViewModel viewModel);

        /// <summary>
        /// Maps an entity to a summary view model
        /// </summary>
        TSummaryViewModel ToSummaryViewModel(TEntity entity);

        /// <summary>
        /// Maps a collection of entities to view models
        /// </summary>
        IEnumerable<TViewModel> ToViewModels(IEnumerable<TEntity> entities);

        /// <summary>
        /// Maps a collection of entities to summary view models
        /// </summary>
        IEnumerable<TSummaryViewModel> ToSummaryViewModels(IEnumerable<TEntity> entities);
    }

    /// <summary>
    /// Base interface for all mapping services
    /// </summary>
    public interface IMappingService
    {
        // Project mappings
        ProjectViewModel ToViewModel(Project entity);
        Project ToEntity(ProjectViewModel viewModel);
        void UpdateEntity(Project entity, ProjectViewModel viewModel);
        ProjectSummaryViewModel ToSummaryViewModel(Project entity);

        // SparkKit mappings
        SparkKitViewModel ToViewModel(SparkKit entity);
        SparkKit ToEntity(SparkKitViewModel viewModel);
        void UpdateEntity(SparkKit entity, SparkKitViewModel viewModel);
        SparkKitSummaryViewModel ToSummaryViewModel(SparkKit entity);

        // BlogPost mappings
        BlogPostViewModel ToViewModel(BlogPost entity);
        BlogPost ToEntity(BlogPostViewModel viewModel);
        void UpdateEntity(BlogPost entity, BlogPostViewModel viewModel);
        BlogPostSummaryViewModel ToSummaryViewModel(BlogPost entity);

        // Tag mappings
        TagViewModel ToViewModel(Tag entity);
        Tag ToEntity(TagViewModel viewModel);
        void UpdateEntity(Tag entity, TagViewModel viewModel);
        TagSummaryViewModel ToSummaryViewModel(Tag entity);

        // ProjectStatus mappings
        ProjectStatusViewModel ToViewModel(ProjectStatus entity);
        ProjectStatus ToEntity(ProjectStatusViewModel viewModel);
        void UpdateEntity(ProjectStatus entity, ProjectStatusViewModel viewModel);
        ProjectStatusSummaryViewModel ToSummaryViewModel(ProjectStatus entity);

        // CTA mappings
        CTAViewModel ToViewModel(CTA entity);
        CTA ToEntity(CTAViewModel viewModel);
        void UpdateEntity(CTA entity, CTAViewModel viewModel);
        CTASummaryViewModel ToSummaryViewModel(CTA entity);

        // Page mappings
        PageViewModel ToViewModel(Page entity);
        Page ToEntity(PageViewModel viewModel);
        void UpdateEntity(Page entity, PageViewModel viewModel);
        PageSummaryViewModel ToSummaryViewModel(Page entity);

        // IdeaPitch mappings
        IdeaPitchViewModel ToViewModel(IdeaPitch entity);
        IdeaPitch ToEntity(IdeaPitchViewModel viewModel);
        void UpdateEntity(IdeaPitch entity, IdeaPitchViewModel viewModel);
        IdeaPitchSummaryViewModel ToSummaryViewModel(IdeaPitch entity);

        // IdeaPitchStatus mappings
        IdeaPitchStatusViewModel ToViewModel(IdeaPitchStatus entity);
        IdeaPitchStatus ToEntity(IdeaPitchStatusViewModel viewModel);
        void UpdateEntity(IdeaPitchStatus entity, IdeaPitchStatusViewModel viewModel);
        IdeaPitchStatusSummaryViewModel ToSummaryViewModel(IdeaPitchStatus entity);

        // MediaFile mappings
        MediaFileViewModel ToViewModel(MediaFile entity);
        MediaFile ToEntity(MediaFileViewModel viewModel);
        void UpdateEntity(MediaFile entity, MediaFileViewModel viewModel);

        // Junction entity mappings
        ProjectTagViewModel ToViewModel(ProjectTag entity);
        SparkKitTagViewModel ToViewModel(SparkKitTag entity);
        BlogPostTagViewModel ToViewModel(BlogPostTag entity);
        PageCTAViewModel ToViewModel(PageCTA entity);

        // Collection mappings
        IEnumerable<ProjectViewModel> ToViewModels(IEnumerable<Project> entities);
        IEnumerable<ProjectSummaryViewModel> ToSummaryViewModels(IEnumerable<Project> entities);
        IEnumerable<SparkKitViewModel> ToViewModels(IEnumerable<SparkKit> entities);
        IEnumerable<SparkKitSummaryViewModel> ToSummaryViewModels(IEnumerable<SparkKit> entities);
        IEnumerable<BlogPostViewModel> ToViewModels(IEnumerable<BlogPost> entities);
        IEnumerable<BlogPostSummaryViewModel> ToSummaryViewModels(IEnumerable<BlogPost> entities);
        IEnumerable<TagViewModel> ToViewModels(IEnumerable<Tag> entities);
        IEnumerable<TagSummaryViewModel> ToSummaryViewModels(IEnumerable<Tag> entities);
    }
}
