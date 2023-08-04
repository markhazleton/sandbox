namespace LINQSamples.Models
{
    public class MyCompany
    {
        public List<EmployeeModel> Employees { get; set; }
        public EmployeeModel? GetEmployeeByLogin(string login) => Employees.FirstOrDefault(e => e.Logins.Any(l => l.LoginName == login));
    }
}
