using Markdig;
using System.Text.RegularExpressions;

namespace GarageSpark.Services
{
    /// <summary>
    /// Service for processing Markdown content with frontmatter support
    /// </summary>
    public interface IMarkdownService
    {
        /// <summary>
        /// Converts Markdown content to HTML
        /// </summary>
        string ToHtml(string markdown);

        /// <summary>
        /// Extracts frontmatter from Markdown content
        /// </summary>
        FrontmatterResult ExtractFrontmatter(string content);

        /// <summary>
        /// Processes content for safe HTML output
        /// </summary>
        string SanitizeHtml(string html);

        /// <summary>
        /// Extracts excerpt from content
        /// </summary>
        string ExtractExcerpt(string content, int maxLength = 300);

        /// <summary>
        /// Generates table of contents from content
        /// </summary>
        List<TocItem> GenerateTableOfContents(string markdown);
    }

    public class MarkdownService : IMarkdownService
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdownService()
        {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseEmojiAndSmiley()
                .UseSoftlineBreakAsHardlineBreak()
                .Build();
        }

        public string ToHtml(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return string.Empty;

            return Markdown.ToHtml(markdown, _pipeline);
        }

        public FrontmatterResult ExtractFrontmatter(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new FrontmatterResult { Content = content, Frontmatter = new Dictionary<string, object>() };

            // Check for YAML frontmatter
            var frontmatterMatch = Regex.Match(content, @"^---\s*\n(.*?)\n---\s*\n(.*)", RegexOptions.Singleline | RegexOptions.Multiline);

            if (!frontmatterMatch.Success)
                return new FrontmatterResult { Content = content, Frontmatter = new Dictionary<string, object>() };

            var frontmatterYaml = frontmatterMatch.Groups[1].Value;
            var contentWithoutFrontmatter = frontmatterMatch.Groups[2].Value;

            var frontmatter = ParseSimpleYaml(frontmatterYaml);

            return new FrontmatterResult
            {
                Content = contentWithoutFrontmatter,
                Frontmatter = frontmatter
            };
        }

        public string SanitizeHtml(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            // Basic HTML sanitization - remove script tags and dangerous attributes
            html = Regex.Replace(html, @"<script[^>]*>.*?</script>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = Regex.Replace(html, @"<iframe[^>]*>.*?</iframe>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            html = Regex.Replace(html, @"on\w+\s*=\s*[""'][^""']*[""']", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"javascript:", "", RegexOptions.IgnoreCase);

            return html;
        }

        public string ExtractExcerpt(string content, int maxLength = 300)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            // Remove frontmatter if present
            var result = ExtractFrontmatter(content);
            var cleanContent = result.Content;

            // Convert to HTML to process markdown
            var html = ToHtml(cleanContent);

            // Strip HTML tags
            var plainText = Regex.Replace(html, @"<[^>]+>", " ");
            plainText = Regex.Replace(plainText, @"\s+", " ").Trim();

            if (plainText.Length <= maxLength)
                return plainText;

            // Find the last complete word within the limit
            var excerpt = plainText.Substring(0, maxLength);
            var lastSpace = excerpt.LastIndexOf(' ');

            if (lastSpace > 0)
                excerpt = excerpt.Substring(0, lastSpace);

            return excerpt + "...";
        }

        public List<TocItem> GenerateTableOfContents(string markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return new List<TocItem>();

            var toc = new List<TocItem>();
            var lines = markdown.Split('\n');

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^(#{1,6})\s+(.+)$");
                if (match.Success)
                {
                    var level = match.Groups[1].Value.Length;
                    var title = match.Groups[2].Value.Trim();
                    var anchor = GenerateAnchor(title);

                    toc.Add(new TocItem
                    {
                        Level = level,
                        Title = title,
                        Anchor = anchor
                    });
                }
            }

            return toc;
        }

        private static Dictionary<string, object> ParseSimpleYaml(string yaml)
        {
            var result = new Dictionary<string, object>();

            if (string.IsNullOrWhiteSpace(yaml))
                return result;

            var lines = yaml.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^\s*([^:]+):\s*(.*)$");
                if (match.Success)
                {
                    var key = match.Groups[1].Value.Trim();
                    var value = match.Groups[2].Value.Trim();

                    // Remove quotes if present
                    if ((value.StartsWith('"') && value.EndsWith('"')) ||
                        (value.StartsWith('\'') && value.EndsWith('\'')))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    // Try to parse as different types
                    if (bool.TryParse(value, out var boolValue))
                        result[key] = boolValue;
                    else if (int.TryParse(value, out var intValue))
                        result[key] = intValue;
                    else if (DateTime.TryParse(value, out var dateValue))
                        result[key] = dateValue;
                    else
                        result[key] = value;
                }
            }

            return result;
        }

        private static string GenerateAnchor(string title)
        {
            // Convert title to URL-friendly anchor
            var anchor = title.ToLowerInvariant()
                .Replace(" ", "-")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("!", "")
                .Replace("?", "")
                .Replace("'", "")
                .Replace("\"", "");

            anchor = Regex.Replace(anchor, @"[^a-z0-9\-]", "");
            anchor = Regex.Replace(anchor, @"-+", "-");
            anchor = anchor.Trim('-');

            return anchor;
        }
    }

    /// <summary>
    /// Result of frontmatter extraction
    /// </summary>
    public class FrontmatterResult
    {
        public string Content { get; set; } = string.Empty;
        public Dictionary<string, object> Frontmatter { get; set; } = new();

        public T? GetFrontmatterValue<T>(string key, T? defaultValue = default)
        {
            if (!Frontmatter.TryGetValue(key, out var value))
                return defaultValue;

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }
    }

    /// <summary>
    /// Table of contents item
    /// </summary>
    public class TocItem
    {
        public int Level { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Anchor { get; set; } = string.Empty;
    }
}
