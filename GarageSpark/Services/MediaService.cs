using GarageSpark.Data;
using GarageSpark.Data.Entities;
using GarageSpark.Services.Mapping;
using GarageSpark.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GarageSpark.Services
{
    /// <summary>
    /// Service interface for media file operations
    /// </summary>
    public interface IMediaService
    {
        Task<MediaFileViewModel> UploadFileAsync(MediaUploadViewModel model, string userId);
        Task<MediaFileViewModel> SaveFromUnsplashAsync(UnsplashSearchResultViewModel unsplashResult, string userId);
        Task<MediaPagedResultViewModel> GetMediaFilesAsync(MediaSearchViewModel searchModel);
        Task<MediaFileViewModel?> GetMediaFileAsync(Guid id);
        Task<bool> DeleteMediaFileAsync(Guid id);
        Task<MediaFileViewModel> UpdateMediaFileAsync(Guid id, MediaFileViewModel model);
        Task<MediaLibraryStatsViewModel> GetStatsAsync();
    }

    /// <summary>
    /// Service for handling media file operations including uploads and Unsplash integration
    /// </summary>
    public class MediaService : IMediaService
    {
        private readonly AppDbContext _context;
        private readonly IMappingService _mappingService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MediaService> _logger;
        private readonly string _uploadsPath;

        public MediaService(
            AppDbContext context,
            IMappingService mappingService,
            IWebHostEnvironment environment,
            ILogger<MediaService> logger)
        {
            _context = context;
            _mappingService = mappingService;
            _environment = environment;
            _logger = logger;
            _uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", "media");

            // Ensure uploads directory exists
            Directory.CreateDirectory(_uploadsPath);
        }

        public async Task<MediaFileViewModel> UploadFileAsync(MediaUploadViewModel model, string userId)
        {
            try
            {
                _logger.LogInformation("Starting file upload for user {UserId}", userId);

                // Validate file
                ValidateFile(model.File);

                // Generate unique filename
                var fileExtension = Path.GetExtension(model.File.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(_uploadsPath, fileName);

                // Save file to disk
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(stream);
                }

                // Get image dimensions if it's an image
                var (width, height) = await GetImageDimensionsAsync(filePath, model.File.ContentType);

                // Create media file entity
                var mediaFile = new MediaFile
                {
                    Id = Guid.NewGuid(),
                    FileName = fileName,
                    OriginalFileName = model.File.FileName,
                    FilePath = filePath,
                    Url = $"/uploads/media/{fileName}",
                    ContentType = model.File.ContentType,
                    FileSize = model.File.Length,
                    Width = width,
                    Height = height,
                    Alt = model.Alt,
                    Caption = model.Caption,
                    Description = model.Description,
                    Source = MediaSource.Local,
                    IsPublic = model.IsPublic,
                    CreatedBy = userId,
                    ModifiedBy = userId
                };

                _context.MediaFiles.Add(mediaFile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("File uploaded successfully: {FileName}", fileName);

                return _mappingService.ToViewModel(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file for user {UserId}", userId);
                throw;
            }
        }

        public async Task<MediaFileViewModel> SaveFromUnsplashAsync(UnsplashSearchResultViewModel unsplashResult, string userId)
        {
            try
            {
                _logger.LogInformation("Saving Unsplash image {ImageId} for user {UserId}", unsplashResult.Id, userId);

                // Check if we already have this Unsplash image
                var existingMedia = await _context.MediaFiles
                    .FirstOrDefaultAsync(m => m.Source == MediaSource.Unsplash && m.ExternalId == unsplashResult.Id);

                if (existingMedia != null)
                {
                    return _mappingService.ToViewModel(existingMedia);
                }

                // Create media file entity from Unsplash data
                var mediaFile = new MediaFile
                {
                    Id = Guid.NewGuid(),
                    FileName = $"unsplash-{unsplashResult.Id}.jpg",
                    OriginalFileName = $"unsplash-{unsplashResult.Id}.jpg",
                    FilePath = string.Empty, // No local file path for Unsplash images
                    Url = unsplashResult.Urls.Regular,
                    ContentType = "image/jpeg",
                    FileSize = 0, // Unknown for Unsplash images
                    Width = unsplashResult.Width,
                    Height = unsplashResult.Height,
                    Alt = unsplashResult.AltDescription ?? unsplashResult.Description,
                    Caption = unsplashResult.Description,
                    Description = unsplashResult.Description,
                    Source = MediaSource.Unsplash,
                    ExternalId = unsplashResult.Id,
                    ExternalUrl = unsplashResult.Urls.Regular,
                    ExternalAuthor = unsplashResult.User.Name,
                    ExternalAuthorUrl = unsplashResult.User.Links.Html,
                    ExternalSource = "Unsplash",
                    IsPublic = true,
                    CreatedBy = userId,
                    ModifiedBy = userId
                };

                _context.MediaFiles.Add(mediaFile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Unsplash image saved successfully: {ImageId}", unsplashResult.Id);

                return _mappingService.ToViewModel(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Unsplash image {ImageId} for user {UserId}", unsplashResult.Id, userId);
                throw;
            }
        }

        public async Task<MediaPagedResultViewModel> GetMediaFilesAsync(MediaSearchViewModel searchModel)
        {
            try
            {
                var query = _context.MediaFiles.AsQueryable();

                // Apply search filters
                if (!string.IsNullOrWhiteSpace(searchModel.SearchTerm))
                {
                    var searchTerm = searchModel.SearchTerm.ToLower();
                    query = query.Where(m =>
                        m.FileName.ToLower().Contains(searchTerm) ||
                        m.OriginalFileName.ToLower().Contains(searchTerm) ||
                        m.Alt.ToLower().Contains(searchTerm) ||
                        m.Caption.ToLower().Contains(searchTerm) ||
                        m.Description.ToLower().Contains(searchTerm));
                }

                if (searchModel.FileType.HasValue)
                {
                    query = searchModel.FileType.Value switch
                    {
                        MediaFileType.Image => query.Where(m => m.ContentType.StartsWith("image/")),
                        MediaFileType.Video => query.Where(m => m.ContentType.StartsWith("video/")),
                        MediaFileType.Document => query.Where(m => !m.ContentType.StartsWith("image/") && !m.ContentType.StartsWith("video/")),
                        _ => query
                    };
                }

                if (searchModel.Source.HasValue)
                {
                    query = query.Where(m => m.Source == searchModel.Source.Value);
                }

                if (searchModel.FromDate.HasValue)
                {
                    query = query.Where(m => m.CreatedAt >= searchModel.FromDate.Value);
                }

                if (searchModel.ToDate.HasValue)
                {
                    query = query.Where(m => m.CreatedAt <= searchModel.ToDate.Value.AddDays(1));
                }

                // Apply sorting
                query = searchModel.SortBy switch
                {
                    MediaSortBy.NameAsc => query.OrderBy(m => m.FileName),
                    MediaSortBy.NameDesc => query.OrderByDescending(m => m.FileName),
                    MediaSortBy.CreatedAsc => query.OrderBy(m => m.CreatedAt),
                    MediaSortBy.CreatedDesc => query.OrderByDescending(m => m.CreatedAt),
                    MediaSortBy.SizeAsc => query.OrderBy(m => m.FileSize),
                    MediaSortBy.SizeDesc => query.OrderByDescending(m => m.FileSize),
                    MediaSortBy.TypeAsc => query.OrderBy(m => m.ContentType),
                    MediaSortBy.TypeDesc => query.OrderByDescending(m => m.ContentType),
                    _ => query.OrderByDescending(m => m.CreatedAt)
                };

                var totalCount = await query.CountAsync();

                var mediaFiles = await query
                    .Skip((searchModel.Page - 1) * searchModel.PageSize)
                    .Take(searchModel.PageSize)
                    .ToListAsync();

                var mediaFileViewModels = mediaFiles
                    .Select(m => _mappingService.ToViewModel(m))
                    .ToList();

                return new MediaPagedResultViewModel
                {
                    MediaFiles = mediaFileViewModels,
                    TotalCount = totalCount,
                    Page = searchModel.Page,
                    PageSize = searchModel.PageSize,
                    SearchCriteria = searchModel
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media files");
                throw;
            }
        }

        public async Task<MediaFileViewModel?> GetMediaFileAsync(Guid id)
        {
            try
            {
                var mediaFile = await _context.MediaFiles.FindAsync(id);
                return mediaFile != null ? _mappingService.ToViewModel(mediaFile) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media file {MediaFileId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteMediaFileAsync(Guid id)
        {
            try
            {
                var mediaFile = await _context.MediaFiles.FindAsync(id);
                if (mediaFile == null)
                    return false;

                // Delete physical file for local media
                if (mediaFile.Source == MediaSource.Local && !string.IsNullOrEmpty(mediaFile.FilePath) && File.Exists(mediaFile.FilePath))
                {
                    File.Delete(mediaFile.FilePath);
                }

                _context.MediaFiles.Remove(mediaFile);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Media file deleted: {MediaFileId}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media file {MediaFileId}", id);
                throw;
            }
        }

        public async Task<MediaFileViewModel> UpdateMediaFileAsync(Guid id, MediaFileViewModel model)
        {
            try
            {
                var mediaFile = await _context.MediaFiles.FindAsync(id);
                if (mediaFile == null)
                    throw new ArgumentException("Media file not found", nameof(id));

                // Update editable properties
                mediaFile.Alt = model.Alt;
                mediaFile.Caption = model.Caption;
                mediaFile.Description = model.Description;
                mediaFile.IsPublic = model.IsPublic;
                mediaFile.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Media file updated: {MediaFileId}", id);

                return _mappingService.ToViewModel(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating media file {MediaFileId}", id);
                throw;
            }
        }

        public async Task<MediaLibraryStatsViewModel> GetStatsAsync()
        {
            try
            {
                var stats = await _context.MediaFiles
                    .GroupBy(m => 1)
                    .Select(g => new MediaLibraryStatsViewModel
                    {
                        TotalFiles = g.Count(),
                        Images = g.Count(m => m.ContentType.StartsWith("image/")),
                        Videos = g.Count(m => m.ContentType.StartsWith("video/")),
                        Documents = g.Count(m => !m.ContentType.StartsWith("image/") && !m.ContentType.StartsWith("video/")),
                        TotalSize = g.Sum(m => m.FileSize),
                        LocalFiles = g.Count(m => m.Source == MediaSource.Local),
                        UnsplashFiles = g.Count(m => m.Source == MediaSource.Unsplash),
                        ExternalFiles = g.Count(m => m.Source == MediaSource.External)
                    })
                    .FirstOrDefaultAsync();

                return stats ?? new MediaLibraryStatsViewModel();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media library stats");
                throw;
            }
        }

        private static void ValidateFile(IFormFile file)
        {
            // Check file size (max 10MB)
            const long maxSize = 10 * 1024 * 1024; // 10MB
            if (file.Length > maxSize)
            {
                throw new ArgumentException("File size cannot exceed 10MB");
            }

            // Check allowed file types
            var allowedTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml",
                "video/mp4", "video/webm", "video/ogg",
                "application/pdf", "text/plain", "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                throw new ArgumentException($"File type '{file.ContentType}' is not allowed");
            }
        }

        private static async Task<(int? width, int? height)> GetImageDimensionsAsync(string filePath, string contentType)
        {
            if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                return (null, null);

            try
            {
                // For now, return null - in a real implementation, you'd use ImageSharp or similar
                // to get actual image dimensions
                await Task.CompletedTask;
                return (null, null);
            }
            catch
            {
                return (null, null);
            }
        }
    }
}
