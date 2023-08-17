using ConcurrentProcessing.Concurrent;

MyTaskProcessor taskProcessor = new(maxTaskCount: 100, maxConcurrency: 10);

List<MyTask> results = await taskProcessor.RunAsync();

foreach (var result in results)
{
    if (result is null) continue;
    Console.WriteLine(result.ToString());
}

class MyTaskProcessor : ConcurrentProcessor<MyTask>
{
    public MyTaskProcessor(int maxTaskCount, int maxConcurrency)
        : base(maxTaskCount, maxConcurrency)
    {
    }

    protected override async Task<MyTask> ProcessAsync(ConcurrentProcessorModel taskData)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(500, 1000)));

        return new MyTask(taskData);
    }
}



