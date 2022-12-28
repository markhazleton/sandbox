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
            StartDate = DateTime.Now;
        }
        public DateTime StartDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Class { get; set; }
    }
}
