using CSVSamples;
using LINQSamples.Models;

Console.WriteLine("Start of CSV Sample");

List<Student> students = new StudentList().students;

Console.WriteLine($"Got List with {students.Count} students");

students.WriteToCSV<Student>(Resources.STRStudentsFileName);

Console.WriteLine($"Write student list to csv file named {Resources.STRStudentsFileName}.");

students.Clear();
Console.WriteLine($"Cleared List now have {students.Count} students.");

students = students.LoadFromCsv<Student>(Resources.STRStudentsFileName);
Console.WriteLine($"Load List from CSV with {students.Count} students.");

Console.WriteLine("End of CSV Sample");

