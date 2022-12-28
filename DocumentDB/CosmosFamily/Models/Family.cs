using Newtonsoft.Json;

namespace CosmosFamily.Models;

public class Family
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public string LastName { get; set; }
    public Parent[] Parents { get; set; }
    public Child[] Children { get; set; }
    public Address Address { get; set; }
    public bool IsRegistered { get; set; }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public bool AddChild(string firstName, string gender, int grade, string? familyName = null)
    {
        List<Child> init = this.Children is null ? new List<Child>() : this.Children.ToList();
        init.Add(new Child()
        {
            FamilyName = familyName ?? this.LastName,
            FirstName = firstName,
            Gender = gender,
            Grade = grade
        }
        );
        this.Children = init.ToArray();
        return true;
    }


}
