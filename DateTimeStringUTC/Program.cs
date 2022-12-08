using DateTimeStringUTC;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<BenchmarkingTests>();


[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BenchmarkingTests
{
    [Benchmark]
    public string Method1()
    {
        var IsoString = "2022-09-06T11:45:00-05:00";
        return DateTime.Parse(IsoString).ToDallasTime().ToString("h:mm tt");
    }
    [Benchmark]
    public string Method2()
    {
        var IsoString = "2022-09-06T11:45:00-05:00";
        return IsoString.TimeStringFromIsoDateString();
    }

}





