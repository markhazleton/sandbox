using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CoolMultipleAwaits;
public class TaskResultCollection
{
    public static async Task WhenAllWithLoggingAsync<T>(IEnumerable<Task> tasks, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "WAWLA:An error occurred while executing one or more tasks.");
        }
    }
    public async Task GetTaskResult<T>(string sectionName, Task<T> task) where T : class
    {
        var sw = new Stopwatch();
        sw.Start();
        var section = new TaskResult { Name = sectionName };
        try
        {
            section.Data = await task;
            sw.Stop();
            Telemetry.Add(GetTelemetry(sectionName, sw.ElapsedMilliseconds));
        }
        catch (Exception ex)
        {
            sw.Stop();
            Telemetry.Add(GetTelemetry(sectionName, sw.ElapsedMilliseconds, "Exception", ex.Message));
            section.Data = null;
        }
        finally
        {
            Sections.Add(section);
        }
    }
    private static string GetTelemetry(string sectionName, long elapsedTimeMS)
    {
        return sectionName is null
            ? throw new ArgumentNullException(nameof(sectionName))
            : $"{sectionName}: Request took {elapsedTimeMS:N0} MS";
    }
    private static string GetTelemetry(string sectionName, long elapsedTimeMS, string Code, string Description)
    {
        return sectionName is null
            ? throw new ArgumentNullException(nameof(sectionName))
            : $"{sectionName}: Request took {elapsedTimeMS:N0} MS with ERROR {Code}:{Description}";
    }


    public List<TaskResult> Sections { get; set; } = [];
    public string? TaskName { get; set; }
    public List<string> Telemetry { get; internal set; } = [];

    public class TaskResult
    {
        public TaskResult()
        {
            Name = "UNKNOWN";
        }
        public TaskResult(string name, object? data)
        {
            Name = name;
            Data = data;
        }
        public object? Data { get; set; }
        public string Name { get; set; }
    }
}
