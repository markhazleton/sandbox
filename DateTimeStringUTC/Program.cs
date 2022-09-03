using DateTimeStringUTC;

var test = new BenchmarkingTests();

foreach (var item in test.TimeStringFromIsoString())
{
    Console.WriteLine(item);
}
foreach (var item in test.ConvertDateToShortTime())
{
    Console.WriteLine(item);
}


BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchmarkingTests>();
