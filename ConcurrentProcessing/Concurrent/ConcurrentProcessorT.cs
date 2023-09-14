using System.Diagnostics;

namespace ConcurrentProcessing.Concurrent
{
    public abstract class ConcurrentProcessor<T>
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
            tasks = new List<Task<T>>();
        }

        protected async Task<long> AwaitSemaphoreAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            await semaphore.WaitAsync();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        protected virtual int? GetNextTaskId(int? taskId)
        {
            if (taskId < maxTaskCount) return taskId + 1;
            else return null;
        }

        protected async Task<T> ManageProcessAsync(int taskId, int taskCount, long waitMS, SemaphoreSlim semaphore)
        {
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            try
            {
                ConcurrentProcessorModel taskData = new()
                {
                    TaskId = taskId,
                    TaskCount = taskCount,
                    WaitMS = waitMS,
                    SemaphoreCount = semaphore.CurrentCount,
                    SemaphoreWait = waitMS
                };

                T result = await ProcessAsync(taskData);
                return result;
            }
            finally
            {
                semaphore.Release();
                sw.Stop();
            }
        }

        protected abstract Task<T> ProcessAsync(ConcurrentProcessorModel taskData);

        public async Task<List<T>> RunAsync()
        {
            int? taskId = 0;

            List<T> results = new();

            while (taskId is not null)
            {
                long waitMS = await AwaitSemaphoreAsync();
                Task<T> task = ManageProcessAsync(taskId.Value, tasks.Count, waitMS, semaphore);
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
}

