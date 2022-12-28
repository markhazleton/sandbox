namespace LINQSamples.Models
{
    public class Student
    {
        public Student()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Class = string.Empty;
            Age = 0;
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Class { get; set; }
    }
}
