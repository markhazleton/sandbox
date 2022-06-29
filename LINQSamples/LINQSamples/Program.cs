using LINQSamples.Models;

var mytest = new CounterList();
var totalCount = mytest.Items.Sum(s => s.TotalCount);
var unreadCount = mytest.Items.Sum(s => s.UnreadCount);
Console.WriteLine($"You have read {totalCount - unreadCount} of {totalCount} messages");
