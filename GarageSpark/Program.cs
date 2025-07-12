using GarageSpark.Data;
using GarageSpark.Extensions;
using GarageSpark.Services;
using GarageSpark.Services.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebSpark.Bootswatch;
using WebSpark.HttpClientUtility.ClientService;
using WebSpark.HttpClientUtility.RequestResult;
using WebSpark.HttpClientUtility.StringConverter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AppDbContext for GarageSpark entities
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Email confirmation settings
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add HttpContextAccessor for Bootswatch Tag Helper
builder.Services.AddHttpContextAccessor();

// Add Bootswatch theme switcher services (includes StyleCache)
builder.Services.AddBootswatchThemeSwitcher();

// Add HttpClientFactory (essential for managing HttpClient instances)
builder.Services.AddHttpClient();

// Add memory cache for caching decorator
builder.Services.AddMemoryCache();

// Register the core HttpClient service and its dependencies
// Choose System.Text.Json as the JSON serializer
builder.Services.AddSingleton<IStringConverter, SystemJsonStringConverter>();

// Register the basic service implementation
builder.Services.AddScoped<IHttpClientService, HttpClientService>();

// Register the HttpRequestResult service stack (using decorators)
builder.Services.AddScoped<HttpRequestResultService>(); // Base service - always register

// Register the final IHttpRequestResultService using a factory to build the decorator chain
builder.Services.AddScoped<IHttpRequestResultService>(provider =>
{
    // Start with the base service instance
    IHttpRequestResultService service = provider.GetRequiredService<HttpRequestResultService>();

    // Return the basic service for now (we can add decorators later)
    return service;
});

// Register our custom API service
builder.Services.AddScoped<ApiService>();

// Register mapping services
builder.Services.AddMappingServices();

// Register Markdown service
builder.Services.AddScoped<IMarkdownService, MarkdownService>();

// Register Blog CMS service
builder.Services.AddScoped<IBlogCmsService, BlogCmsService>();

// Register Media services
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddHttpClient<IUnsplashService, UnsplashService>();

// Register example service
builder.Services.AddScoped<ProjectService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Use all Bootswatch features (includes StyleCache and static files)
app.UseBootswatchAll();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();


app.Run();
