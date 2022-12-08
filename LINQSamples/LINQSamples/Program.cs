using LINQSamples.Models;

var mytest = new CounterList();
var totalCount = mytest.Items.Sum(s => s.TotalCount);
var unreadCount = mytest.Items.Sum(s => s.UnreadCount);
Console.WriteLine($"You have read {totalCount - unreadCount} of {totalCount} messages");

//
//  Response has Nullable List, need to put in Guard Clause
//
var result = new CounterList_Response();


result.Items?.OrderByDescending(o => o.UnreadCount)?.ToList().ForEach(item =>
{
    Console.WriteLine($"UnRead {item.UnreadCount} Total {item.TotalCount} messages");
});

foreach (var item in (result?.Items ?? new()).OrderByDescending(o => o.UnreadCount))
{
    Console.WriteLine($"UnRead {item.UnreadCount} Total {item.TotalCount} messages");
}



