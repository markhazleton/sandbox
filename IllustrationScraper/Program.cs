using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace IllustrationExtractor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string url = "https://storybook.bswsites.com/iframe.html?id=foundation-illustration--glossary&viewMode=story";
            string outputFileName = "illustrations.json";

            try
            {
                var illustrationNames = await ExtractIllustrationNamesWithSelenium(url);

                Console.WriteLine($"Found {illustrationNames.Count} unique illustration names:");
                Console.WriteLine();

                // Create JSON objects with the specified format
                var jsonObjects = new List<Dictionary<string, string>>();

                foreach (var name in illustrationNames.OrderBy(n => n))
                {
                    Console.WriteLine($"- {name}");

                    var illustrationObject = new Dictionary<string, string>
                    {
                        ["illustration_name"] = name,
                        ["png_url"] = $"https://bswcdndesign-prod.bswhealth.com/content/img/{name}.png"
                    };
                    jsonObjects.Add(illustrationObject);
                }

                // Save to JSON file
                await SaveToJsonFile(jsonObjects, outputFileName);

                Console.WriteLine($"\nSaved {jsonObjects.Count} illustrations to {outputFileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        static async Task<List<string>> ExtractIllustrationNamesWithSelenium(string url)
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless"); // Run without opening browser window
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            using (var driver = new ChromeDriver(options))
            {
                Console.WriteLine($"Loading page: {url}");

                // Navigate to the URL
                driver.Navigate().GoToUrl(url);

                // Wait for the page to load and JavaScript to execute
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

                // Wait for some content to be loaded (adjust selector as needed)
                try
                {
                    wait.Until(d => d.FindElements(By.TagName("body")).Count > 0);

                    // Additional wait for dynamic content - you might need to adjust this
                    // based on what specific elements indicate the illustrations are loaded
                    await Task.Delay(5000); // Wait 5 seconds for JS to fully execute

                    Console.WriteLine("Page loaded, extracting content...");
                }
                catch (WebDriverTimeoutException)
                {
                    Console.WriteLine("Warning: Page load timeout, proceeding anyway...");
                }

                // Get the fully rendered HTML
                string htmlContent = driver.PageSource;

                Console.WriteLine($"Retrieved {htmlContent.Length} characters of rendered HTML");

                // Extract illustration names using regex
                var illustrationNames = new HashSet<string>();

                // Pattern to match illustration="{name}"
                string pattern = @"illustration=""([^""]+)""";

                var matches = Regex.Matches(htmlContent, pattern, RegexOptions.IgnoreCase);

                Console.WriteLine($"Found {matches.Count} illustration assignments");

                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        string illustrationName = match.Groups[1].Value.Trim();
                        if (!string.IsNullOrEmpty(illustrationName))
                        {
                            illustrationNames.Add(illustrationName);
                        }
                    }
                }

                // Also try alternative patterns in case the format is different
                // Pattern for class names or data attributes that might contain illustration names
                var alternativePatterns = new[]
                {
                    @"class=""[^""]*illustration-([^""\s]+)",
                    @"data-illustration=""([^""]+)""",
                    @"illustration:\s*['""]([^'""]+)['""]"
                };

                foreach (var altPattern in alternativePatterns)
                {
                    var altMatches = Regex.Matches(htmlContent, altPattern, RegexOptions.IgnoreCase);
                    foreach (Match match in altMatches)
                    {
                        if (match.Groups.Count > 1)
                        {
                            string illustrationName = match.Groups[1].Value.Trim();
                            if (!string.IsNullOrEmpty(illustrationName))
                            {
                                illustrationNames.Add(illustrationName);
                            }
                        }
                    }
                }

                return illustrationNames.ToList();
            }
        }

        static async Task SaveToJsonFile(List<Dictionary<string, string>> data, string fileName)
        {
            try
            {
                // Create JSON with proper formatting
                var json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

                // Write to file
                await File.WriteAllTextAsync(fileName, json);

                Console.WriteLine($"JSON file saved successfully: {Path.GetFullPath(fileName)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving JSON file: {ex.Message}");
                throw;
            }
        }
    }
}