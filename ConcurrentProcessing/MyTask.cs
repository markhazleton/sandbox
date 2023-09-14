using ConcurrentProcessing.Concurrent;

class MyTask : ConcurrentProcessorModel
{
    public MyTask(ConcurrentProcessorModel model)
    {
        TaskId = model.TaskId;
        TaskCount = model.TaskCount;
        WaitMS = model.WaitMS;
        SemaphoreCount = model.SemaphoreCount;
        SemaphoreWait = model.SemaphoreWait;
    }

}



