using ConcurrentProcessing.Concurrent;
using ConcurrentProcessing.Sample;
using System.Diagnostics;

await RunConcurrent(100, 1);
await RunConcurrent(100, 10);
await RunConcurrent(100, 20);
await RunConcurrent(100, 40);
await RunConcurrent(100, 80);
await RunConcurrent(100, 100);
await RunConcurrent(1000, 500);
await RunConcurrent(1000, 1000);

static async Task RunConcurrent(int maxTaskCount, int maxConcurrency)
{
    Stopwatch sw = Stopwatch.StartNew();
    Console.WriteLine($"Starting {maxTaskCount} tasks with a max concurrency of {maxConcurrency}...");
    SampleTaskProcessor taskProcessor = new(maxTaskCount: maxTaskCount, maxConcurrency: maxConcurrency);
    List<SampleTaskResult> results = await taskProcessor.RunAsync();
    List<ConcurrentProcessorModel> concurrentModels = results.Cast<ConcurrentProcessorModel>().ToList();
    MetricCalculator.CalculateAndDisplayMetrics(concurrentModels);
    sw.Stop();
    Console.WriteLine($"Total Duration: {sw.ElapsedMilliseconds}ms");
}

