namespace ConcurrentProcessing.Concurrent
{
    public static class MetricCalculator
    {
        public static void CalculateAndDisplayMetrics(List<ConcurrentProcessorModel> models)
        {
            if (models == null || models.Count == 0)
            {
                Console.WriteLine("No data to calculate metrics.");
                return;
            }

            // Calculate and display metrics for each property
            CalculateAndDisplayMetric(models, "TaskCount", m => m.TaskCount);
            CalculateAndDisplayMetric(models, "WaitTicks", m => m.WaitTicks);
            CalculateAndDisplayMetric(models, "SemaphoreCount", m => m.SemaphoreCount);
            CalculateAndDisplayMetric(models, "SemaphoreWait", m => m.SemaphoreWait);
            CalculateAndDisplayMetric(models, "TaskDuration", m => m.TaskDurationMS);
        }

        private static void CalculateAndDisplayMetric(
            List<ConcurrentProcessorModel> models,
            string metricName,
            Func<ConcurrentProcessorModel, long> metricSelector)
        {
            var metricValues = models.Select(metricSelector).ToList();

            if (metricValues.Count == 0)
            {
                Console.WriteLine($"No data for {metricName}.");
                return;
            }
            long min = metricValues.Min();
            long max = metricValues.Max();
            double average = metricValues.Average();
            Console.WriteLine($"{metricName.PadRight(20)}\tMinimum: {min}\tMaximum: {max}\tAverage: {average:F2}");
        }
    }
}