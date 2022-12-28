using CSVSamples;
using LINQSamples.Models;
using System.Reflection;
using System.Text;

Console.WriteLine("Start of CSV Sample");

List<Student> students = GetStudentList();
Console.WriteLine($"Got List with {students.Count} students.");

WriteToCSV(students, Resources.STRStudentsFileName);
Console.WriteLine($"Write student list to csv file named {Resources.STRStudentsFileName}.");

students.Clear();
Console.WriteLine($"Cleared List now have {students.Count} students.");

students = LoadFromCsv<Student>(Resources.STRStudentsFileName);
Console.WriteLine($"Load List from CSV with {students.Count} students.");

Console.WriteLine("End of CSV Sample");

string GetCsvString<T>(T model)
{
    // Use the StringBuilder class to create the CSV string
    StringBuilder csv = new();

    // Get the list of properties of the model class
    var properties = typeof(T).GetProperties();

    // Create a list of strings to hold the data row
    List<string> dataRow = new();

    // Iterate through the properties and add their values to the data row,
    // enclosing strings in quotes and formatting dates
    foreach (var property in properties)
    {
        object? value = property.GetValue(model, null);
        if (property.PropertyType == typeof(string))
        {
            dataRow.Add($"\"{value}\"");
        }
        else if (property.PropertyType == typeof(DateTime))
        {
            dataRow.Add($"\"{(DateTime)value:yyyy-MM-dd}\"");
        }
        else
        {
            dataRow.Add(value is null ? string.Empty : value.ToString());
        }
    }
    // Return the CSV string
    return string.Join(",", dataRow);
}

List<T> LoadFromCsv<T>(string fileName)
{
    // Read the contents of the CSV file into a string
    string csvString = File.ReadAllText(fileName);

    // Split the CSV string into lines
    string[] lines = csvString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

    // Split the first line (the header row) into column names
    string[] headerColumns = lines[0].Split(new string[] { ",", "\"" }, StringSplitOptions.RemoveEmptyEntries);

    // Create an empty list of objects of the model class
    List<T> models = new();

    // Iterate through the remaining lines (the data rows)
    for (int i = 1; i < lines.Length; i++)
    {
        // Split the line into values
        string[] dataColumns = lines[i].Split(new string[] { ",", "\"" }, StringSplitOptions.RemoveEmptyEntries);

        // If there is a blank or invalid row, then skip it
        if (dataColumns.Length < headerColumns.Length) continue;

        // Create a new object of the model class
        T model = (T)Activator.CreateInstance(typeof(T));

        // Set the values of the object's properties using the values from the CSV file
        for (int j = 0; j < headerColumns.Length; j++)
        {
            PropertyInfo property = typeof(T).GetProperty(headerColumns[j]);
            if (property.PropertyType == typeof(string))
            {
                // If the value is a string, remove the leading and trailing quotes
                property.SetValue(model, dataColumns[j].Trim('"'), null);
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                // If the value is a date, parse it using the specified format
                property.SetValue(model, GetDateFromString(dataColumns[j]), null);
            }
            else
            {
                // For other types, use the Convert class to parse the value
                property.SetValue(model, Convert.ChangeType(dataColumns[j], property.PropertyType), null);
            }
        }
        // Add the object to the list
        if (model is null)
        {
            continue;
        }
        models.Add(model);
    }
    // Return the list of models
    return models;
}
DateTime GetDateFromString(string s)
{
    // DateTime.ParseExact(dataColumns[j], "yyyy-MM-dd", CultureInfo.InvariantCulture)
    if (DateTime.TryParse(s, out DateTime date))
    {
        return date;
    }
    else
    {
        return DateTime.MinValue;
    }

}

static List<Student> GetStudentList()
{
    return new()
{
    new Student { FirstName = "John", LastName = "Doe", Age = 18, Class = "A" },
    new Student { FirstName = "Jane", LastName = "Doe", Age = 19, Class = "B" },
    new Student { FirstName = "Bob", LastName = "Smith", Age = 20, Class = "C" },
    new Student { FirstName = "Marlis", LastName = "Hazleton", Age = 22, Class = "D" },
    new Student { FirstName = "Ian", LastName = "Hazleton", Age = 22, Class = "E" },
    new Student { FirstName = "Berit", LastName = "Hazleton", Age = 20, Class = "F" },
};
}

void WriteToCSV(List<Student> students, string path)
{
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
        csv.AppendLine(GetCsvString<Student>(student));
    }
    using StreamWriter sw = new(path);
    sw.Write(csv.ToString());
}