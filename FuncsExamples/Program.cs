using FuncsExamples;

SimpleFuncsNoInputParameters.Runner();

IEnumerableExtensions.TestFlatten();

Func<string, string> UpperCase = str => str.ToUpper();
// Create an array of strings.
string[] words = { "orange", "apple", "Article", "elephant" };
// Query the array and select strings according to the UpperCase method.
IEnumerable<String> aWords = words.Select(UpperCase);
// Output the results to the console.
foreach (String word in aWords)
    Console.WriteLine(word);

