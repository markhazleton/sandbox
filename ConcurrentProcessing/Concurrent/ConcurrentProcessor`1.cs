using System.Diagnostics;

namespace ConcurrentProcessing.Concurrent;

public abstract class ConcurrentProcessor<T> where T : ConcurrentProcessorModel
{
    private readonly int maxConcurrency;
    private readonly int maxTaskCount;
    private readonly SemaphoreSlim semaphore;
    private readonly List<Task<T>> tasks;

    protected ConcurrentProcessor(int maxTaskCount, int maxConcurrency)
    {
        this.maxTaskCount = maxTaskCount;
        this.maxConcurrency = maxConcurrency;
        semaphore = new SemaphoreSlim(maxConcurrency);
        tasks = [];
    }

    protected async Task<long> AwaitSemaphoreAsync()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        await semaphore.WaitAsync();
        stopwatch.Stop();
        return stopwatch.ElapsedTicks;
    }

    protected virtual int? GetNextTaskId(int? taskId)
    {
        if (taskId < maxTaskCount) return taskId + 1;
        else return null;
    }

    protected async Task<T> ManageProcessAsync(int taskId, int taskCount, long waitTicks, SemaphoreSlim semaphore) 
    {
        Stopwatch sw = Stopwatch.StartNew();
        sw.Start();
        T? result = default;
        try
        {
            ConcurrentProcessorModel taskData = new()
            {
                TaskId = taskId,
                TaskCount = taskCount,
                WaitTicks = waitTicks,
                SemaphoreCount = semaphore.CurrentCount,
                SemaphoreWait = waitTicks
            };

            result = await ProcessAsync(taskData);
        }
        finally
        {
            semaphore.Release();
            sw.Stop();
            if(result is not null)
                result.TaskDurationMS = sw.ElapsedMilliseconds;
        }
        return result;
    }

    protected abstract Task<T> ProcessAsync(ConcurrentProcessorModel taskData);

    public async Task<List<T>> RunAsync()
    {
        int? taskId = 1;

        List<T> results = [];

        while (taskId is not null)
        {
            long waitTicks = await AwaitSemaphoreAsync();
            Task<T> task = ManageProcessAsync(taskId.Value, tasks.Count, waitTicks, semaphore);
            tasks.Add(task);
            taskId = GetNextTaskId(taskId);

            if (tasks.Count >= maxConcurrency)
            {
                Task<T> finishedTask = await Task.WhenAny(tasks);
                results.Add(await finishedTask);
                tasks.Remove(finishedTask);
            }
        }
        await Task.WhenAll(tasks);
        foreach (var task in tasks)
        {
            results.Add(await task); // Add the remaining task results to the list
        }
        return results;

    }
}
