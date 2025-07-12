# ViewModels and Mapping Service

This document describes the ViewModels and mapping service implementation for the GarageSpark application.

## Overview

The ViewModels provide a clean separation between the data layer (Entities) and the presentation layer (Views/Controllers). The mapping service handles the conversion between entities and ViewModels using manual mapping for better performance and control.

## Architecture

### ViewModels Structure

- **BaseViewModel**: Abstract base class containing common properties (Id, CreatedAt, UpdatedAt, etc.)
- **Entity ViewModels**: Full ViewModels for CRUD operations with validation attributes
- **Summary ViewModels**: Lightweight ViewModels for list displays and summaries
- **Selection ViewModels**: Specialized ViewModels for dropdowns and selection scenarios
- **Junction ViewModels**: ViewModels for many-to-many relationship entities

### Available ViewModels

#### Core Entities

- `ProjectViewModel` / `ProjectSummaryViewModel`
- `SparkKitViewModel` / `SparkKitSummaryViewModel`
- `BlogPostViewModel` / `BlogPostSummaryViewModel`
- `TagViewModel` / `TagSummaryViewModel` / `TagSelectionViewModel`
- `PageViewModel` / `PageSummaryViewModel`

#### Lookup Entities

- `ProjectStatusViewModel` / `ProjectStatusSummaryViewModel`
- `CTAViewModel` / `CTASummaryViewModel` / `CTASelectionViewModel`
- `IdeaPitchViewModel` / `IdeaPitchSummaryViewModel`
- `IdeaPitchStatusViewModel` / `IdeaPitchStatusSummaryViewModel`

#### Junction Entities

- `ProjectTagViewModel`
- `SparkKitTagViewModel`
- `BlogPostTagViewModel`
- `PageCTAViewModel`

## Mapping Service

### Interface: `IMappingService`

The mapping service provides methods for:

- Converting entities to ViewModels
- Converting ViewModels to entities
- Updating existing entities from ViewModels
- Converting entities to summary ViewModels
- Batch conversions for collections

### Key Methods

```csharp
// Basic conversions
TViewModel ToViewModel(TEntity entity);
TEntity ToEntity(TViewModel viewModel);
void UpdateEntity(TEntity entity, TViewModel viewModel);
TSummaryViewModel ToSummaryViewModel(TEntity entity);

// Collection conversions
IEnumerable<TViewModel> ToViewModels(IEnumerable<TEntity> entities);
IEnumerable<TSummaryViewModel> ToSummaryViewModels(IEnumerable<TEntity> entities);
```

## Usage Examples

### 1. Basic CRUD Operations

```csharp
public class ProjectController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMappingService _mappingService;

    public ProjectController(AppDbContext context, IMappingService mappingService)
    {
        _context = context;
        _mappingService = mappingService;
    }

    // GET: Projects
    public async Task<IActionResult> Index()
    {
        var projects = await _context.Projects
            .Include(p => p.Status)
            .Include(p => p.Tags)
            .ToListAsync();

        var viewModels = _mappingService.ToSummaryViewModels(projects);
        return View(viewModels);
    }

    // GET: Projects/Details/5
    public async Task<IActionResult> Details(Guid id)
    {
        var project = await _context.Projects
            .Include(p => p.Status)
            .Include(p => p.Tags)
            .Include(p => p.LinkedPitches)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        var viewModel = _mappingService.ToViewModel(project);
        return View(viewModel);
    }

    // POST: Projects/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProjectViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var entity = _mappingService.ToEntity(viewModel);
            entity.CreatedBy = User.Identity.Name;
            entity.ModifiedBy = User.Identity.Name;
            
            _context.Projects.Add(entity);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }
        
        return View(viewModel);
    }

    // POST: Projects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ProjectViewModel viewModel)
    {
        if (id != viewModel.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            var entity = await _context.Projects.FindAsync(id);
            if (entity == null)
                return NotFound();

            _mappingService.UpdateEntity(entity, viewModel);
            entity.ModifiedBy = User.Identity.Name;
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        return View(viewModel);
    }
}
```

### 2. Using the ProjectService

```csharp
public class ProjectController : Controller
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _projectService.GetAllProjectsAsync();
        return View(projects);
    }

    public async Task<IActionResult> Details(string slug)
    {
        var project = await _projectService.GetProjectBySlugAsync(slug);
        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _projectService.CreateProjectAsync(viewModel, User.Identity.Name);
            return RedirectToAction(nameof(Details), new { slug = result.Slug });
        }

        return View(viewModel);
    }
}
```

### 3. Working with Tags

```csharp
// In a service or controller
var tags = await _context.Tags.ToListAsync();
var tagSelectionViewModels = tags.Select(t => new TagSelectionViewModel
{
    Id = t.Id,
    Name = t.Name,
    ColorHex = t.ColorHex,
    IsSelected = selectedTagIds.Contains(t.Id)
}).ToList();
```

## Validation Features

### Built-in Validation Attributes

- **Required**: For mandatory fields
- **MaxLength**: For string length validation
- **Url**: For URL format validation
- **EmailAddress**: For email format validation
- **RegularExpression**: For pattern matching (slugs, hex colors)
- **Range**: For numeric range validation
- **Display**: For user-friendly labels

### Example Validation in ViewModels

```csharp
[Required(ErrorMessage = "Name is required")]
[MaxLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
[Display(Name = "Project Name")]
public string Name { get; set; } = string.Empty;

[Url(ErrorMessage = "Please enter a valid URL")]
[Display(Name = "Repository URL")]
public string RepoUrl { get; set; } = string.Empty;

[RegularExpression(@"^[a-z0-9-]+$", ErrorMessage = "Slug can only contain lowercase letters, numbers, and hyphens")]
public string Slug { get; set; } = string.Empty;
```

## Dependency Injection Registration

The mapping service is registered in `Program.cs`:

```csharp
// Register mapping services
builder.Services.AddMappingServices();
```

This extension method registers the `IMappingService` interface with the `MappingService` implementation as a scoped service.

## Best Practices

### 1. Always Include Navigation Properties

When loading entities for mapping, include the necessary navigation properties:

```csharp
var project = await _context.Projects
    .Include(p => p.Status)
    .Include(p => p.Tags)
    .Include(p => p.LinkedPitches)
    .FirstOrDefaultAsync(p => p.Id == id);
```

### 2. Use Summary ViewModels for Lists

For list views and performance-sensitive scenarios, use summary ViewModels:

```csharp
var projects = _mappingService.ToSummaryViewModels(entities);
```

### 3. Handle Null Checks

The mapping service includes null checks and returns appropriate defaults.

### 4. Audit Trail Management

Set audit fields appropriately:

```csharp
viewModel.CreatedBy = User.Identity.Name;
viewModel.ModifiedBy = User.Identity.Name;
```

### 5. Batch Operations

Use collection mapping methods for better performance:

```csharp
var viewModels = _mappingService.ToViewModels(entities);
```

## Extension Points

The mapping service can be extended by:

1. Adding new mapping methods to the `IMappingService` interface
2. Implementing the methods in the `MappingService` class
3. Creating specialized mapping services for complex scenarios
4. Adding custom validation attributes to ViewModels

## Error Handling

The mapping service throws `ArgumentNullException` for null inputs and handles navigation property null scenarios gracefully by returning empty collections or default values.
