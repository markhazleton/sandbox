using ThreadSafeDictionary;

PropertyBag<string, int> propertyBag = new();

// Adding key-value pairs
propertyBag.Add("Apple", 5);
propertyBag.Add("Banana", 3);
propertyBag.Add("Orange", 7);

// Updating a value
propertyBag["Banana"] = 10;

// Displaying key-value pairs using GetList method
List<string> keyValuePairList = propertyBag.GetList();
Console.WriteLine("Key-Value Pairs:");
foreach (string kvp in keyValuePairList)
{
    Console.WriteLine(kvp);
}

// Displaying key-value pairs using ToString method
Console.WriteLine("\nPropertyBag Contents:");
Console.WriteLine(propertyBag.ToString());

// Accessing values using indexer
Console.WriteLine("\nValue for key 'Apple': " + propertyBag["Apple"]);
Console.WriteLine("Value for key 'Banana': " + propertyBag["Banana"]);
Console.WriteLine("Value for key 'Grapes': " + propertyBag["Grapes"]); // Key not present

// Adding a dictionary
Dictionary<string, int> additionalPairs = new()
{
            { "Peach", 8 },
            { "Cherry", 4 }
        };
propertyBag.Add(additionalPairs);

// Displaying updated key-value pairs
Console.WriteLine("\nUpdated Key-Value Pairs:");
foreach (string kvp in propertyBag.GetList())
{
    Console.WriteLine(kvp);
}