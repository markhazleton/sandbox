namespace LINQSamples.Models;

public class StudentList
{
    public StudentList()
    {
        students = new List<Student>()
    {
        new Student { FirstName = "John", LastName = "Doe", Age = 18, Class = "A" },
        new Student { FirstName = "Jane", LastName = "Doe", Age = 19, Class = "B" },
        new Student { FirstName = "Bob", LastName = "Smith", Age = 20, Class = "C" },
        new Student { FirstName = "Marlis", LastName = "Hazleton", Age = 22, Class = "D" },
        new Student { FirstName = "Ian", LastName = "Hazleton", Age = 22, Class = "E" },
        new Student { FirstName = "Berit", LastName = "Hazleton", Age = 20, Class = "F" },
    };
    }
    public List<Student> students { get; set; }
}
