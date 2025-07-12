using GarageSpark.Data.Entities;
using GarageSpark.Services;
using GarageSpark.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GarageSpark.Controllers
{
    /// <summary>
    /// Controller for media management operations
    /// </summary>
    [Authorize]
    public class MediaController : Controller
    {
        private readonly IMediaService _mediaService;
        private readonly IUnsplashService _unsplashService;
        private readonly ILogger<MediaController> _logger;

        public MediaController(
            IMediaService mediaService,
            IUnsplashService unsplashService,
            ILogger<MediaController> logger)
        {
            _mediaService = mediaService;
            _unsplashService = unsplashService;
            _logger = logger;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";
        }

        // GET: Media
        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchTerm = null,
            string? source = null,
            int page = 1)
        {
            try
            {
                MediaSource? sourceEnum = null;
                if (!string.IsNullOrEmpty(source) && Enum.TryParse<MediaSource>(source, out var parsedSource))
                {
                    sourceEnum = parsedSource;
                }

                var searchModel = new MediaSearchViewModel
                {
                    SearchTerm = searchTerm ?? string.Empty,
                    Source = sourceEnum,
                    Page = page,
                    PageSize = 12
                };

                var result = await _mediaService.GetMediaFilesAsync(searchModel);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_MediaGrid", result.MediaFiles);
                }

                return View(result.MediaFiles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading media library");
                TempData["Error"] = "Failed to load media library. Please try again.";
                return View(new List<MediaFileViewModel>());
            }
        }

        // GET: Media/Upload
        [HttpGet]
        public IActionResult Upload()
        {
            return View(new MediaUploadViewModel());
        }

        // POST: Media/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(MediaUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = GetCurrentUserId();
                var result = await _mediaService.UploadFileAsync(model, userId);

                TempData["Success"] = "File uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                ModelState.AddModelError("", "An error occurred while uploading the file. Please try again.");
                return View(model);
            }
        }

        // GET: Media/Unsplash
        [HttpGet]
        public IActionResult Unsplash(string? query = null, int page = 1)
        {
            var model = new MediaSearchViewModel
            {
                SearchTerm = query ?? string.Empty,
                Page = page
            };

            return View(model);
        }

        // POST: Media/SearchUnsplash
        [HttpPost]
        public async Task<IActionResult> SearchUnsplash([FromBody] MediaSearchViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.SearchTerm))
                {
                    return Json(new { success = false, error = "Search term is required" });
                }

                var results = await _unsplashService.SearchPhotosAsync(
                    model.SearchTerm,
                    model.Page,
                    model.PageSize);

                return Json(new { success = true, data = results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching Unsplash");
                return Json(new { success = false, error = "Failed to search images. Please try again." });
            }
        }

        // POST: Media/SaveFromUnsplash
        [HttpPost]
        public async Task<IActionResult> SaveFromUnsplash([FromBody] UnsplashSaveRequestViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.PhotoId))
                {
                    return Json(new { success = false, error = "Photo ID is required" });
                }

                // Get the photo details from Unsplash
                var photo = await _unsplashService.GetPhotoAsync(model.PhotoId);
                if (photo == null)
                {
                    return Json(new { success = false, error = "Photo not found on Unsplash" });
                }

                var userId = GetCurrentUserId();
                var result = await _mediaService.SaveFromUnsplashAsync(photo, userId);

                // Trigger download tracking
                await _unsplashService.TriggerDownloadAsync(model.PhotoId);

                return Json(new { success = true, mediaId = result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image from Unsplash");
                return Json(new { success = false, error = "Failed to save image. Please try again." });
            }
        }

        // GET: Media/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var mediaFile = await _mediaService.GetMediaFileAsync(id);

                if (mediaFile == null)
                {
                    TempData["Error"] = "Media file not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading media file details for ID: {MediaId}", id);
                TempData["Error"] = "Failed to load media file details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Media/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var mediaFile = await _mediaService.GetMediaFileAsync(id);

                if (mediaFile == null)
                {
                    TempData["Error"] = "Media file not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading media file for editing, ID: {MediaId}", id);
                TempData["Error"] = "Failed to load media file for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Media/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, MediaFileViewModel model)
        {
            if (id != model.Id)
            {
                TempData["Error"] = "Invalid media file ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _mediaService.UpdateMediaFileAsync(id, model);

                TempData["Success"] = "Media file updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating media file, ID: {MediaId}", id);
                ModelState.AddModelError("", "An error occurred while updating the media file. Please try again.");
                return View(model);
            }
        }

        // POST: Media/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _mediaService.DeleteMediaFileAsync(id);

                if (result)
                {
                    TempData["Success"] = "Media file deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to delete media file.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting media file, ID: {MediaId}", id);
                TempData["Error"] = "Failed to delete media file. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Media/Stats
        [HttpGet]
        public async Task<IActionResult> Stats()
        {
            try
            {
                var stats = await _mediaService.GetStatsAsync();
                return View(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading media library statistics");
                TempData["Error"] = "Failed to load statistics.";
                return RedirectToAction(nameof(Index));
            }
        }

        #region API Endpoints for Media Selection

        /// <summary>
        /// API endpoint to get media files for picker
        /// </summary>
        [HttpGet]
        [Route("api/media")]
        public async Task<IActionResult> GetMediaFiles(
            string? searchTerm = null,
            string? source = null,
            int page = 1,
            int pageSize = 20)
        {
            try
            {
                MediaSource? sourceEnum = null;
                if (!string.IsNullOrEmpty(source) && Enum.TryParse<MediaSource>(source, out var parsedSource))
                {
                    sourceEnum = parsedSource;
                }

                var searchModel = new MediaSearchViewModel
                {
                    SearchTerm = searchTerm ?? string.Empty,
                    Source = sourceEnum,
                    Page = page,
                    PageSize = pageSize
                };

                var result = await _mediaService.GetMediaFilesAsync(searchModel);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media files for API");
                return StatusCode(500, new { error = "Failed to retrieve media files" });
            }
        }

        /// <summary>
        /// API endpoint to get a specific media file by ID
        /// </summary>
        [HttpGet]
        [Route("api/media/{id:guid}")]
        public async Task<IActionResult> GetMediaFile(Guid id)
        {
            try
            {
                var mediaFile = await _mediaService.GetMediaFileAsync(id);
                if (mediaFile == null)
                {
                    return NotFound(new { error = "Media file not found" });
                }

                return Json(mediaFile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving media file {MediaFileId}", id);
                return StatusCode(500, new { error = "Failed to retrieve media file" });
            }
        }

        #endregion
    }

    /// <summary>
    /// View model for Unsplash save requests
    /// </summary>
    public class UnsplashSaveRequestViewModel
    {
        public string PhotoId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
