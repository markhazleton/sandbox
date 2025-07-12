using GarageSpark.Models;
using GarageSpark.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebSpark.Bootswatch.Helpers;
using WebSpark.Bootswatch.Services;

namespace GarageSpark.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiService _apiService;
        private readonly StyleCache _styleCache;

        public HomeController(ILogger<HomeController> logger, ApiService apiService, StyleCache styleCache)
        {
            _logger = logger;
            _apiService = apiService;
            _styleCache = styleCache;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Weather(string city = "London")
        {
            try
            {
                var weatherData = await _apiService.GetWeatherAsync(city);
                ViewBag.City = city;
                ViewBag.WeatherData = weatherData;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting weather data for {City}", city);
                ViewBag.Error = "Unable to retrieve weather data.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Posts()
        {
            // Redirect to the new Blog controller if user is authenticated
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Blog");
            }

            // For unauthenticated users, show a message or redirect to login
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Dashboard()
        {
            ViewBag.UserEmail = User.Identity?.Name;
            return View();
        }

        [HttpGet]
        public IActionResult Themes()
        {
            try
            {
                var allStyles = _styleCache.GetAllStyles();
                var currentTheme = BootswatchThemeHelper.GetCurrentThemeName(HttpContext);
                var currentColorMode = BootswatchThemeHelper.GetCurrentColorMode(HttpContext);

                ViewBag.AllStyles = allStyles;
                ViewBag.CurrentTheme = currentTheme;
                ViewBag.CurrentColorMode = currentColorMode;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting theme information");
                ViewBag.Error = "Unable to retrieve theme information.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromBody] PostData post)
        {
            try
            {
                var success = await _apiService.CreatePostAsync(post);
                return Json(new { success = success, message = success ? "Post created successfully" : "Failed to create post" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post");
                return Json(new { success = false, message = "Error creating post" });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
