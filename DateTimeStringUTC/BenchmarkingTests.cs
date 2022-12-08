using BenchmarkDotNet.Attributes;

namespace DateTimeStringUTC;

[MemoryDiagnoser]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class BenchmarkingTests
{
    private string[] IsoString = new string[]
    {
        "2022-09-06T01:05:00-05:00",
        "2022-09-06T00:05:00-05:00",
        "2022-09-06T03:45:00-05:00",
        "2022-09-06T11:45:00-05:00",
        "2022-09-06T17:45:00-05:00"
    };

    [Benchmark]
    public string[] TimeStringFromIsoString()
    {
        for (int i = 0; i < IsoString.Length; i++)
        {
            IsoString[i] = IsoString[i].TimeStringFromIsoDateString();

        }
        return IsoString;
    }

    [Benchmark]
    public string[] ConvertDateToShortTime()
    {
        for (int i = 0; i < IsoString.Length; i++)
        {
            IsoString[i] = IsoString[i].DateTimeFromIsoString().ToShortTimeString();
        }
        return IsoString;
    }

}

