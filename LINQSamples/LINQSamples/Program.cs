using LINQSamples.Models;
using System.Reflection;
using System.Text;

List<Student> students = new()
{
    new Student { FirstName = "John", LastName = "Doe", Age = 18, Class = "A" },
    new Student { FirstName = "Jane", LastName = "Doe", Age = 19, Class = "B" },
    new Student { FirstName = "Bob", LastName = "Smith", Age = 20, Class = "C" },
    new Student { FirstName = "Marlis", LastName = "Hazleton", Age = 22, Class = "D" },
    new Student { FirstName = "Ian", LastName = "Hazleton", Age = 22, Class = "E" },
    new Student { FirstName = "Berit", LastName = "Hazleton", Age = 20, Class = "F" },
};
var properties = typeof(Student).GetProperties();
List<string> headerRow = new();
foreach (var property in properties)
{
    headerRow.Add(property.Name);
}
StringBuilder csv = new();
csv.AppendLine(string.Join(",", headerRow));

foreach (var student in students)
{
    List<string> dataRow = new();
    foreach (var property in properties)
    {
        dataRow.Add(property.GetValue(student, null).ToString());
    }
    csv.AppendLine(string.Join(",", dataRow));
}

string csvString = csv.ToString();
string[] lines = csvString.Split('\n');
string[] headerColumns = lines[0].Split(',');
for (int i = 1; i < lines.Length; i++)
{

}
using (StreamWriter sw = new("students.csv"))
{
    sw.Write(csv.ToString());
}

using (StreamReader sr = new("students.csv"))
{
    csvString = sr.ReadToEnd();
}

csvString = File.ReadAllText("students.csv");
lines = csvString.Split('\n');
headerColumns = lines[0].Split(',');
students = new List<Student>();

for (int i = 1; i < lines.Length; i++)
{
    string[] dataColumns = lines[i].Split(',');
    if (dataColumns is null) continue;
    if (dataColumns.Length == 0) continue;
    if (dataColumns.Length == 1) continue;

    // Create a new object of your model class
    Student student = new();
    // Set the values of the object's properties using the values from the CSV file
    for (int j = 0; j < headerColumns.Length; j++)
    {
        PropertyInfo property = typeof(Student).GetProperty(headerColumns[j]);
        property?.SetValue(student, Convert.ChangeType(dataColumns[j], property.PropertyType), null);
    }
    // Add the object to the list
    students.Add(student);
}
Console.WriteLine("End of CSV Sample");

var mytest = new CounterList();
var totalCount = mytest.Items.Sum(s => s.TotalCount);
var unreadCount = mytest.Items.Sum(s => s.UnreadCount);
Console.WriteLine($"You have read {totalCount - unreadCount} of {totalCount} messages");

//
//  Response has Nullable List, need to put in Guard Clause
//
CounterList_Response? result = new();
result.Items?.OrderByDescending(o => o.UnreadCount)?.ToList()
    .ForEach(
        item =>
        {
            Console.WriteLine($"UnRead {item.UnreadCount} Total {item.TotalCount} messages");
        });
foreach (var item in (result?.Items ?? new()).OrderByDescending(o => o.UnreadCount))
{
    Console.WriteLine($"UnRead {item.UnreadCount} Total {item.TotalCount} messages");
}
List<int?>? nullableList = new();
ProcessList(nullableList);

nullableList = null;
ProcessList(nullableList);

nullableList = new List<int?>()
{
    null,0,1,2,null,4
};

ProcessList(nullableList);

static void ProcessList(List<int?>? nullableList)
{
    Console.WriteLine($"Process List");
    if (nullableList is null) Console.WriteLine($"List Is Null");

    foreach (int? value in nullableList ?? new List<int?>())
    {
        if (value.HasValue)
        {
            Console.WriteLine($"Has Value: {value}");
        }
        else
        {
            Console.WriteLine($"Is Null");
        }
    }
}

