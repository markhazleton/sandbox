using GarageSpark.Data;
using GarageSpark.Data.Entities;
using GarageSpark.Services.Mapping;
using GarageSpark.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GarageSpark.Services
{
    /// <summary>
    /// Example service demonstrating how to use the mapping service for CRUD operations
    /// </summary>
    public class ProjectService
    {
        private readonly AppDbContext _context;
        private readonly IMappingService _mappingService;

        public ProjectService(AppDbContext context, IMappingService mappingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
        }

        /// <summary>
        /// Gets all projects as view models
        /// </summary>
        public async Task<IEnumerable<ProjectSummaryViewModel>> GetAllProjectsAsync()
        {
            var projects = await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.Tags)
                .OrderBy(p => p.Order)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(projects);
        }

        /// <summary>
        /// Gets a project by ID as a view model
        /// </summary>
        public async Task<ProjectViewModel?> GetProjectByIdAsync(Guid id)
        {
            var project = await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.Tags)
                .Include(p => p.LinkedPitches)
                .FirstOrDefaultAsync(p => p.Id == id);

            return project != null ? _mappingService.ToViewModel(project) : null;
        }

        /// <summary>
        /// Gets a project by slug as a view model
        /// </summary>
        public async Task<ProjectViewModel?> GetProjectBySlugAsync(string slug)
        {
            var project = await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.Tags)
                .Include(p => p.LinkedPitches)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            return project != null ? _mappingService.ToViewModel(project) : null;
        }

        /// <summary>
        /// Creates a new project from a view model
        /// </summary>
        public async Task<ProjectViewModel> CreateProjectAsync(ProjectViewModel viewModel, string currentUser)
        {
            // Set audit fields
            viewModel.CreatedBy = currentUser;
            viewModel.ModifiedBy = currentUser;
            viewModel.CreatedAt = DateTime.UtcNow;
            viewModel.UpdatedAt = DateTime.UtcNow;

            // Convert to entity
            var entity = _mappingService.ToEntity(viewModel);

            // Add to context and save
            _context.Projects.Add(entity);
            await _context.SaveChangesAsync();

            // Handle tag associations if needed
            if (viewModel.SelectedTagIds.Any())
            {
                await UpdateProjectTagsAsync(entity.Id, viewModel.SelectedTagIds);
            }

            // Return the created project as view model
            return await GetProjectByIdAsync(entity.Id) ?? viewModel;
        }

        /// <summary>
        /// Updates an existing project from a view model
        /// </summary>
        public async Task<ProjectViewModel?> UpdateProjectAsync(Guid id, ProjectViewModel viewModel, string currentUser)
        {
            var existingEntity = await _context.Projects
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingEntity == null)
                return null;

            // Set audit fields
            viewModel.ModifiedBy = currentUser;

            // Update the entity
            _mappingService.UpdateEntity(existingEntity, viewModel);

            // Handle tag associations
            await UpdateProjectTagsAsync(existingEntity.Id, viewModel.SelectedTagIds);

            await _context.SaveChangesAsync();

            // Return the updated project as view model
            return await GetProjectByIdAsync(id);
        }

        /// <summary>
        /// Deletes a project by ID
        /// </summary>
        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Updates project tag associations
        /// </summary>
        private async Task UpdateProjectTagsAsync(Guid projectId, List<Guid> selectedTagIds)
        {
            // Remove existing associations
            var existingTags = await _context.ProjectTags
                .Where(pt => pt.ProjectId == projectId)
                .ToListAsync();

            _context.ProjectTags.RemoveRange(existingTags);

            // Add new associations
            foreach (var tagId in selectedTagIds)
            {
                _context.ProjectTags.Add(new ProjectTag
                {
                    ProjectId = projectId,
                    TagId = tagId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Gets projects by tag
        /// </summary>
        public async Task<IEnumerable<ProjectSummaryViewModel>> GetProjectsByTagAsync(string tagSlug)
        {
            var projects = await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.Tags)
                .Where(p => p.Tags.Any(t => t.Slug == tagSlug))
                .OrderBy(p => p.Order)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(projects);
        }

        /// <summary>
        /// Gets projects by status
        /// </summary>
        public async Task<IEnumerable<ProjectSummaryViewModel>> GetProjectsByStatusAsync(Guid statusId)
        {
            var projects = await _context.Projects
                .Include(p => p.Status)
                .Include(p => p.Tags)
                .Where(p => p.StatusId == statusId)
                .OrderBy(p => p.Order)
                .ToListAsync();

            return _mappingService.ToSummaryViewModels(projects);
        }
    }
}
