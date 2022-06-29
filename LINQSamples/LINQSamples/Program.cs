// See https://aka.ms/new-console-template for more information


using LINQSamples;

var mytest = new CounterList();

var totalCount = mytest.Items.Sum(s => s.TotalCount);
var unreadCount = mytest.Items.Sum(s => s.UnreadCount);




Console.WriteLine($"You have read {totalCount - unreadCount} of {totalCount} messages");
