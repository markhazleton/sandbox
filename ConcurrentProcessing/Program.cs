using System.Diagnostics;

const int maxTaskCount = 10; // Maximum total tasks allowed
const int maxConcurrency = 2; // Maximum concurrent tasks allowed

SemaphoreSlim semaphore = new(maxConcurrency);
List<Task> tasks = new();
int? taskId = 0;
while (taskId is not null)
{
    Task task = ProcessAsync(taskId.Value, tasks.Count, await AwaitSemaphoreAsync(semaphore), semaphore);
    tasks.Add(task);
    taskId = GetNextTaskId(taskId);

    if (tasks.Count >= maxConcurrency)
    {
        Task finishedTask = await Task.WhenAny(tasks);
        tasks.Remove(finishedTask);
    }
}
await Task.WhenAll(tasks);

static int? GetNextTaskId(int? taskId)
{
    if (taskId < maxTaskCount) return taskId + 1;
    else return null;
}

static async Task ProcessAsync(int taskId, int taskCount, long waitTicks, SemaphoreSlim semaphore)
{
    try
    {
        await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(1, 500)));
        Console.WriteLine($"Task:{taskId:D3} T:{taskCount} S:{semaphore.CurrentCount} W:{waitTicks}");
    }
    finally
    {
        semaphore.Release();
    }
}

static async Task<long> AwaitSemaphoreAsync(SemaphoreSlim semaphore)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    await semaphore.WaitAsync();
    stopwatch.Stop();
    return stopwatch.ElapsedTicks;
}