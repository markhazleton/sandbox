namespace FuncsExamples;

public class SimpleFuncsNoInputParameters
{
    public static void Runner()
    {
        Console.WriteLine(DateCalculator(() => Math.Round(DateTime.Now.Subtract(new DateTime(1966, 1, 22)).TotalDays, 2)));
        Console.WriteLine(DateCalculator(() => Math.Round(DateTime.Now.Subtract(new DateTime(2000, 1, 1)).TotalDays, 2)));
        Console.WriteLine(DateCalculator(() => Math.Round(new DateTime(2020, 12, 31).Subtract(DateTime.Now).TotalDays, 2)));
    }

    public static string DateCalculator(Func<double> daysSinceSomeDate)
    {
        return $"{daysSinceSomeDate()} days in your date calculation";
    }
}
