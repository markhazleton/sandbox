using GarageSpark.Services;
using GarageSpark.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GarageSpark.Controllers
{
    /// <summary>
    /// Blog controller for managing blog posts using the Blog CMS service
    /// Demonstrates Markdown + frontmatter workflow
    /// Restricted to authenticated users only
    /// </summary>
    [Authorize]
    public class BlogController : Controller
    {
        private readonly IBlogCmsService _blogCmsService;
        private readonly ILogger<BlogController> _logger;

        public BlogController(IBlogCmsService blogCmsService, ILogger<BlogController> logger)
        {
            _blogCmsService = blogCmsService ?? throw new ArgumentNullException(nameof(blogCmsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: Blog
        public async Task<IActionResult> Index(BlogPostSearchViewModel searchCriteria)
        {
            try
            {
                var result = await _blogCmsService.GetPostsAsync(searchCriteria);
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog posts");
                return View("Error");
            }
        }

        // GET: Blog/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                var blogPost = await _blogCmsService.GetPostByIdAsync(id);
                if (blogPost == null)
                {
                    return NotFound();
                }

                return View(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post {PostId}", id);
                return View("Error");
            }
        }

        // GET: Blog/Post/my-post-slug
        [Route("Blog/Post/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return NotFound();
            }

            try
            {
                var blogPost = await _blogCmsService.GetPostBySlugAsync(slug);
                if (blogPost == null)
                {
                    return NotFound();
                }

                // Redirect to canonical URL if this is a preview
                if (blogPost.IsPublished)
                {
                    return View("Details", blogPost);
                }

                // Only show unpublished posts to authenticated users
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return NotFound();
                }

                return View("Details", blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post with slug {Slug}", slug);
                return View("Error");
            }
        }

        // GET: Blog/Create
        public IActionResult Create()
        {
            var model = new BlogPostFormViewModel
            {
                // Set some example frontmatter content
                Content = @"---
title: ""My New Blog Post""
description: ""A sample description for SEO""
tags: [""development"", ""coding""]
author: ""Your Name""
date: " + DateTime.UtcNow.ToString("yyyy-MM-dd") + @"
---

# My New Blog Post

Write your **Markdown** content here!

## Features

- Supports frontmatter for metadata
- Automatic HTML generation
- SEO optimization
- Tag management

[Learn more about Markdown](https://www.markdownguide.org/)"
            };

            return View(model);
        }

        // POST: Blog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPostFormViewModel model)
        {
            // Convert comma-separated media file IDs to List<Guid>
            if (!string.IsNullOrWhiteSpace(model.SelectedMediaFileIdsString))
            {
                _logger.LogInformation("Raw media file IDs string: {Value}", model.SelectedMediaFileIdsString);
                try
                {
                    model.SelectedMediaFileIds = model.SelectedMediaFileIdsString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => Guid.Parse(id.Trim()))
                        .Distinct() // Remove duplicates
                        .ToList();
                    _logger.LogInformation("Parsed {Count} unique media file IDs: {Ids}",
                        model.SelectedMediaFileIds.Count,
                        string.Join(", ", model.SelectedMediaFileIds));
                }
                catch (FormatException)
                {
                    ModelState.AddModelError(nameof(model.SelectedMediaFileIds), "Invalid media file selection.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Validation error: {ErrorMessage}", error.ErrorMessage);
                }
                return View(model);
            }

            try
            {
                var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                var createdPost = await _blogCmsService.CreatePostAsync(model, currentUser);

                _logger.LogInformation("Blog post created successfully: {PostId}", createdPost.Id);

                return RedirectToAction(nameof(Details), new { id = createdPost.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating blog post");
                ModelState.AddModelError("", "An error occurred while creating the blog post. Please try again.");
                return View(model);
            }
        }

        // GET: Blog/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                var blogPost = await _blogCmsService.GetPostByIdAsync(id);
                if (blogPost == null)
                {
                    return NotFound();
                }

                var formModel = new BlogPostFormViewModel
                {
                    Title = blogPost.Title,
                    Excerpt = blogPost.Excerpt,
                    Content = blogPost.Content,
                    CoverMediaFileId = blogPost.CoverMediaFileId,
                    Slug = blogPost.Slug,
                    TagNames = string.Join(", ", blogPost.Tags.Select(t => t.Name))
                };

                return View(formModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post for editing {PostId}", id);
                return View("Error");
            }
        }

        // POST: Blog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, BlogPostFormViewModel model)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            // Convert comma-separated media file IDs to List<Guid>
            if (!string.IsNullOrWhiteSpace(model.SelectedMediaFileIdsString))
            {
                try
                {
                    model.SelectedMediaFileIds = model.SelectedMediaFileIdsString
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(id => Guid.Parse(id.Trim()))
                        .Distinct() // Remove duplicates
                        .ToList();
                }
                catch (FormatException)
                {
                    _logger.LogWarning("Invalid media file ID format in SelectedMediaFileIdsString: {Value}", model.SelectedMediaFileIdsString);
                    ModelState.AddModelError(nameof(model.SelectedMediaFileIdsString), "Invalid media file selection format.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning("Edit validation error: {Error}", error.ErrorMessage);
                }
                return View(model);
            }

            try
            {
                var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                var updatedPost = await _blogCmsService.UpdatePostAsync(id, model, currentUser);

                if (updatedPost == null)
                {
                    return NotFound();
                }

                _logger.LogInformation("Blog post updated successfully: {PostId}", id);

                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blog post {PostId}", id);
                ModelState.AddModelError("", "An error occurred while updating the blog post. Please try again.");
                return View(model);
            }
        }

        // GET: Blog/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                var blogPost = await _blogCmsService.GetPostByIdAsync(id);
                if (blogPost == null)
                {
                    return NotFound();
                }

                return View(blogPost);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog post for deletion {PostId}", id);
                return View("Error");
            }
        }

        // POST: Blog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var success = await _blogCmsService.DeletePostAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                _logger.LogInformation("Blog post deleted successfully: {PostId}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog post {PostId}", id);
                return View("Error");
            }
        }

        // POST: Blog/Publish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(Guid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                var publishedPost = await _blogCmsService.PublishPostAsync(id, currentUser);

                if (publishedPost == null)
                {
                    return NotFound();
                }

                _logger.LogInformation("Blog post published successfully: {PostId}", id);

                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing blog post {PostId}", id);
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // POST: Blog/Unpublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(Guid id)
        {
            try
            {
                var currentUser = User.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                var unpublishedPost = await _blogCmsService.UnpublishPostAsync(id, currentUser);

                if (unpublishedPost == null)
                {
                    return NotFound();
                }

                _logger.LogInformation("Blog post unpublished successfully: {PostId}", id);

                return RedirectToAction(nameof(Details), new { id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unpublishing blog post {PostId}", id);
                return RedirectToAction(nameof(Details), new { id = id });
            }
        }

        // GET: Blog/Analytics
        public async Task<IActionResult> Analytics()
        {
            try
            {
                var analytics = await _blogCmsService.GetAnalyticsAsync();
                return View(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog analytics");
                return View("Error");
            }
        }
    }
}
