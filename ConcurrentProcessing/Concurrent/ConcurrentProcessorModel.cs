namespace ConcurrentProcessing.Concurrent;

public class ConcurrentProcessorModel
{
    public int TaskId { get; set; }
    public int TaskCount { get; set; }
    public long WaitTicks { get; set; }
    public int SemaphoreCount { get; set; }
    public long SemaphoreWait { get; set; }
    public long TaskDurationMS { get; set; }

    public override string? ToString()
    {
        return $"Task:{TaskId:D3} DurationMS:{TaskDurationMS:D5} WaitTicks:{WaitTicks:D5} TaskList:{TaskCount:D2} SemaphoreCount:{SemaphoreCount:D2} SemaphoreWait:{SemaphoreWait:D4}";
    }

}

