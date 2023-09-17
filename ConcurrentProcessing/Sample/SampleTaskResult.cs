using ConcurrentProcessing.Concurrent;

namespace ConcurrentProcessing.Sample;

public class SampleTaskResult : ConcurrentProcessorModel
{
    public SampleTaskResult(ConcurrentProcessorModel model)
    {
        TaskId = model.TaskId;
        TaskCount = model.TaskCount;
        WaitTicks = model.WaitTicks;
        SemaphoreCount = model.SemaphoreCount;
        SemaphoreWait = model.SemaphoreWait;
    }
}
