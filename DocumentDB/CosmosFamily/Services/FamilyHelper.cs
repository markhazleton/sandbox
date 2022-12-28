using CosmosFamily.Models;

namespace CosmosFamily.Services;
public static class FamilyHelper
{

    public static Family CreateHazletonFamily()
    {
        // Create a family object for the Andersen family
        var HazletonFamily = new Family
        {
            Id = "Hazleton.1",
            PartitionKey = "Hazleton",
            LastName = "Hazleton",
            Parents = new Parent[]
            {
                new Parent { FirstName = "Mark" },
                new Parent { FirstName = "Lesley" }
            },
            Address = new Address { State = "Texas", County = "Tarrant", City = "Roanoke" },
            IsRegistered = false
        };

        HazletonFamily.AddChild("Marlis", "F", 12);
        HazletonFamily.AddChild("Berit", "F", 10);
        HazletonFamily.AddChild("Ian", "F", 10);
        return HazletonFamily;

    }
    public static Family CreateWakefieldFamily()
    {
        return new Family()
        {
            Id = "Wakefield.7",
            PartitionKey = "Wakefield",
            LastName = "Wakefield",
            Parents = new Parent[]
                {
                    new Parent { FamilyName = "Wakefield", FirstName = "Robin" },
                    new Parent { FamilyName = "Miller", FirstName = "Ben" }
                },
            Children = new Child[]
                {
                    new Child
                    {
                        FamilyName = "Merriam",
                        FirstName = "Jesse",
                        Gender = "female",
                        Grade = 8,
                        Pets = new Pet[]
                        {
                            new Pet { GivenName = "Goofy" },
                            new Pet { GivenName = "Shadow" }
                        }
                    },
                    new Child
                    {
                        FamilyName = "Miller",
                        FirstName = "Lisa",
                        Gender = "female",
                        Grade = 1
                    }
                },
            Address = new Address { State = "New York", County = "Manhattan", City = "New York" },
            IsRegistered = true
        };

    }

}
