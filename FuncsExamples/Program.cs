using FuncsExamples;

// SimpleFuncsNoInputParameters.Runner();
// IEnumerableExtensions.TestFlatten();
// Func<string, string> UpperCase = str => str.ToUpper();
// Create an array of strings.
// string[] words = { "orange", "apple", "Article", "elephant" };
// Query the array and select strings according to the UpperCase method.
// IEnumerable<String> aWords = words.Select(UpperCase);
// Output the results to the console.
// foreach (String word in aWords)
//    Console.WriteLine(word);

var pCount = new Patientcount();

Console.WriteLine(TestCounts.SumUnreadMessages(pCount));
Console.WriteLine(TestCounts.SumUnreadMessagesTwo(pCount));

pCount.LetterCount = 1;

Console.WriteLine(TestCounts.SumUnreadMessages(pCount));
Console.WriteLine(TestCounts.SumUnreadMessagesTwo(pCount));

pCount.CareCompanionMessageCount = 1;

Console.WriteLine(TestCounts.SumUnreadMessages(pCount));
Console.WriteLine(TestCounts.SumUnreadMessagesTwo(pCount));

pCount.ConversationCount = 1;   

Console.WriteLine(TestCounts.SumUnreadMessages(pCount));
Console.WriteLine(TestCounts.SumUnreadMessagesTwo(pCount));

pCount.LabResultsCount= 1;

Console.WriteLine(TestCounts.SumUnreadMessages(pCount));
Console.WriteLine(TestCounts.SumUnreadMessagesTwo(pCount));



public static class TestCounts
{
    public static int SumUnreadMessagesTwo(Patientcount? pCount)
    {
        if (pCount is null) return 0;
        int totalUnread = (
            (pCount.CareCompanionMessageCount ?? 0)
            + (pCount.LabResultsCount ?? 0)
            + (pCount.LetterCount ?? 0)
            + (pCount.ConversationCount ?? 0)
            + (pCount.AppointmentMessageCount ?? 0));
        return totalUnread;
    }

    public static int SumUnreadMessages(Patientcount? patientCount)
    {
        if (patientCount is null) return 0;
        int sum = 0;
        if (patientCount.CareCompanionMessageCount.HasValue)
            sum += patientCount.CareCompanionMessageCount.Value;
        if (patientCount.LabResultsCount.HasValue)
            sum += patientCount.LabResultsCount.Value;
        if (patientCount.LetterCount.HasValue)
            sum += patientCount.LetterCount.Value;
        if (patientCount.ConversationCount.HasValue)
            sum += patientCount.ConversationCount.Value;
        if (patientCount.AppointmentMessageCount.HasValue)
            sum += patientCount.AppointmentMessageCount.Value;
        return sum;
    }
}

