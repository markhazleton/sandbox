using LINQSamples.Models;
using System.Reflection;
using System.Text;

namespace CSVSamples;

public static class ListExtensions
{
    private static string GetCsvString<T>(T model)
    {
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
            else if (property.PropertyType.IsPrimitive)
            {
                dataRow.Add(value is null ? string.Empty : value.ToString());
            }
            else
            {
                dataRow.Add(string.Empty);
            }
        }
        // Return the CSV string
        return string.Join(",", dataRow);
    }
    private static DateTime GetDateFromString(string s)
    {
        if (DateTime.TryParse(s, out DateTime date))
        {
            return date;
        }
        else
        {
            return DateTime.MinValue;
        }
    }

    public static List<T> LoadFromCsv<T>(this List<T> models, string fileName)
    {
        // Read the contents of the CSV file into a string
        string csvString = File.ReadAllText(fileName);

        // Split the CSV string into lines
        string[] lines = csvString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        // Split the first line (the header row) into column names
        string[] headerColumns = lines[0].Split(new string[] { ",", "\"" }, StringSplitOptions.RemoveEmptyEntries);

        // Create an empty list of objects of the model class
        models = new();

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
        return models;
    }

    public static void WriteToCSV<T>(this List<T> listT, string path)
    {
        var properties = typeof(Student).GetProperties();
        List<string> headerRow = new();
        foreach (var property in properties)
        {
            headerRow.Add(property.Name);
        }
        StringBuilder csv = new();
        csv.AppendLine(string.Join(",", headerRow));
        foreach (var student in listT)
        {
            csv.AppendLine(GetCsvString<T>(student));
        }
        using StreamWriter sw = new(path);
        sw.Write(csv.ToString());
    }


}
