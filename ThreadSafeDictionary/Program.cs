using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

class Program
{
    private static ConcurrentQueue<string> stringQueue = new ConcurrentQueue<string>();

    static void Main(string[] args)
    {
        Task task1 = Task.Run(() => EnqueueStrings("Thread 1"));
        Task task2 = Task.Run(() => EnqueueStrings("Thread 2"));

        Task.WhenAll(task1, task2).Wait();

        ProcessQueue();

        foreach (var item in stringQueue)
        {
            Console.WriteLine($"Item: {item}");
        }
    }

    static void EnqueueStrings(string threadName)
    {
        for (int i = 0; i < 10; i++)
        {
            stringQueue.Enqueue($"{threadName} - Item {i}");
        }
    }

    static void ProcessQueue()
    {
        while (stringQueue.TryDequeue(out string item))
        {
            Console.WriteLine($"Processed item: {item}");
        }
    }
}
