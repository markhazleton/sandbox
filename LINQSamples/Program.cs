using LINQSamples.Models;

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

